using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A cursor record in a GS/OS Resource Fork.
/// </summary>
public readonly struct CursorRecord
{
    /// <summary>
    /// The minimum size of Cursor record structure in bytes.
    /// </summary>
    public const int MinSize = 18;

    /// <summary>
    /// Gets the height of the cursor, in pixels.
    /// </summary>
    public ushort Height { get; }

    /// <summary>
    /// Gets the width of the cursor, in Words.
    /// </summary>
    public ushort Width { get; }

    /// <summary>
    /// Gets the image of the cursor.
    /// </summary>
    public byte[] ImageData { get; }

    /// <summary>
    /// Gets the mask of the cursor.
    /// </summary>
    public byte[] MaskData { get; }

    /// <summary>
    /// Gets the cursor's Y "hot spot."
    /// </summary>
    public short HotSpotY { get; }

    /// <summary>
    /// Gets the cursor's X "hot spot."
    /// </summary>
    public short HotSpotX { get; }

    /// <summary>
    /// Gets the cursor flags.
    /// </summary>
    public CursorFlags Flags { get; }

    /// <summary>
    /// Gets the reserved1 field.
    /// </summary>
    public uint Reserved1 { get; }

    /// <summary>
    /// Gets the reserved2 field.
    /// </summary>
    public uint Reserved2 { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CursorRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Cursor record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public CursorRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.076.html
        int offset = 0;

        // The height of the cursor, in pixels.
        Height = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The width of the cursor, in Words.
        Width = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The image of the cursor. There are height*width Words in the cursor, or twice that many Bytes.
        int imageDataLength = Height * Width * 2;
        ImageData = data.Slice(offset, imageDataLength).ToArray();
        offset += imageDataLength;

        // The mask of the cursor. This is the same size as the image.
        MaskData = data.Slice(offset, imageDataLength).ToArray();
        offset += imageDataLength;

        // The cursor's Y "hot spot."
        HotSpotY = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Word The cursor's X "hot spot."
        HotSpotX = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Cursor flags:
        // Bit 7: 1 = 640 Mode, 0 = 320 Mode
        // All other bits are reserved and must be zero.
        Flags = (CursorFlags)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Reserved1 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Reserved2 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset <= data.Length, "Did not consume all data for CursorRecord");
    }
}
