namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Flags for the TextEdit control template.
/// </summary>
[Flags]
public enum TextEditFlags : uint
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Must be set to 0.
    /// </summary>
    NoControl = 1u << 31,

    /// <summary>
    /// Must be set to 1.
    /// </summary>
    SingleFormat = 1u << 30,

    /// <summary>
    /// Indicates single style only.
    /// </summary>
    SingleStyle = 1u << 29,

    /// <summary>
    /// Indicates no word wrap.
    /// </summary>
    NoWordWrap = 1u << 28,

    /// <summary>
    /// Indicates no scrolling.
    /// </summary>
    NoScroll = 1u << 27,

    /// <summary>
    /// Indicates read-only text.
    /// </summary>
    ReadOnly = 1u << 26,

    /// <summary>
    /// Indicates smart cut and paste enabled.
    /// </summary>
    SmartCutPaste = 1u << 25,

    /// <summary>
    /// Indicates tab switching enabled.
    /// </summary>
    TabSwitch = 1u << 24,

    /// <summary>
    /// Indicates draw bounds enabled.
    /// </summary>
    DrawBounds = 1u << 23,

    /// <summary>
    /// Indicates color highlight enabled.
    /// </summary>
    ColorHilight = 1u << 22,

    /// <summary>
    /// Indicates grow ruler enabled.
    /// </summary>
    GrowRuler = 1u << 21,

    /// <summary>
    /// Disables text selection.
    /// </summary>
    DisableSelection = 1u << 20,

    /// <summary>
    /// Draws inactive selection.
    /// </summary>
    DrawInactiveSelection = 1u << 19,

}