using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;
using ProDosVolumeReader.Utilities;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a ProDOS subdirectory header.
/// </summary>
public readonly struct SubdirectoryHeader
{
    /// <summary>
    /// The size in bytes of the subdirectory header.
    /// </summary>
    public const int Size = 39;

    /// <summary>
    /// Gets the storage type and name length byte.
    /// </summary>
    public byte StorageTypeAndNameLength { get; }

    /// <summary>
    /// Gets the storage type. A value of $E identifies the current block as the key block
    /// of a subdirectory file.
    /// </summary>
    public StorageType StorageType => (StorageType)(StorageTypeAndNameLength >> 4);

    /// <summary>
    /// Gets the length of the subdirectory's name.
    /// </summary>
    public byte NameLength => (byte)(StorageTypeAndNameLength & 0b0000_1111);

    /// <summary>
    /// Gets the file name bytes.
    /// </summary>
    public String15 FileName { get; }

    /// <summary>
    /// Gets the reserved bytes.
    /// </summary>
    public ByteArray8 Reserved { get; }

    /// <summary>
    /// Gets the date and time at which this subdirectory was created.
    /// </summary>
    public ProDosDateTime CreationDate { get; }

    /// <summary>
    /// Gets the version number of ProDOS under which this subdirectory was created.
    /// </summary>
    public byte Version { get; }

    /// <summary>
    /// Gets the minimum version number of ProDOS that can access the information in this subdirectory.
    /// </summary>
    public byte MinVersion { get; }

    /// <summary>
    /// Gets the access flags that determine whether this subdirectory can be read,
    /// written, destroyed, and renamed, and whether the file needs to be backed up.
    /// </summary>
    public FileAccessFlags AccessFlags { get; }

    /// <summary>
    /// Gets the length in bytes of each entry in this subdirectory.
    /// </summary>
    public byte EntryLength { get; }

    /// <summary>
    /// Gets the number of entries that are stored in each block of the directory file.
    /// </summary>
    public byte EntriesPerBlock { get; }

    /// <summary>
    /// Gets the number of active file entries in this subdirectory file.
    /// </summary>
    public ushort FileCount { get; }

    /// <summary>
    /// Gets the block address of the directory file block that contains the entry for this subdirectory.
    /// </summary>
    public ushort ParentPointer { get; }

    /// <summary>
    /// Gets the entry number for this subdirectory within the block indicated by parent_pointer.
    /// </summary>
    public byte ParentEntryNumber { get; }

    /// <summary>
    /// Gets the entry_length for the directory that owns this subdirectory file.
    /// </summary>
    public byte ParentEntryLength { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubdirectoryHeader"/> struct.
    /// </summary>
    /// <param name="data">The byte data.</param>
    /// <exception cref="ArgumentException">Thrown if the data length is not correct.</exception>
    public SubdirectoryHeader(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be {Size} bytes in length.", nameof(data));
        }

        int offset = 0;

        // storage_type and name_length (1 byte): Two four-bit fields are packed into
        // this byte. A value of $E in the high four bits (the storage_type) identifies
        // the current block as the key block of a subdirectory file. The low four bits
        // contain the length of the subdirectory's name.
        StorageTypeAndNameLength = data[offset];
        offset += 1;

        if (StorageType != StorageType.SubdirectoryHeader)
        {
            throw new ArgumentException($"Invalid storage type for SubdirectoryHeader: {StorageType}.", nameof(data));
        }

        // file_name (15 bytes): The first name_length bytes of this field contain the
        // subdirectory's name. This name must conform to the filename syntax explained
        // in Chapter 2.
        FileName = new String15(data.Slice(offset, String15.Size));
        offset += String15.Size;

        // reserved (8 bytes): Reserved for future expansion of the file system.
        Reserved = new ByteArray8(data.Slice(offset, ByteArray8.Size));
        offset += ByteArray8.Size;

        // creation (4 bytes): The date and time at which this subdirectory was created.
        CreationDate = new ProDosDateTime(data.Slice(offset, ProDosDateTime.Size));
        offset += ProDosDateTime.Size;

        // version (1 byte): The version number of ProDOS under which this subdirectory
        // was created.
        Version = data[offset];
        offset += 1;

        // min_version (1 byte): The minimum version number of ProDOS that can access
        // the information in this subdirectory.
        MinVersion = data[offset];
        offset += 1;

        // access (1 byte): Determines whether this subdirectory can be read, written,
        // destroyed, and renamed, and whether the file needs to be backed up.
        AccessFlags = (FileAccessFlags)data[offset];
        offset += 1;

        // entry_length (1 byte): The length in bytes of each entry in this subdirectory.
        EntryLength = data[offset];
        offset += 1;

        // entries_per_block (1 byte): The number of entries that are stored in each
        // block of the directory file.
        EntriesPerBlock = data[offset];
        offset += 1;

        // file_count (2 bytes): The number of active file entries in this subdirectory file.
        FileCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // parent_pointer (2 bytes): The block address of the directory file block that
        // contains the entry for this subdirectory.
        ParentPointer = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // parent_entry_number (1 byte): The entry number for this subdirectory within
        // the block indicated by parent_pointer.
        ParentEntryNumber = data[offset];
        offset += 1;

        // parent_entry_length (1 byte): The entry_length for the directory that owns
        // this subdirectory file.
        ParentEntryLength = data[offset];
        offset += 1;

        Debug.Assert(offset == data.Length, "Did not consume all bytes for SubdirectoryHeader");
    }
}
