using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Colors used in the window bar.
/// </summary>
public readonly struct WindowBarColor
{
    /// <summary>
    /// Size of WindowBarColor structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw ushort value representing the window bar color settings.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowBarColor"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the WindowBarColor record.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public WindowBarColor(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"WindowBarColor requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for WindowBarColor record.");
    }

    /// <summary>
    /// Gets the window bar color pattern.
    /// </summary>
    public WindowBarColorPattern Pattern => (WindowBarColorPattern)(RawValue >> 8);
    
    /// <summary>
    /// Gets the pattern color index.
    /// </summary>
    public byte PatternColorIndex => (byte)((RawValue >> 4) & 0xF);
    
    /// <summary>
    /// Gets the background color index.
    /// </summary>
    public byte BackgroundColorIndex => (byte)(RawValue & 0xF);
}
