using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Reference types for WindowParam1Record (color table, title, control list).
/// </summary>
public readonly struct WindowParam1ReferenceTypes
{
    /// <summary>
    /// Size of WindowParam1ReferenceTypes structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw value
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowParam1ReferenceTypes"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the WindowParam1ReferenceTypes.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public WindowParam1ReferenceTypes(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"WindowParam1ReferenceTypes requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data);
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for WindowParam1ReferenceTypes.");
    }

    /// <summary>
    /// Color table reference type (bits 10-11)
    /// </summary>
    public ReferenceType ColorTableReferenceType => (ReferenceType)((RawValue >> 10) & 0x3);

    /// <summary>
    /// Title reference type (bits 8-9)
    /// </summary>
    public ReferenceType TitleReferenceType => (ReferenceType)((RawValue >> 8) & 0x3);

    /// <summary>
    /// Control reference type (bits 0-7)
    /// </summary>
    public ReferenceType ControlReferenceType => (ReferenceType)(RawValue & 0xFF);
}
