using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A text style record used in text records.
/// </summary>
public readonly struct TextStyleRecord
{
    /// <summary>
    /// The minimum size of a TextStyleRecord in bytes.
    /// </summary>
    public const int MinSize = 14;

    /// <summary>
    /// Gets the format of the text style record.
    /// </summary>
    public TEFormat Format { get; }

    /// <summary>
    /// Gets the version number for text style record.
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="ArgumentException">The data length is invalid.</exception>
    public TextStyleRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length must be at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-62 to E-63
        int offset = 0;

        Format = new TEFormat(data[offset..], out var bytesRead);
        offset += bytesRead;

        Debug.Assert(offset <= data.Length, "Did not consume all data.");
    }
}
