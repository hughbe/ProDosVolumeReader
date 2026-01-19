namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Match Key values.
/// </summary>
public enum BundleDocumentMatchKey : ushort
{
    /// <summary>
    /// No key.
    /// </summary>
    Empty = 0x00,

    /// <summary>
    /// File type key.
    /// </summary>
    MatchFileType = 0x01,

    /// <summary>
    /// Aux type key.
    /// </summary>
    MatchAuxType = 0x02,

    /// <summary>
    /// File name key.
    /// </summary>
    MatchFileName = 0x03,

    /// <summary>
    /// Creation date key.
    /// </summary>
    MatchCreationDate = 0x04,

    /// <summary>
    /// Modification date key.
    /// </summary>
    MatchModificationDate = 0x05,

    /// <summary>
    /// Local access key.
    /// </summary>
    MatchLocalAccess = 0x06,

    /// <summary>
    /// Network access key.
    /// </summary>
    MatchNetworkAccess = 0x07,

    /// <summary>
    /// Extended key.
    /// </summary>
    MatchExtended = 0x08,

    /// <summary>
    /// HFS file type key.
    /// </summary>
    MatchHFSFileType = 0x09,

    /// <summary>
    /// HFS creator key.
    /// </summary>
    MatchHFSCreator = 0x0A,

    /// <summary>
    /// Match option list key.
    /// </summary>
    MatchOptionList = 0x0B,

    /// <summary>
    /// Match EOF key.
    /// </summary>
    MatchEOF = 0x0C
}