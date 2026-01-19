
/// <summary>
/// Control Manager flags (high-order byte of control flags word).
/// </summary>
[Flags]
public enum ControlMoreFlags : ushort
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0x0000,

    /// <summary>
    /// This control is currently the target of any typing or editing commands.
    /// </summary>
    FCtlTarget = 0x8000,

    /// <summary>
    /// This control can be made the target control.
    /// </summary>
    FCtlCanBeTarget = 0x4000,

    /// <summary>
    /// This control can be called when events are passed via SendEventToCtl.
    /// </summary>
    FCtlWantEvents = 0x2000,

    /// <summary>
    /// ctlProc contains the ID of a standard control procedure (not a pointer).
    /// </summary>
    FCtlProcRefNotPtr = 0x1000,

    /// <summary>
    /// This control needs to be notified when the size of the owning window has changed.
    /// </summary>
    FCtlTellAboutSize = 0x0800,

    /// <summary>
    /// This is a multipart control.
    /// </summary>
    FCtlIsMultiPart = 0x0400,
}
