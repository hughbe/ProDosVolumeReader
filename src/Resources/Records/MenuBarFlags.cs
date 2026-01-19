using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;
 
/// <summary>
/// Represents the flags field in a MenuBarRecord.
/// </summary>
public readonly struct MenuBarFlags
{
    /// <summary>
    /// Size of the MenuBarFlags field in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// The raw 16-bit value of the flags.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Gets the menu reference type (bits 14-15).
    /// 00 - Pointers, 01 - Handles, 10 - Resource IDs, 11 - Invalid
    /// </summary>
    public ReferenceType MenuReferenceType => (ReferenceType)((RawValue >> 14) & 0b11);

    /// <summary>
    /// Gets the reserved bits (0-13). Should be 0.
    /// </summary>
    public ushort Reserved => (ushort)(RawValue & 0x3FFF);

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuBarFlags"/> struct from a span of bytes.
    /// </summary>
    /// <param name="data">A span of at least 2 bytes containing the flags value in little-endian order.</param>
    public MenuBarFlags(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"MenuBarFlags requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for MenuBarFlags.");
    }
}
