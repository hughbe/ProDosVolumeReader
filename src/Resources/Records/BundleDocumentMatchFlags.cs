namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Match flags for BundleDocument structure.
/// </summary>
[Flags]
public enum BundleDocumentMatchFlags : uint
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// FileType match flag.
    /// </summary>
    FileType = 0x1,

    /// <summary>
    /// AuxType match flag.
    /// </summary>
    AuxType = 0x2,

    /// <summary>
    /// FileName match flag.
    /// </summary>
    FileName = 0x4,

    /// <summary>
    /// Create date/time match flag.
    /// </summary>
    CreateDateTime = 0x8,

    /// <summary>
    /// Modification date/time match flag.
    /// </summary>
    ModDateTime = 0x10,

    /// <summary>
    /// Local/Network access match flag.
    /// </summary>
    LocalAccess = 0x20,

    /// <summary>
    /// Network access match flag.
    /// </summary>
    NetworkAccess = 0x40,

    /// <summary>
    /// 'Where' match flag.
    /// </summary>
    Where = 0x80,

    /// <summary>
    /// HFS File Type match flag.
    /// </summary>
    HFSFileType = 0x100,

    /// <summary>
    /// HFS Creator match flag.
    /// </summary>
    HFSCreator = 0x200,

    /// <summary>
    /// Option list match flag.
    /// </summary>
    OptionList = 0x400,

    /// <summary>
    /// End of flags marker.
    /// </summary>
    EOF = 0x800,
}
