using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Class 1 Input String record.
/// </summary>
public readonly struct Class1InputStringRecord
{
    /// <summary>
    /// Minimum size of Class1InputStringRecord structure in bytes.
    /// </summary>
    public const int MinSize = 2;

    /// <summary>
    /// Gets the length of the string.
    /// </summary>
    public ushort Length { get; }

    /// <summary>
    /// Gets the string characters.
    /// </summary>
    public string StringCharacters { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Class1InputStringRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Class1InputStringRecord.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public Class1InputStringRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-43
        int offset = 0;

        // The number of bytes stored at stringCharacters. This is an
        // unsigned integer; valid values lie in the range from 1 to 65,535.
        Length = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Length > data.Length - offset)
        {
            throw new ArgumentException($"Data length {data.Length} is insufficient for string of length {Length}.", nameof(data));
        }

        // Array of length characters.
        StringCharacters = Encoding.ASCII.GetString(data.Slice(offset, Length));
        offset += Length;

        Debug.Assert(offset <= data.Length, "Did not consume all data for Class1InputStringRecord.");
    }
}
