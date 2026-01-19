using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing text.
/// </summary>
public readonly struct TextRecord
{
    /// <summary>
    /// Gets the text content.
    /// </summary>
    public string StringCharacters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the TextRecord.</param>
    public TextRecord(ReadOnlySpan<byte> data)
    {
        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-66
        int offset = 0;

        // Array of up to 65,535 characters. Any length information is contained
        // in a separately maintained field.
        StringCharacters = Encoding.ASCII.GetString(data.Slice(offset, data.Length));
        offset += data.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for TextRecord.");
    }
}
