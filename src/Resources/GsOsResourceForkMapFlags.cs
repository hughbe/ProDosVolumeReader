namespace ProDosVolumeReader.Resources;

/// <summary>
/// Control flags for a GS/OS Resource Fork Map.
/// </summary>
[Flags]
public enum GsOsResourceForkMapFlags : ushort
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates whether the resource map has been modified and must be written to disk when the file is closed.
    /// 1 - Map changed, 0 - Map not changed.
    /// </summary>
    Changed = 1 << 1,
}
