namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Patterns for window bar colors.
/// </summary>
public enum WindowBarColorPattern : byte
{
    /// <summary>
    /// Solid color pattern.
    /// </summary>
    Solid = 0x00,

    /// <summary>
    /// Dithered color pattern.
    /// </summary>
    Dither = 0x01,

    /// <summary>
    /// Lined color pattern.
    /// </summary>
    Lined = 0x02,
}
