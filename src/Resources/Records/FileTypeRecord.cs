using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// File Type Record
/// </summary>
public readonly struct FileTypeRecord
{
    /// <summary>
    /// Minimum size of a File Type Record in bytes.
    /// </summary>
    public const int MinSize = FileTypeHeader.Size;

    /// <summary>
    /// Gets the File Type Header.
    /// </summary>
    public FileTypeHeader Header { get; }

    /// <summary>
    /// Gets the list of File Type Index Entries.
    /// </summary>
    public List<FileTypeIndexEntry> IndexEntries { get; }

    /// <summary>
    /// Gets the list of descriptions for this file type.
    /// </summary>
    public List<string> Descriptions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw record data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public FileTypeRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length for FileTypeRecord: expected at least {MinSize}, got {data.Length}", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20051108132137/https://web.pdx.edu/~heiss/technotes/ftyp/ftn.42.xxxx.html
        int offset = 0;

        Header = new FileTypeHeader(data.Slice(offset, FileTypeHeader.Size));
        offset += FileTypeHeader.Size;

        Debug.Assert(offset <= data.Length, "Did not consume all data for FileTypeRecord");

        if (Header.FirstIndexEntryOffset < FileTypeHeader.Size || Header.FirstIndexEntryOffset > data.Length)
        {
            throw new ArgumentException($"Invalid FirstIndexEntryOffset in FileTypeHeader: {Header.FirstIndexEntryOffset} exceeds data length {data.Length}", nameof(data));
        }
        if (Header.IndexRecordSize < FileTypeIndexEntry.Size)
        {
            throw new ArgumentException($"Invalid IndexRecordSize in FileTypeHeader: {Header.IndexRecordSize} is less than minimum {FileTypeIndexEntry.Size}", nameof(data));
        }

        offset = Header.FirstIndexEntryOffset;
        if (offset + Header.NumberOfEntries * Header.IndexRecordSize > data.Length)
        {
            throw new ArgumentException($"FileTypeRecord data is too short for the number of index entries: required {offset + Header.NumberOfEntries * Header.IndexRecordSize}, got {data.Length}", nameof(data));
        }
        
        var indexEntries = new List<FileTypeIndexEntry>(Header.NumberOfEntries);
        for (int i = 0; i < Header.NumberOfEntries; i++)
        {
            indexEntries.Add(new FileTypeIndexEntry(data.Slice(offset, Header.IndexRecordSize)));
            offset += Header.IndexRecordSize;
        }

        IndexEntries = indexEntries;

        int lastEntryEnd = offset;
        var descriptions = new List<string>(Header.NumberOfEntries);
        foreach (var entry in IndexEntries)
        {
            if (entry.DescriptionOffset < lastEntryEnd || entry.DescriptionOffset >= data.Length)
            {
                throw new ArgumentException($"Invalid DescriptionOffset in FileTypeIndexEntry: {entry.DescriptionOffset} exceeds data length {data.Length}", nameof(data));
            }

            offset = entry.DescriptionOffset;
            var descLength = data[offset];
            offset++;

            if (offset + descLength > data.Length)
            {
                throw new ArgumentException($"FileTypeRecord data is too short for description string: required {offset + descLength}, got {data.Length}", nameof(data));
            }

            descriptions.Add(Encoding.ASCII.GetString(data.Slice(offset, descLength)));
            offset += descLength;
        }

        Descriptions = descriptions;

        Debug.Assert(offset <= data.Length, "Did not consume all data for FileTypeRecord");
    }
}
