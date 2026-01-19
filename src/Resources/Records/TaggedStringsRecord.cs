using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Tagged Strings structure in a Resource Fork.
/// </summary>
public readonly struct TaggedStringsRecord
{
    /// <summary>
    /// Minimum size of TaggedStringsRecord structure in bytes.
    /// </summary>
    public const int MinSize = 2;

    /// <summary>
    /// Gets the number of word/string pairs in this resource.
    /// </summary>
    public ushort Count { get; }

    /// <summary>
    /// Gets the word/string pairs.
    /// </summary>
    public List<TaggedString> Pairs { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaggedStringsRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Tagged Strings.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public TaggedStringsRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"TaggedStringsRecord requires at least {MinSize} bytes.", nameof(data));
        }

        int offset = 0;

        // Number of word/string pairs in this resource.
        Count = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // firstWord (+002): Word
        // Word value of first pair.
        // firstString (+004): String
        // Pascal string of first pair.
        // secondWord (+xxx): Word
        // Word value of second pair.
        // secondString (+yyy): String
        // Pascal string of second pair.
        var pairs = new List<TaggedString>(Count);
        for (int i = 0; i < Count; i++)
        {
            pairs.Add(new TaggedString(data[offset..], out int bytesRead));
            offset += bytesRead;
        }

        Pairs = pairs;

        Debug.Assert(offset <= data.Length, "Did not consume all data for TaggedStringsRecord.");
    }
}
