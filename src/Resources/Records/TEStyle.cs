using System.Buffers.Binary;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A text style structure used in text records.
/// </summary>
public readonly struct TEStyle
{
    /// <summary>
    /// The size of a TEStyle in bytes.
    /// </summary>
    public const int Size = 12;

    /// <summary>
    /// Gets the ID of the font used by this style.
    /// </summary>
    public uint FontID { get; }

    /// <summary>
    /// Gets the foreground color for the text.
    /// </summary>
    public ushort ForegroundColor { get; }

    /// <summary>
    /// Gets the background color for the text.
    /// </summary>
    public ushort BackgroundColor { get; }

    /// <summary>
    /// Gets the application-specific data.
    /// </summary>
    public uint UserData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TEStyle"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the TEStyle.</param>
    /// <exception cref="ArgumentException">Thrown if the data length is invalid.</exception>
    public TEStyle(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data length must be exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // 49-41
        int offset = 0;

        // Font Manager font ID record identifying the font of the text. See
        // Chapter 8, “Font Manager,” in Volume 1 of the Toolbox Reference for
        // more information about font IDs.
        FontID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Foreground color for the text. Note that all bits in TextEdit color
        // words are significant. TextEdit generates QuickDraw II color patterns
        // by replicating a color word the appropriate number of times for the
        // current resolution (8 times for 640 mode, 16 times for 320 mode). See
        // Chapter 16, “QuickDraw II,” in Volume 2 of the Toolbox Reference for
        // more information on QuickDrawII patterns and dithered colors.
        ForegroundColor = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Background color for the text.
        BackgroundColor = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Application-specific data.
        UserData = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;
    }
}
