namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Types for references in menu records.
/// </summary>
public enum ReferenceType : byte
{
    /// <summary>
    /// Pointer to data in the resource fork.
    /// </summary>
    Ptr = 0x00,

    /// <summary>
    /// Handle to data in the resource fork.
    /// </summary>
    Handle = 0x01,

    /// <summary>
    /// Resource ID of data in the resource fork.
    /// </summary>
    Resource = 0x02,

    /// <summary>
    /// Invalid reference.
    /// </summary>
    Invalid = 0x03
}
