using System.Numerics;
using System.Runtime.InteropServices;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a ProDOS volume block allocation bitmap.
/// </summary>
/// <remarks>
/// The bitmap usually starts on block 6, immediately after the volume directory,
/// but that's not mandatory. One bit is assigned for every block on the volume;
/// with 8*512=4096 bits per block, the bitmap will span at most ceil(65535/4096)=16 blocks.
/// 
/// Each byte holds the bits for 8 consecutive blocks, with the lowest-numbered block
/// in the high bit. Bits are set for unallocated blocks, so a full disk will have
/// nothing but zeroes.
/// </remarks>
public class VolumeBlockAllocationBitmap
{
    private const int BlockSize = 512;
    private const int BitsPerBlock = BlockSize * 8; // 4096 bits per block

    private readonly byte[] _bitmapData;
    private readonly ushort _totalBlocks;

    /// <summary>
    /// Gets the raw bitmap data.
    /// </summary>
    public ReadOnlySpan<byte> RawData => _bitmapData;

    /// <summary>
    /// Gets the total number of blocks on the volume.
    /// </summary>
    public ushort TotalBlocks => _totalBlocks;

    /// <summary>
    /// Gets the number of free (unallocated) blocks on the volume.
    /// </summary>
    public int FreeBlockCount { get; }

    /// <summary>
    /// Gets the number of used (allocated) blocks on the volume.
    /// </summary>
    public int UsedBlockCount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeBlockAllocationBitmap"/> class.
    /// </summary>
    /// <param name="bitmapData">The raw bitmap data read from the disk.</param>
    /// <param name="totalBlocks">The total number of blocks on the volume.</param>
    public VolumeBlockAllocationBitmap(byte[] bitmapData, ushort totalBlocks)
    {
        _bitmapData = bitmapData;
        _totalBlocks = totalBlocks;

        // Count free blocks using hardware-accelerated popcount on full bytes,
        // then handle any remaining bits in the last partial byte.
        int fullBytes = totalBlocks / 8;
        int remainingBits = totalBlocks % 8;

        int freeCount = 0;

        // Process 4 bytes at a time using uint PopCount for maximum throughput.
        ReadOnlySpan<byte> data = bitmapData.AsSpan(0, fullBytes);
        ReadOnlySpan<uint> uints = MemoryMarshal.Cast<byte, uint>(data);
        for (int i = 0; i < uints.Length; i++)
        {
            freeCount += BitOperations.PopCount(uints[i]);
        }

        // Handle remaining full bytes that didn't fit into a uint.
        for (int i = uints.Length * 4; i < fullBytes; i++)
        {
            freeCount += BitOperations.PopCount(bitmapData[i]);
        }

        // Handle the last partial byte (only the top 'remainingBits' bits are valid).
        if (remainingBits > 0 && fullBytes < bitmapData.Length)
        {
            byte lastByte = bitmapData[fullBytes];
            // Mask off the low bits that don't correspond to real blocks.
            byte mask = (byte)(0xFF << (8 - remainingBits));
            freeCount += BitOperations.PopCount((uint)(lastByte & mask));
        }

        FreeBlockCount = freeCount;
        UsedBlockCount = totalBlocks - freeCount;
    }

    /// <summary>
    /// Determines whether the specified block is free (unallocated).
    /// </summary>
    /// <param name="blockNumber">The block number to check.</param>
    /// <returns><see langword="true"/> if the block is free; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the block number is out of range.</exception>
    public bool IsBlockFree(ushort blockNumber)
    {
        if (blockNumber >= _totalBlocks)
        {
            throw new ArgumentOutOfRangeException(nameof(blockNumber), $"Block number {blockNumber} is out of range. Total blocks: {_totalBlocks}.");
        }

        // Each byte holds bits for 8 consecutive blocks.
        // The lowest-numbered block is in the high bit (bit 7).
        // Bits are set (1) for unallocated blocks.
        int byteIndex = blockNumber / 8;
        int bitIndex = 7 - (blockNumber % 8); // High bit = lowest block number

        if (byteIndex >= _bitmapData.Length)
        {
            return false;
        }

        return (_bitmapData[byteIndex] & (1 << bitIndex)) != 0;
    }

    /// <summary>
    /// Determines whether the specified block is used (allocated).
    /// </summary>
    /// <param name="blockNumber">The block number to check.</param>
    /// <returns><see langword="true"/> if the block is used; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the block number is out of range.</exception>
    public bool IsBlockUsed(ushort blockNumber)
    {
        return !IsBlockFree(blockNumber);
    }

    /// <summary>
    /// Enumerates all free (unallocated) block numbers.
    /// </summary>
    /// <returns>An enumerable of free block numbers.</returns>
    public IEnumerable<ushort> EnumerateFreeBlocks()
    {
        for (ushort i = 0; i < _totalBlocks; i++)
        {
            if (IsBlockFree(i))
            {
                yield return i;
            }
        }
    }

    /// <summary>
    /// Enumerates all used (allocated) block numbers.
    /// </summary>
    /// <returns>An enumerable of used block numbers.</returns>
    public IEnumerable<ushort> EnumerateUsedBlocks()
    {
        for (ushort i = 0; i < _totalBlocks; i++)
        {
            if (IsBlockUsed(i))
            {
                yield return i;
            }
        }
    }

    /// <summary>
    /// Calculates the number of bitmap blocks required for the given total block count.
    /// </summary>
    /// <param name="totalBlocks">The total number of blocks on the volume.</param>
    /// <returns>The number of blocks needed for the bitmap.</returns>
    public static int CalculateBitmapBlockCount(ushort totalBlocks)
    {
        // Each block holds 4096 bits (512 bytes * 8 bits)
        // We need ceil(totalBlocks / 4096) blocks
        return (totalBlocks + BitsPerBlock - 1) / BitsPerBlock;
    }
}
