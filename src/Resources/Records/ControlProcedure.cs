
/// <summary>
/// Control procedure types for Apple IIgs controls.
/// </summary>
public enum ControlProcedure : uint
{
    /// <summary>
    /// Simple button
    /// </summary>
    SimpleButton = 0x80000000,

    /// <summary>
    /// Check box
    /// </summary>
    CheckBox = 0x82000000,

    /// <summary>
    /// Icon button
    /// </summary>
    IconButton = 0x07FF0001,

    /// <summary>
    /// LineEdit
    /// </summary>
    LineEdit = 0x83000000,

    /// <summary>
    /// List
    /// </summary>
    List = 0x89000000,

    /// <summary>
    /// Picture
    /// </summary>
    Picture = 0x8D000000,

    /// <summary>
    /// Pop-up
    /// </summary>
    PopUp = 0x87000000,

    /// <summary>
    /// Radio control
    /// </summary>
    RadioButton = 0x84000000,

    /// <summary>
    /// Scroll bar
    /// </summary>
    ScrollBar = 0x86000000,

    /// <summary>
    /// Size box
    /// </summary>
    SizeBox = 0x88000000,

    /// <summary>
    /// Static Text
    /// </summary>
    StaticText = 0x81000000,

    /// <summary>
    /// TextEdit
    /// </summary>
    TextEdit = 0x85000000,
}
