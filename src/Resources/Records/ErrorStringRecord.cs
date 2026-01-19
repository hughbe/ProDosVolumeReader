using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Error String Record
/// </summary>
public readonly struct ErrorStringRecord
{
    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorStringRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Error String record.</param>
    public ErrorStringRecord(ReadOnlySpan<byte> data)
    {
        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-47
        // The actual contents of the error string record is a C-style string.
        int offset = 0;

        var length = data.IndexOf((byte)0);
        if (length == -1)
        {
            throw new ArgumentException("ErrorStringRecord is missing null terminator.", nameof(data));
        }

        Message = Encoding.ASCII.GetString(data.Slice(offset, length));
        offset += length + 1; // +1 for null terminator

        Debug.Assert(offset <= data.Length, "Did not consume all data for ErrorStringRecord.");
    }
}
