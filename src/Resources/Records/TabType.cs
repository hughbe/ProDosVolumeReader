namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Specifies tab types for text rulers.
/// </summary>
public enum TabType : ushort
{
    /// <summary>
    /// No tab type specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// Regular tab type.
    /// </summary>
    Regular = 1,

    /// <summary>
    /// Absolute tab type.
    /// </summary>
    Absolute = 2,
}
