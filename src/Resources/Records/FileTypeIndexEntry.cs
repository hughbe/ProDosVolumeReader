using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// File Type Index Entry
/// </summary>
public readonly struct FileTypeIndexEntry
{
    /// <summary>
    /// Size of a File Type Index Entry in bytes.
    /// </summary>
    public const int Size = 10;

    /// <summary>
    /// Gets the file type that should match for the string to which this index entry points.
    /// </summary>
    public ushort FileType { get; }

    /// <summary>
    /// Gets the auxiliary type that should match for the string to which this index entry points.
    /// </summary>
    public uint AuxType { get; }

    /// <summary>
    /// Gets the file type flags.
    /// </summary>
    public FileTypeFlags Flags { get; } 

    /// <summary>
    /// Gets the offset of the description string within the File Type Record.
    /// </summary>
    public ushort DescriptionOffset { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeIndexEntry"/> struct.
    /// </summary>
    /// <param name="data">The raw index entry data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public FileTypeIndexEntry(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid data length for FileTypeIndexEntry: expected {Size}, got {data.Length}", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20051108132137/https://web.pdx.edu/~heiss/technotes/ftyp/ftn.42.xxxx.html
        int offset = 0;

        // The file type that should match for the string to which this index entry
        // points.
        FileType = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The auxiliary type that should match for the string to which this index
        // entry points.
        AuxType = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // A word, defined bit-wise, indicating the type of match this entry contains
        //  The following definitions apply if the bit in question is set:
        // Bit 15: This record matches this file type and any auxiliary type.
        // This bit would be set, for example, for a record for file type $FF (ProDOS
        // 8 application).
        // Bit 14: This record matches this auxiliary type and any file type.
        // Bit 13: This record is the beginning of a range of file types and auxiliary
        // types to match this string. Any file type and auxiliary type combination
        // falling linearly between this record and the record with the same offset
        // and bit 12 set should be given this string by default if no specific match is
        // found.
        // Bit 12: This record is the end of a range of file types and auxiliary types
        // to match this string. Any file type and auxiliary type combination falling
        // linearly between the record with the same offset and bit 13 set and this
        // record should be given this string by default if no specific match is found.
        // Bits 11-0: Reserved, must be set to zero when creating files.
        // Note: A range uses the file type and auxiliary type combined as a
        // six-byte value, with the file type bytes as most significant. For
        // example, file type $15, auxiliary type $4000 would fall in the range
        // that starts with file type $13, auxiliary type $0800 and ends with file
        // type $17, auxiliary type $2000
        Flags = (FileTypeFlags)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The offset, from the beginning of the file, to the Pascal string matching
        // the description in this index entry. Note that more than one index entry
        // can point to the same string.
        DescriptionOffset = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for FileTypeIndexEntry");
    }
}
