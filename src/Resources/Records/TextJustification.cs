namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Specifies text justification options for static text controls.
/// </summary>
public enum TextJustification : short
{
    /// <summary>
    /// Text is right justified in the display window.
    /// </summary>
    Right = -1,

    /// <summary>
    /// Text is left justified in the display window.
    /// </summary>
    Left = 0,

    /// <summary>
    /// Text is centered in the display window.
    /// </summary>
    Center = 1,

    /// <summary>
    /// Text is fully justified (both left and right) in the display window.
    /// </summary>
    Full = 2
}
