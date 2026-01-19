using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Alert String Record
/// </summary>
public readonly struct AlertStringRecord
{
    /// <summary>
    /// Gets the alert message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertStringRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Alert String record.</param>
    public AlertStringRecord(ReadOnlySpan<byte> data)
    {
        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-3
        int offset = 0;

        // The alert message to be displayed. Contents of this string must
        // comply with the rules for alert window definitions documented in
        // Chapter 52, “Window Manager Update,”earlier in this book.
        Message = Encoding.ASCII.GetString(data[offset..]);
        offset += data.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for AlertStringRecord.");
    }
}
