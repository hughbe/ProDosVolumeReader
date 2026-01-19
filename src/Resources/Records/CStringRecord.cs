using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing a C-style string.
/// </summary>
public readonly struct CStringRecord
{
    /// <summary>
    /// Minimum size of CStringRecord structure in bytes (at least 1 byte for null terminator).
    /// </summary>
    public const int MinSize = 1;

    /// <summary>
    /// Gets the string characters.
    /// </summary>
    public string StringCharacters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CStringRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the CStringRecord.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public CStringRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"CStringRecord requires at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-46
        int offset = 0;

        // Array of characters; last character must be a null terminator ($00). The
        // string may contain up to 65,535 characters, including the null
        // terminator.
        var length = data.IndexOf((byte)0);
        if (length == -1)
        {
            throw new ArgumentException("CStringRecord is missing null terminator.", nameof(data));
        }

        StringCharacters = Encoding.ASCII.GetString(data.Slice(offset, length));
        offset += length + 1; // +1 for null terminator

        Debug.Assert(offset <= data.Length, "Did not consume all data for CStringRecord.");
    }
}
