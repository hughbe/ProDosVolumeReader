using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources;

/// <summary>
/// The GS/OS Resource Fork Free Block.
/// </summary>
public readonly struct GsOsResourceForkFreeBlock
{
    /// <summary>
    /// Size of Resource Fork Free Block structure in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// Offset of the free block in bytes.
    /// </summary>
    public uint BlockOffset { get; }

    /// <summary>
    /// Size of the free block in bytes.
    /// </summary>
    public uint BlockSize { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GsOsResourceForkFreeBlock"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the free block.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public GsOsResourceForkFreeBlock(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data length must be {Size} bytes.", nameof(data));
        }

        // Structure documented in https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
        // and https://ciderpress2.com/formatdoc/ResourceFork-notes.html
        int offset = 0;

        // Offset, in bytes, to the free block from the start of the resource fork.
        // A NIL value indicates the end of the used blocks in the array.
        BlockOffset = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Size, in bytes, of the free block of space.
        BlockSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all data for ResourceForkFreeBlock.");
    }
}