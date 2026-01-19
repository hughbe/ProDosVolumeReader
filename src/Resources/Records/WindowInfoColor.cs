using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Colors used in the window information area.
/// </summary>
public readonly struct WindowInfoColor
{
    /// <summary>
    /// Size of WindowInfoColor structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw ushort value representing the window info color settings.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowInfoColor"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the WindowInfoColor record.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public WindowInfoColor(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"WindowInfoColor requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for WindowInfoColor record.");
    }
    
    /// <summary>
    /// Gets the alert frame inside outline color.
    /// </summary>
    public byte AlertInsideFrameColorIndex => (byte)((RawValue >> 12) & 0xF);
    
    /// <summary>
    /// Gets the color for info bar.
    /// </summary>
    public byte InfoBarColorIndex => (byte)((RawValue >> 4) & 0xF);
}
