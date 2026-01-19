using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Colors for window grow box and alert frame middle outline.
/// </summary>
public readonly struct WindowGrowColor
{
    /// <summary>
    /// Size of WindowGrowColor structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Raw ushort value representing the window grow color settings.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowGrowColor"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the WindowGrowColor record.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public WindowGrowColor(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"WindowGrowColor requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for WindowGrowColor record.");
    }

    /// <summary>
    /// Gets the alert frame mid outline color.
    /// </summary>
    public byte AlertMidFrameColorIndex => (byte)((RawValue >> 12) & 0xF);
    
    /// <summary>
    /// Gets the color for unselected size box.
    /// </summary>
    public byte SizeUnselectedColorIndex => (byte)((RawValue >> 4) & 0xF);
    
    /// <summary>
    /// Gets the color for selected size box.
    /// </summary>
    public byte SizeSelectedColorIndex => (byte)(RawValue & 0xF);
}
