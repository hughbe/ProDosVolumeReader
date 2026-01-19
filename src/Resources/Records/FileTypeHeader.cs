using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// File Type Header
/// </summary>
public readonly struct FileTypeHeader
{
    /// <summary>
    /// Size of the File Type Header structure in bytes.
    /// </summary>
    public const int Size = 12;

    /// <summary>
    /// Gets the version number.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the major version number.
    /// </summary>
    public int MajorVersion => (Version >> 8) & 0x0F;

    /// <summary>
    /// Gets the minor version number.
    /// </summary>
    public int MinorVersion => Version & 0xFF;

    /// <summary>
    /// Gets the file type flags.
    /// </summary>
    public FileTypeFlags Flags { get; }

    /// <summary>
    /// Gets the number of entries in the file.
    /// </summary>
    public ushort NumberOfEntries { get; }

    /// <summary>
    /// Gets the reserved field.
    /// </summary>
    public ushort Reserved { get; }

    /// <summary>
    /// Gets the index record size.
    /// </summary>
    public ushort IndexRecordSize { get; }

    /// <summary>
    /// Gets the offset to the first index entry.
    /// </summary>
    public ushort FirstIndexEntryOffset { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeHeader"/> struct.
    /// </summary>
    /// <param name="data">The raw record data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public FileTypeHeader(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid data length for FileTypeHeader: expected {Size}, got {data.Length}", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20051108132137/https://web.pdx.edu/~heiss/technotes/ftyp/ftn.42.xxxx.html
        int offset = 0;

        // Version number. This is toolbox style, so revision x.y would be stored as
        // $0xyy. x is the major revision number; when this value changes, it means
        // that previously-written code will not be able to read this file. yy is
        // the minor version number; when it changes, there are new fields but the
        // old ones are in the same order. This Note describes version 1.0 of the
        // File Type Descriptor format.
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (MajorVersion != 1)
        {
            throw new ArgumentException($"Unsupported FileTypeHeader major version: {MajorVersion}", nameof(data));
        }

        // This word is all the flags words from all the records combined using a
        // binary OR instruction. The flags word for each entry indicates the type
        // of entry it contains (see the section "The Index Entries"). A particular
        // bit in this word will be set if there exists a record in the file where
        // the corresponding bit in the flags word is set. For example, bit 14 will
        // be clear in this word if no index entry has bit 14 set.
        Flags = (FileTypeFlags)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The number of entries in this file.
        NumberOfEntries = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Reserved for the application's use. The Finder calculates a value for
        // each file and stores it in this field when the file is read into memory.
        // This should be zero in the file on disk.
        Reserved = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The number of bytes in each index record.
        IndexRecordSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Offset, from the beginning of the file, to the first index entry.
        // This field is provided so that other fields may be added to the header
        // at a later date without breaking existing programs.
        FirstIndexEntryOffset = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for FileTypeHeader");
    }
}
