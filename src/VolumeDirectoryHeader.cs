using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;
using ProDosVolumeReader.Utilities;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a ProDOS volume directory header.
/// </summary>
public readonly struct VolumeDirectoryHeader
{
    /// <summary>
    /// The size in bytes of the Volume Directory Header.
    /// </summary>
    public const int Size = 39;

    /// <summary>
    /// Gets the storage type and name length.
    /// </summary>
    public byte StorageTypeAndNameLength { get; }

    /// <summary>
    /// Gets the storage type.
    /// </summary>
    public StorageType StorageType => (StorageType)(StorageTypeAndNameLength >> 4);

    /// <summary>
    /// Gets the name length.
    /// </summary>
    public byte NameLength => (byte)(StorageTypeAndNameLength & 0b0000_1111);

    /// <summary>
    /// Gets the file name bytes.
    /// </summary>
    public String15 FileNameBytes { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string FileName
    {
        get
        {
            if ((LowercaseFlags & 0x8000) != 0)
            {
                // GS/OS volume - apply lowercase flags as per GS/OS tech note #8
                return string.Create(NameLength, (FileNameBytes, LowercaseFlags, NameLength), static (chars, state) =>
                {
                    ReadOnlySpan<byte> nameSpan = state.FileNameBytes.AsSpan()[..state.NameLength];
                    for (int i = 0; i < state.NameLength; i++)
                    {
                        char c = (char)nameSpan[i];
                        if (c >= 'A' && c <= 'Z' && (state.LowercaseFlags & (1 << i)) != 0)
                        {
                            c = (char)(c + 0x20); // Convert to lowercase
                        }

                        chars[i] = c;
                    }
                });
            }

            ReadOnlySpan<byte> nameSpan = FileNameBytes.AsSpan()[..NameLength];
            return Encoding.ASCII.GetString(nameSpan);
        }
    }

    /// <summary>
    /// Gets the reserved bytes.
    /// </summary>
    public ushort Reserved { get; }

    /// <summary>
    /// Gets the last modification date (undocumented).
    /// </summary>
    public ProDosDateTime LastModificationDate { get; }

    /// <summary>
    /// Gets the lowercase flags (GS/OS only).
    /// </summary>
    public ushort LowercaseFlags { get; }

    /// <summary>
    /// Gets the creation date.
    /// </summary>
    public ProDosDateTime CreationDate { get; }

    /// <summary>
    /// Gets the minimum version.
    /// </summary>
    public byte Version { get; }

    /// <summary>
    /// Gets the minimum version.
    /// </summary>
    public byte MinVersion { get; }

    /// <summary>
    /// Gets the access flags that determine whether this volume directory can be read,
    /// written, destroyed, and renamed.
    /// </summary>
    public VolumeAccessFlags AccessFlags { get; }

    /// <summary>
    /// Gets the length in bytes of each entry in this directory.
    /// </summary>
    public byte EntryLength { get; }

    /// <summary>
    /// Gets the number entries per block.
    /// </summary>
    public byte EntriesPerBlock { get; }

    /// <summary>
    /// Gets the file count.
    /// </summary>
    public ushort FileCount { get; }

    /// <summary>
    /// Gets the block address of the first block of the volume's bit map.
    /// </summary>
    public ushort BitMapPointer { get; }

    /// <summary>
    /// Gets the total number of blocks on the volume.
    /// </summary>
    public ushort TotalBlocks { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeDirectoryHeader"/> struct.
    /// </summary>
    /// <param name="data">The byte data.</param>
    /// <exception cref="ArgumentException">Thrown if the data length is not correct.</exception>
    public VolumeDirectoryHeader(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be {Size} bytes in length.", nameof(data));
        }

        int offset = 0;

        // storage_type and name_length (1 byte): Two four-bit fields are packed into
        // this byte. A value of $F in the high four bits (the storage_type) identifies
        // the current block as the key block of a volume directory file. The low
        // four bits contain the length of the volume's name (see the file_name field,
        // below). The name_length can be changed by a RENAME call.
        StorageTypeAndNameLength = data[offset];
        offset += 1;

        if (StorageType != StorageType.VolumeDirectoryHeader)
        {
            throw new ArgumentException("Data does not represent a valid Volume Directory Header.", nameof(data));
        }

        // file_name (15 bytes): The first n bytes of this field, where n is specified
        // by name_length, contain the volume's name. This name must conform to the
        // filename (volume name) syntax explained in Chapter 2. The name does not
        // begin with the slash that usually precedes volume names. This field can
        // be changed by the RENAME call.
        FileNameBytes = new String15(data.Slice(offset, String15.Size));
        offset += String15.Size;

        // reserved (8 bytes): Reserved for future expansion of the file system.
        // However, this has been documented and reverse engineered, e.g.
        // https://ciderpress2.com/formatdoc/ProDOS-notes.html

        // (reserved, should be zeroes)
        Reserved = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // (undocumented? GS/OS feature) modification date/time
        LastModificationDate = new ProDosDateTime(data.Slice(offset, ProDosDateTime.Size));
        offset += ProDosDateTime.Size;

        // lower-case flags (see TN.GSOS.008)
        LowercaseFlags = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // creation (4 bytes): The date and time at which this volume was initialized.
        // The format of these bytes is described in Section B.4.2.2.
        CreationDate = new ProDosDateTime(data.Slice(offset, ProDosDateTime.Size));
        offset += ProDosDateTime.Size;

        // version (1 byte): The version number of ProDOS under which this volume
        // was initialized. This byte allows newer versions of ProDOS to determine
        // the format of the volume, and adjust their directory interpretation to
        // conform to older volume formats. In ProDOS 1.0, version = 0.
        Version = data[offset];
        offset += 1;

        // min_version: Reserved for future use. In ProDOS 1.0, it is 0.
        MinVersion = data[offset];
        offset += 1;

        // access (1 byte): Determines whether this volume directory can be read
        // written, destroyed, and renamed. The format of this field is described
        // in Section B.4.2.3.
        AccessFlags = (VolumeAccessFlags)data[offset];
        offset += 1;

        // entry_length (1 byte): The length in bytes of each entry in this directory.
        // The volume directory header itself is of this length. entry_length = $27.
        EntryLength = data[offset];
        offset += 1;

        // entries_per_block (1 byte): The number of entries that are stored in
        // each block of the directory file. entries_per_block = $0D.
        EntriesPerBlock = data[offset];
        offset += 1;

        // file_count (2 bytes): The number of active file entries in this directory
        // file. An active file is one whose storage_type is not 0. See Section B.2.4
        // for a description of file entries.
        FileCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // bit_map_pointer (2 bytes): The block address of the first block of the
        // volume's bit map. The bit map occupies consecutive blocks, one for every
        // 4,096 blocks (or fraction thereof) on the volume. You can calculate the
        // number of blocks in the bit map using the total_blocks field, described
        // below.
        // The bit map has one bit for each block on the volume: a value of 1 means
        // the block is free; 0 means it is in use. If the number of blocks used by
        // all files on the volume is not the same as the number recorded in the bit
        // map, the directory structure of the volume has been damaged.
        BitMapPointer = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // total_blocks (2 bytes): The total number of blocks on the volume.
        TotalBlocks = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all bytes for VolumeDirectoryHeader");
    }
}
