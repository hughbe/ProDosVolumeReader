using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A Tagged String structure in a Resource Fork.
/// </summary>
public readonly struct TaggedString
{
    /// <summary>
    /// Minimum size of TaggedString structure in bytes.
    /// </summary>
    public const int MinSize = 3;

    /// <summary>
    /// Gets the key.
    /// </summary>
    public ushort Key { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaggedString"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the TaggedString.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public TaggedString(ReadOnlySpan<byte> data, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"TaggedString requires at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.076.html
        int offset = 0;

        // Word value of first pair.
        Key = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Pascal string of first pair.
        byte stringLength = data[offset];
        offset += 1;

        if (offset + stringLength > data.Length)
        {
            throw new ArgumentException("TaggedString data is incomplete.", nameof(data));
        }

        Value = Encoding.ASCII.GetString(data.Slice(offset, stringLength));
        offset += stringLength;

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Read beyond end of data.");
    }
}
