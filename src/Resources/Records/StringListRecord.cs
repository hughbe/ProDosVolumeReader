using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing a string list.
/// </summary>
public readonly struct StringListRecord
{
    /// <summary>
    /// The minimum size of StringListRecord structure in bytes.
    /// </summary>
    public const int MinSize = 2;

    /// <summary>
    /// Gets the count of strings in the string list.
    /// </summary>
    public ushort Count { get; }

    /// <summary>
    /// Gets the list of strings.
    /// </summary>
    public List<string> Strings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringListRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the StringListRecord.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public StringListRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-61
        int offset = 0;

        // The number of Pascal strings stored at strings.
        Count = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // An array of count Pascal strings.
        var strings = new List<string>(Count);
        for (int i = 0; i < Count; i++)
        {
            byte stringLength = data[offset];
            offset += 1;

            if (offset + stringLength > data.Length)
            {
                throw new ArgumentException($"Invalid data length for string {i}: {data.Length}", nameof(data));
            }

            string str = Encoding.ASCII.GetString(data.Slice(offset, stringLength));
            offset += stringLength;

            strings.Add(str);
        }

        Strings = strings;

        Debug.Assert(offset <= data.Length, "Did not consume all data for StringListRecord.");
    }
}
