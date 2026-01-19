using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;
using ProDosVolumeReader.Utilities;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a ProDOS file entry in a directory.
/// </summary>
public readonly struct FileEntry
{
    /// <summary>
    /// The size in bytes of a file entry.
    /// </summary>
    public const int Size = 39;

    /// <summary>
    /// Gets the storage type and name length byte.
    /// </summary>
    public byte StorageTypeAndNameLength { get; }

    /// <summary>
    /// Gets the storage type which specifies the type of file pointed to by this file entry.
    /// </summary>
    public StorageType StorageType => (StorageType)(StorageTypeAndNameLength >> 4);

    /// <summary>
    /// Gets the length of the file's name.
    /// </summary>
    public byte NameLength => (byte)(StorageTypeAndNameLength & 0b0000_1111);

    /// <summary>
    /// Gets the file name bytes.
    /// </summary>
    public ByteArray15 FileNameBytes { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string FileName
    {
        get
        {
            var nameBytes = FileNameBytes.AsSpan()[..NameLength];
            return Encoding.ASCII.GetString(nameBytes);
        }
    }

    /// <summary>
    /// Gets the file type, a descriptor of the internal structure of the file.
    /// </summary>
    public FileType FileType { get; }

    /// <summary>
    /// Gets the block address of the master index block if a tree file,
    /// of the index block if a sapling file, and of the block if a seedling file.
    /// </summary>
    public ushort KeyPointer { get; }

    /// <summary>
    /// Gets the total number of blocks actually used by the file.
    /// </summary>
    public ushort BlocksUsed { get; }

    /// <summary>
    /// Gets the total number of bytes readable from the file (End Of File).
    /// </summary>
    public uint Eof { get; }

    /// <summary>
    /// Gets the date and time at which the file was created.
    /// </summary>
    public ProDosDateTime CreationDate { get; }

    /// <summary>
    /// Gets the version number of ProDOS under which the file was created.
    /// </summary>
    public byte Version { get; }

    /// <summary>
    /// Gets the minimum version number of ProDOS that can access the information in this file.
    /// </summary>
    public byte MinVersion { get; }

    /// <summary>
    /// Gets the access flags that determine whether this file can be read,
    /// written, destroyed, and renamed, and whether the file needs to be backed up.
    /// </summary>
    public FileAccessFlags AccessFlags { get; }

    /// <summary>
    /// Gets the auxiliary type, a general-purpose field in which a system program
    /// can store additional information about the internal format of a file.
    /// </summary>
    public ushort AuxType { get; }

    /// <summary>
    /// Gets the date and time that the last CLOSE operation after a WRITE was performed on this file.
    /// </summary>
    public ProDosDateTime LastModificationDate { get; }

    /// <summary>
    /// Gets the block address of the key block of the directory that owns this file entry.
    /// </summary>
    public ushort HeaderPointer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileEntry"/> struct.
    /// </summary>
    /// <param name="data">The byte data.</param>
    /// <exception cref="ArgumentException">Thrown if the data length is not correct.</exception>
    public FileEntry(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be {Size} bytes in length.", nameof(data));
        }

        int offset = 0;

        // storage_type and name_length (1 byte): Two four-bit fields are packed into
        // this byte. The value in the high-order four bits (the storage_type) specifies
        // the type of file pointed to by this file entry:
        // $1 = Seedling file, $2 = Sapling file, $3 = Tree file, $4 = Pascal area, $D = Subdirectory
        // The low four bits contain the length of the file's name.
        StorageTypeAndNameLength = data[offset];
        offset += 1;

        // file_name (15 bytes): The first name_length bytes of this field contain
        // the file's name. This name must conform to the filename syntax explained
        // in Chapter 2.
        FileNameBytes = new ByteArray15(data.Slice(offset, ByteArray15.Size));
        offset += ByteArray15.Size;

        // file_type (1 byte): A descriptor of the internal structure of the file.
        FileType = (FileType)data[offset];
        offset += 1;

        // key_pointer (2 bytes): The block address of the master index block if a
        // tree file, of the index block if a sapling file, and of the block if a
        // seedling file.
        KeyPointer = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // blocks_used (2 bytes): The total number of blocks actually used by the file.
        BlocksUsed = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // EOF (3 bytes): A three-byte integer, lowest bytes first, that represents
        // the total number of bytes readable from the file.
        Eof = (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16));
        offset += 3;

        // creation (4 bytes): The date and time at which the file pointed to by
        // this entry was created.
        CreationDate = new ProDosDateTime(data.Slice(offset, ProDosDateTime.Size));
        offset += ProDosDateTime.Size;

        // version (1 byte): The version number of ProDOS under which the file
        // pointed to by this entry was created.
        Version = data[offset];
        offset += 1;

        // min_version (1 byte): The minimum version number of ProDOS that can
        // access the information in this file.
        MinVersion = data[offset];
        offset += 1;

        // access (1 byte): Determines whether this file can be read, written,
        // destroyed, and renamed, and whether the file needs to be backed up.
        AccessFlags = (FileAccessFlags)data[offset];
        offset += 1;

        // aux_type (2 bytes): A general-purpose field in which a system program
        // can store additional information about the internal format of a file.
        AuxType = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // last_mod (4 bytes): The date and time that the last CLOSE operation
        // after a WRITE was performed on this file.
        LastModificationDate = new ProDosDateTime(data.Slice(offset, ProDosDateTime.Size));
        offset += ProDosDateTime.Size;

        // header_pointer (2 bytes): This field is the block address of the key
        // block of the directory that owns this file entry.
        HeaderPointer = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all bytes for FileEntry");
    }
}
