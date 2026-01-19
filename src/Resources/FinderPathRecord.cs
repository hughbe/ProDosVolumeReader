using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing a Finder Path.
/// </summary>
public readonly struct FinderPathRecord
{
    /// <summary>
    /// Minimum size of FinderPathRecord structure in bytes.
    /// </summary>
    public const int MinSize = 6;

    /// <summary>
    /// Gets the version of the Finder Path record.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the offset to the path name string.
    /// </summary>
    public ushort PathNameOffset { get; }

    /// <summary>
    /// Gets the number of entries in the path.
    /// </summary>
    public ushort NumberOfEntries { get; }

    /// <summary>
    /// Gets the list of entries in the path.
    /// </summary>
    public List<FinderPathEntry> Entries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinderPathRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the FinderPathRecord.</param>
    /// <exception cref="ArgumentException">Thrown when data is too small.</exception>
    /// <exception cref="NotSupportedException">Thrown when version is unsupported.</exception>
    public FinderPathRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"FinderPathRecord requires at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1301-L1315
        int offset = 0;

        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Version != 0)
        {
            throw new NotSupportedException($"Unsupported FinderPathRecord version: {Version}");
        }

        PathNameOffset = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        NumberOfEntries = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (offset + NumberOfEntries * FinderPathEntry.Size > data.Length)
        {
            throw new ArgumentException("FinderPathRecord data is too small for the number of entries specified.", nameof(data));
        }

        var entries = new List<FinderPathEntry>(NumberOfEntries);
        for (int i = 0; i < NumberOfEntries; i++)
        {
            entries.Add(new FinderPathEntry(data.Slice(offset, FinderPathEntry.Size)));
            offset += FinderPathEntry.Size;
        }

        Entries = entries;

        Debug.Assert(offset <= data.Length, "Did not consume all data for FinderPathRecord.");
    }
}
