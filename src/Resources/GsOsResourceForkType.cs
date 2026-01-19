namespace ProDosVolumeReader.Resources;

/// <summary>
/// GS/OS Resource Fork Resource Types
/// </summary>
public enum GsOsResourceForkType : ushort
{
    /// <summary>
    /// Icon Resource
    /// </summary>
    Icon = 0x8001,

    /// <summary>
    /// Picture Resource
    /// </summary>
    Picture = 0x8002,

    /// <summary>
    /// Control List Resource
    /// </summary>
    ControlList = 0x8003,

    /// <summary>
    /// Control Template Resource
    /// </summary>
    ControlTemplate = 0x8004,

    /// <summary>
    /// Class 1 Input String Resource
    /// </summary>
    Class1InputString = 0x8005,

    /// <summary>
    /// Pascal String Resource
    /// </summary>
    PascalString = 0x8006,

    /// <summary>
    /// String List Resource
    /// </summary>
    StringList = 0x8007,

    /// <summary>
    /// Menu Bar Resource
    /// </summary>
    MenuBar = 0x8008,

    /// <summary>
    /// Menu Resource
    /// </summary>
    Menu = 0x8009,

    /// <summary>
    /// Menu Item Resource
    /// </summary>
    MenuItem = 0x800A,

    /// <summary>
    /// Text Box 2 Resource
    /// </summary>
    TextForLETextBox2 = 0x800B,

    /// <summary>
    /// Control Definition Procedure Resource
    /// </summary>
    ControlDefinitionProcedure = 0x800C,

    /// <summary>
    /// Control Color Table Resource
    /// </summary>
    ControlColorTable = 0x800D,

    /// <summary>
    /// Window Param1 Resource
    /// </summary>
    WindowParam1 = 0x800E,

    /// <summary>
    /// Window Param2 Resource
    /// </summary>
    WindowParam2 = 0x800F,

    /// <summary>
    /// Window Colors Resource
    /// </summary>
    WindowColorTable = 0x8010,

    /// <summary>
    /// Text Block Resource
    /// </summary>
    TextBlock = 0x8011,

    /// <summary>
    /// Text Edit Style Resource
    /// </summary>
    TextEditStyle = 0x8012,

    /// <summary>
    /// Tool Startup Resource
    /// </summary>
    ToolStartup = 0x8013,

    /// <summary>
    /// Resource Name Resource
    /// </summary>
    ResourceName = 0x8014,

    /// <summary>
    /// Alert String Resource
    /// </summary>
    AlertString = 0x8015,

    /// <summary>
    /// Text Resource
    /// </summary>
    Text = 0x8016,

    /// <summary>
    /// Code Resource
    /// </summary>
    Code = 0x8017,

    /// <summary>
    /// CDEV Code Resource
    /// </summary>
    CDEVCode = 0x8018,

    /// <summary>
    /// CDEV Flags Resource
    /// </summary>
    CDEVFlags = 0x8019,

    /// <summary>
    /// Two Rects Resource
    /// </summary>
    TwoRects = 0x801A,

    /// <summary>
    /// File Type Resource
    /// </summary>
    FileType = 0x801B,

    /// <summary>
    /// List Controls Resource
    /// </summary>
    ListControls = 0x801C,

    /// <summary>
    /// C String Resource
    /// </summary>
    CString = 0x801D,

    /// <summary>
    /// XCMD Resource
    /// </summary>
    XCMD = 0x801E,

    /// <summary>
    /// XFCN Resource
    /// </summary>
    XFCN = 0x801F,

    /// <summary>
    /// Error String Resource
    /// </summary>
    ErrorString = 0x8020,

    /// <summary>
    /// Keystroke Translation Table Resource
    /// </summary>
    KeystrokeTranslationTable = 0x8021,

    /// <summary>
    /// Wide String Resource
    /// </summary>
    WideString = 0x8022,

    /// <summary>
    /// Class 1 Output String Resource
    /// </summary>
    Class1OutputString = 0x8023,

    /// <summary>
    /// Samples Sound Resource
    /// </summary>
    SampledSound = 0x8024,

    /// <summary>
    /// Text Edit Ruler Resource
    /// </summary>
    TextEditRuler = 0x8025,

    /// <summary>
    /// F-Sequence Resource
    /// </summary>
    FSequence = 0x8026,

    /// <summary>
    /// Cursor Resource
    /// </summary>
    Cursor = 0x8027,

    /// <summary>
    /// Item Struct Resource
    /// </summary>
    ItemStruct = 0x8028,

    /// <summary>
    /// Version Resource
    /// </summary>
    Version = 0x8029,

    /// <summary>
    /// Comment Resource
    /// </summary>
    Comment = 0x802A,

    /// <summary>
    /// Bundle Resource
    /// </summary>
    Bundle = 0x802B,

    /// <summary>
    /// Finder Path Resource
    /// </summary>
    FinderPath = 0x802C,

    /// <summary>
    /// Palette Window Resource
    /// </summary>
    PaletteWindow = 0x802D,

    /// <summary>
    /// Tagged Strings Resource
    /// </summary>
    TaggedStrings = 0x802E,

    /// <summary>
    /// Pattern List Resource
    /// </summary>
    PatternList = 0x802F,

    /// <summary>
    /// Rectangle List Resource
    /// </summary>
    RectangleList = 0xC001,

    /// <summary>
    /// Print Record
    /// </summary>
    PrintRecord = 0xC002,

    /// <summary>
    /// Font Record
    /// </summary>
    FontRecord = 0xC003,
}