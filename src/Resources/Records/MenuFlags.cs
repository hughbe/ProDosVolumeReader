namespace ProDosVolumeReader.Resources.Records;

using System.Buffers.Binary;
using System.Diagnostics;

/// <summary>
/// Bit flags controlling the display and processing attributes of the menu.
/// </summary>
public readonly struct MenuFlags
{
    /// <summary>
    /// Size of MenuFlags structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw value of the flags.
    /// </summary>
    public ushort Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuFlags"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the MenuFlags.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public MenuFlags(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"MenuFlags requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        Value = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for MenuFlags.");
    }

    /// <summary>
    /// Title reference type (bits 14-15)
    /// </summary>
    public ReferenceType TitleRefType => (ReferenceType)((Value >> 14) & 0x3);

    /// <summary>
    /// Item reference type (bits 12-13)
    /// </summary>
    public ReferenceType ItemRefType => (ReferenceType)((Value >> 12) & 0x3);

    /// <summary>
    /// Always call mChoose routine (bit 8)
    /// </summary>
    public bool AlwaysCallMChoose => (Value & (1 << 8)) != 0;

    /// <summary>
    /// Menu disabled (bit 7)
    /// </summary>
    public bool Disabled => (Value & (1 << 7)) != 0;

    /// <summary>
    /// Use XOR to highlight (bit 5)
    /// </summary>
    public bool UseXorHighlight => (Value & (1 << 5)) != 0;

    /// <summary>
    /// Custom menu (bit 4)
    /// </summary>
    public bool Custom => (Value & (1 << 4)) != 0;

    /// <summary>
    /// Allow menu caching (bit 3)
    /// </summary>
    public bool AllowCache => (Value & (1 << 3)) != 0;
}

