using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing a picture.
/// </summary>
public readonly struct PictureRecord
{
    /// <summary>
    /// The minimum size of a PictureRecord in bytes.
    /// </summary>
    public const int MinSize = 12;
    
    /// <summary>
    /// Gets the picture’s scan line control byte.
    /// </summary>
    public ushort ScanLineControlByte { get; }

    /// <summary>
    /// Gets the picture’s boundary rectangle.
    /// </summary>
    public RECT BoundaryRectangle { get; }

    /// <summary>
    /// Gets the version number for picture.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the opcode data.
    /// </summary>
    public byte[] OpcodeData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PictureRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the PictureRecord.</param>
    public PictureRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length must be at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-58
        int offset = 0;

        // Picture’s scan line control byte (high byte is 0)
        ScanLineControlByte = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Picture’s boundary rectangle
        BoundaryRectangle = new RECT(data.Slice(offset, RECT.Size));
        offset += RECT.Size;

        // Version number for picture
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        OpcodeData = data[offset..].ToArray();
        offset += OpcodeData.Length;

        Debug.Assert(offset <= data.Length, "Did not consume all data.");
    }
}
