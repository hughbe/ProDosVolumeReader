using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing a list of rectangles.
/// </summary>
public readonly struct RectangleListRecord
{
    /// <summary>
    /// Minimum size of RectangleListRecord structure in bytes.
    /// </summary>
    public const int MinSize = 2;

    /// <summary>
    /// Gets the count of rectangles.
    /// </summary>
    public ushort Count { get; }

    /// <summary>
    /// Gets the list of rectangles.
    /// </summary>
    public List<RECT> Rectangles { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleListRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the RectangleListRecord.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public RectangleListRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"RectangleListRecord requires at least {MinSize} bytes.", nameof(data));
        }

        int offset = 0;

        // Number of rectangles in this resource.
        Count = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (offset + (Count * RECT.Size) > data.Length)
        {
            throw new ArgumentException("Insufficient data for all rectangles in RectangleListRecord.", nameof(data));
        }

        // First QuickDraw II rectangle structure.
        // ...
        // Rectangles, eight bytes each.
        // lastRectangle (+002+(8*(count-1))): 8 Bytes
        // Last QuickDraw II rectangle structure.
        var rectangles = new List<RECT>(Count);
        for (int i = 0; i < Count; i++)
        {
            rectangles.Add(new RECT(data.Slice(offset, RECT.Size)));
            offset += RECT.Size;
        }

        Rectangles = rectangles;

        Debug.Assert(offset <= data.Length, "Did not consume all data for RectangleListRecord.");
    }
}
