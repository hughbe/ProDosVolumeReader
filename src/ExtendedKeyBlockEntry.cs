using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a mini-entry in an extended key block for GS/OS forked files.
/// Each fork (data and resource) has its own entry at different offsets in the block.
/// </summary>
public readonly struct ExtendedKeyBlockEntry
{
    /// <summary>
    /// The size in bytes of a mini-entry (without optional Finder info).
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// Gets the storage type for the fork.
    /// Must be 1 (Seedling), 2 (Sapling), or 3 (Tree).
    /// </summary>
    public StorageType StorageType { get; }

    /// <summary>
    /// Gets the key block for the fork.
    /// </summary>
    public ushort KeyBlock { get; }

    /// <summary>
    /// Gets the number of blocks used by the fork.
    /// </summary>
    public ushort BlocksUsed { get; }

    /// <summary>
    /// Gets the end-of-file position (size in bytes) of the fork.
    /// </summary>
    public uint Eof { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedKeyBlockEntry"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the mini-entry.</param>
    /// <exception cref="ArgumentException">Thrown if the data length is not equal to <see cref="Size"/>.</exception>
    public ExtendedKeyBlockEntry(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be exactly {Size} bytes for ExtendedKeyBlockEntry", nameof(data));
        }

        int offset = 0;

        // +$00 / 1: storage type for fork (must be 1/2/3)
        StorageType = (StorageType)data[offset];
        offset += 1;

        // +$01 / 2: key block
        KeyBlock = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // +$03 / 2: blocks used
        BlocksUsed = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // +$05 / 3: EOF (24-bit value)
        Eof = (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16));
        offset += 3;

        Debug.Assert(offset == data.Length, "Did not consume all data for ExtendedKeyBlockEntry");
    }

    /// <summary>
    /// Gets a value indicating whether this entry represents a valid fork.
    /// </summary>
    public bool IsValid => StorageType is StorageType.Seedling or StorageType.Sapling or StorageType.Tree;

    /// <summary>
    /// Gets a value indicating whether this fork is empty.
    /// </summary>
    public bool IsEmpty => KeyBlock == 0 && Eof == 0;
}
