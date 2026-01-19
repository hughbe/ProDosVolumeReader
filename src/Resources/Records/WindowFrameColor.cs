using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Colors used in the window frame.
/// </summary>
public readonly struct WindowFrameColor
{
    /// <summary>
    /// Size of WindowFrameColor structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw ushort value representing the window frame color settings.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFrameColor"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the WindowFrameColor record.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public WindowFrameColor(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"WindowFrameColor requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for WindowFrameColor record.");
    }

    /// <summary>
    /// Gets the frame color index.
    /// </summary>
    public byte FrameColorIndex => (byte)((RawValue >> 4) & 0xF);
}
