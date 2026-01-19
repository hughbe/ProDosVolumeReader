namespace ProDosVolumeReader.Resources;

/// <summary>
/// Resource attributes flags for a GS/OS resource.
/// </summary>
[Flags]
public enum GsOsResourceAttributes : ushort
{
    /// <summary>
    /// No attributes set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Memory must be page-aligned.
    /// </summary>
    Page = 1 << 2,

    /// <summary>
    /// May not use special memory.
    /// </summary>
    NoSpec = 1 << 3,

    /// <summary>
    /// May not cross bank boundary.
    /// </summary>
    NoCross = 1 << 4,

    /// <summary>
    /// Resource has been altered.
    /// </summary>
    Changed = 1 << 5,

    /// <summary>
    /// Preload resource at OpenResourceFile time.
    /// </summary>
    PreLoad = 1 << 6,

    /// <summary>
    /// Write-protected.
    /// </summary>
    Protected = 1 << 7,

    /// <summary>
    /// Purge level bit 0 (bits 8-9 form the purge level).
    /// </summary>
    Purge0 = 1 << 8,

    /// <summary>
    /// Purge level bit 1 (bits 8-9 form the purge level).
    /// </summary>
    Purge1 = 1 << 9,

    /// <summary>
    /// Must be loaded at specific location.
    /// </summary>
    AbsLoad = 1 << 10,

    /// <summary>
    /// Converter routine required.
    /// </summary>
    Converter = 1 << 11,

    /// <summary>
    /// Memory must be fixed in place.
    /// </summary>
    Fixed = 1 << 14,

    /// <summary>
    /// Memory locked.
    /// </summary>
    Locked = 1 << 15,
}
