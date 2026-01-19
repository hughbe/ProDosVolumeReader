using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Class 1 Output String record.
/// </summary>
public readonly struct Class1OutputStringRecord
{
    /// <summary>
    /// Minimum size of Class1OutputStringRecord structure in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the number of bytes in the whole structure, including bufferSize, length, and stringCharacters.
    /// </summary>
    public ushort BufferSize { get; }

    /// <summary>
    /// Gets the length of the string.
    /// </summary>
    public ushort StringLength { get; }

    /// <summary>
    /// Gets the string characters.
    /// </summary>
    public string StringCharacters { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Class1OutputStringRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Class1OutputStringRecord.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public Class1OutputStringRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-5
        int offset = 0;

        // The numberof bytes in the entire structure, including bufferSize.
        BufferSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (BufferSize < 4 || BufferSize > data.Length)
        {
            throw new ArgumentException($"Data length {data.Length} is insufficient for buffer size {BufferSize}.", nameof(data));
        }

        // The numberofbytes stored at stringCharacters. This is an
        // unsigned integer; valid values lie in the range from 1 to 65,535. If the
        // returned string does not fit in the buffer, this field indicates the length
        // of the string the system wants to return. Your program should add 4 to
        // that value (to account for bufferSize and stringLength), resize
        // the buffer, and reissue the call.
        StringLength = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (StringLength > data.Length - offset)
        {
            throw new ArgumentException($"Data length {data.Length} is insufficient for string of length {StringLength}.", nameof(data));
        }

        // Array of stringLength characters.
        StringCharacters = Encoding.ASCII.GetString(data.Slice(offset, StringLength));
        offset += StringLength;

        Debug.Assert(offset <= data.Length, "Did not consume all data for Class1OutputStringRecord.");
    }
}
