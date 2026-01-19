namespace ProDosVolumeReader;

/// <summary>
/// Defines the storage type of a ProDOS file.
/// </summary>
public enum StorageType : byte
{
    /// <summary>
    /// Invalid storage type.
    /// </summary>
    Invalid = 0x00,

    /// <summary>
    /// Seedling file.
    /// </summary>
    Seedling = 0x01,

    /// <summary>
    /// Sapling file.
    /// </summary>
    Sapling = 0x02,

    /// <summary>
    /// Tree file.
    /// </summary>
    Tree = 0x03,

    /// <summary>
    /// Pascal area on ProFile hard disk drive (described in ProDOS 8 TN #25).
    /// </summary>
    PascalArea = 0x04,

    /// <summary>
    /// GS/OS forked file.
    /// </summary>
    GSOSForkedFile = 0x05,

    /// <summary>
    /// Subdirectory.
    /// </summary>
    Subdirectory = 0xD,

    /// <summary>
    /// Subdirectory header.
    /// </summary>
    SubdirectoryHeader = 0xE,

    /// <summary>
    /// Volume directory header.
    /// </summary>
    VolumeDirectoryHeader = 0x0F
}
