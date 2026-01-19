using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Item Struct Flags
/// </summary>
public struct ItemStructFlags
{
    /// <summary>
    /// Size of Item Struct Flags in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw 16-bit value of the flags.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemStructFlags"/> struct.
    /// </summary>
    public readonly bool HasIcon => (RawValue & 0x8000) != 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemStructFlags"/> struct.
    /// </summary>
    public readonly ReferenceType IconReferenceType => (ReferenceType)(RawValue & 0x0003);

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemStructFlags"/> struct.
    /// </summary>
    public ItemStructFlags(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid data length for ItemStructFlags: expected {Size}, got {data.Length}", nameof(data));
        }

        // // Structure documented in https://apple2.gs/downloads/library/Apple%20IIgs%20Toolbox%20Changes%20for%20System%20Software%206.0.pdf
        // p49 to p50
        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for ItemStructFlags");
    }
}
