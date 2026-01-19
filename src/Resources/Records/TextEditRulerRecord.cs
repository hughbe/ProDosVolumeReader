using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A Text Edit Ruler Record.
/// </summary>
public readonly struct TextEditRulerRecord
{
    /// <summary>
    /// The minimum size of TextEditRulerRecord structure in bytes.
    /// </summary>
    public const int MinSize = TERuler.MinSize;

    /// <summary>
    /// Gets the text ruler.
    /// </summary>
    public TERuler Ruler { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextEditRulerRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the TextEditRulerRecord.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public TextEditRulerRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-64 to E-65
        int offset = 0;

        Ruler = new TERuler(data[offset..], out int bytesRead);
        offset += bytesRead;

        Debug.Assert(offset <= data.Length, "Read beyond end of data.");
    }
}
