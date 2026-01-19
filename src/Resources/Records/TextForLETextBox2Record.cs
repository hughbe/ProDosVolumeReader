using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// The Text Box 2 record structure in a GS/OS Resource Fork.
/// </summary>
public readonly struct TextForLETextBox2Record
{
    /// <summary>
    /// The minimum size of Text Box 2 record structure in bytes.
    /// </summary>
    public const int MinSize = 2;

    /// <summary>
    /// Gets the length of the string in characters.
    /// </summary>
    public ushort Length { get; }

    /// <summary>
    /// Gets the string characters.
    /// </summary>
    public byte[] StringCharacters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextForLETextBox2Record"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Text Box 2 record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public TextForLETextBox2Record(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-68
        // But see https://mirrors.apple2.org.za/Apple%20II%20Documentation%20Project/Computers/Apple%20II/Apple%20IIGS/Documentation/Apple%20IIGS%20Technical%20Notes%2011-25.pdf
        // Appendix E
        // Page E-68 of Volume 3 shows a length field at the beginning of an rTextForLETextBox2
        // resource. This field is not actually present. The length is simply the size of the resource—it is
        // not stored redundantly.
        int offset = 0;

        // Array of up to 32767 characters. Formatting information is embedded
        // in the character array, and is included in the value of length. See
        // Chapter 10, "LineEdit Tool Set," in the Toolbox Reference for complete
        // information on the syntax for this embedded appearance
        // information.
        StringCharacters = data[offset..].ToArray();
        offset += StringCharacters.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for Text Box 2 record.");
    }
}
