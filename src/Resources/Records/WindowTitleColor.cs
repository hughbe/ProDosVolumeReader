using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Colors used in the window title bar.
/// </summary>
public readonly struct WindowTitleColor
{
    /// <summary>
    /// Size of WindowTitleColor structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw ushort value representing the window title color settings.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowTitleColor"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the WindowTitleColor record.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public WindowTitleColor(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"WindowTitleColor requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for WindowTitleColor record.");
    }

    /// <summary>
    /// Gets the inactive title bar color.
    /// </summary>
    public byte InactiveTitleBarColorIndex => (byte)((RawValue >> 8) & 0xF);

    /// <summary>
    /// Gets the inactive title color.
    /// </summary>
    public byte InactiveTitleColorIndex => (byte)((RawValue >> 4) & 0xF);
    
    /// <summary>
    /// Gets the active title color.
    /// </summary>
    public byte ActiveTitleColorIndex => (byte)(RawValue & 0xF);
}
