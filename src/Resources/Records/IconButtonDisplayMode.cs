using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents the display mode for an icon button, as passed to the DrawIcon routine.
/// </summary>
public readonly struct IconButtonDisplayMode
{
    /// <summary>
    /// The size of the display mode field in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// The raw value of the display mode field.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Gets the background color index (bits 12-15).
    /// </summary>
    public byte BackgroundColor => (byte)((RawValue >> 12) & 0xF);

    /// <summary>
    /// Gets the foreground color index (bits 8-11).
    /// </summary>
    public byte ForegroundColor => (byte)((RawValue >> 8) & 0xF);

    /// <summary>
    /// Gets whether to AND light-gray pattern to image (bit 2).
    /// </summary>
    public bool AndLightGrayPattern => (RawValue & (1 << 2)) != 0;

    /// <summary>
    /// Gets whether to copy light-gray pattern instead of image (bit 1).
    /// </summary>
    public bool OpenIcon => (RawValue & (1 << 1)) != 0;

    /// <summary>
    /// Gets whether to invert image before copying (bit 0).
    /// </summary>
    public bool SelectedIcon => (RawValue & 1) != 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="IconButtonDisplayMode"/> struct from a byte span.
    /// </summary>
    /// <param name="data">A span containing at least 2 bytes representing the display mode.</param>
    public IconButtonDisplayMode(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for IconButtonDisplayMode.");
    }
}