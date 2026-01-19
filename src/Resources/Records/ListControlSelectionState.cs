namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Selection state for a list control member.
/// </summary>
public enum ListControlSelectionState : byte
{
    /// <summary>
    /// Enabled and not selected.
    /// </summary>
    EnabledNotSelected = 0b00,

    /// <summary>
    /// Disabled.
    /// </summary>
    Disabled = 0b01,

    /// <summary>
    /// Enabled and selected.
    /// </summary>
    Selected = 0b10,

    /// <summary>
    /// Invalid state.
    /// </summary>
    Invalid = 0b11
}
