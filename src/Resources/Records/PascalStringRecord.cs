using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A Pascal string record in a GS/OS Resource Fork.
/// </summary>
public readonly struct PascalStringRecord
{
    /// <summary>
    /// Minimum size of a Pascal string record in bytes.
    /// </summary>
    public const int MinSize = 1;

    /// <summary>
    /// Gets the length of the Pascal string.
    /// </summary>
    public byte Length { get; }

    /// <summary>
    /// Gets the value of the Pascal string.
    /// </summary>
    public string StringCharacters { get; }

    /// <summary>
    /// Initalizes a new instance of the <see cref="PascalStringRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Pascal string.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public PascalStringRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-59
        int offset = 0;

        // Number of bytes of data stored in stringCharacters array.
        Length = data[offset];
        offset += 1;

        if (offset + Length > data.Length)
        {
            throw new ArgumentException($"Invalid data length for Pascal string: {data.Length}", nameof(data));
        }

        // Array of lengthByte characters.
        StringCharacters = Encoding.ASCII.GetString(data.Slice(offset, Length));
        offset += Length;

        Debug.Assert(offset <= data.Length, "Read beyond end of data.");
    }
}

