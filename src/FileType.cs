namespace ProDosVolumeReader;

/// <summary>
/// Represents the ProDOS file type, a descriptor of the internal structure of the file.
/// </summary>
public enum FileType : byte
{
    /// <summary>
    /// Typeless file (SOS and ProDOS).
    /// </summary>
    Typeless = 0x00,

    /// <summary>
    /// Bad block file.
    /// </summary>
    BadBlock = 0x01,

    /// <summary>
    /// Pascal code file.
    /// </summary>
    PascalCode = 0x02,

    /// <summary>
    /// Pascal text file.
    /// </summary>
    PascalText = 0x03,

    /// <summary>
    /// ASCII text file (SOS and ProDOS).
    /// </summary>
    Text = 0x04,

    /// <summary>
    /// Pascal data file.
    /// </summary>
    PascalData = 0x05,

    /// <summary>
    /// General binary file (SOS and ProDOS).
    /// </summary>
    Binary = 0x06,

    /// <summary>
    /// Font file.
    /// </summary>
    Font = 0x07,

    /// <summary>
    /// Graphics screen file.
    /// </summary>
    GraphicsScreen = 0x08,

    /// <summary>
    /// Business BASIC program file.
    /// </summary>
    BusinessBasicProgram = 0x09,

    /// <summary>
    /// Business BASIC data file.
    /// </summary>
    BusinessBasicData = 0x0A,

    /// <summary>
    /// Word Processor file.
    /// </summary>
    WordProcessor = 0x0B,

    /// <summary>
    /// SOS system file.
    /// </summary>
    SosSystem = 0x0C,

    /// <summary>
    /// SOS reserved.
    /// </summary>
    SosReserved1 = 0x0D,

    /// <summary>
    /// SOS reserved.
    /// </summary>
    SosReserved2 = 0x0E,

    /// <summary>
    /// Directory file (SOS and ProDOS).
    /// </summary>
    Directory = 0x0F,

    /// <summary>
    /// RPS data file.
    /// </summary>
    RpsData = 0x10,

    /// <summary>
    /// RPS index file.
    /// </summary>
    RpsIndex = 0x11,

    /// <summary>
    /// AppleFile discard file.
    /// </summary>
    AppleFileDiscard = 0x12,

    /// <summary>
    /// AppleFile model file.
    /// </summary>
    AppleFileModel = 0x13,

    /// <summary>
    /// AppleFile report format file.
    /// </summary>
    AppleFileReportFormat = 0x14,

    /// <summary>
    /// Screen Library file.
    /// </summary>
    ScreenLibrary = 0x15,

    /// <summary>
    /// PFS Document file.
    /// </summary>
    PFSDocument = 0x16,

    /// <summary>
    /// SOS reserved.
    /// </summary>
    SosReserved4 = 0x17,

    /// <summary>
    /// SOS reserved.
    /// </summary>
    SosReserved5 = 0x18,

    /// <summary>
    /// AppleWorks Data Base file.
    /// </summary>
    AppleWorksDatabase = 0x19,

    /// <summary>
    /// AppleWorks Word Processor file.
    /// </summary>
    AppleWorksWordProcessor = 0x1A,

    /// <summary>
    /// AppleWorks Spreadsheet file.
    /// </summary>
    AppleWorksSpreadsheet = 0x1B,

    /// <summary>
    /// Desktop Manager file.
    /// </summary>
    DesktopManager = 0x20,

    /// <summary>
    /// Instant Pascal source code file.
    /// </summary>
    InstantPascalSourceCode = 0x21,

    /// <summary>
    /// UCSD Pascal volume file.
    /// </summary>
    UCSDPascalVolume = 0x22,

    /// <summary>
    /// SOS directory file.
    /// </summary>
    SOSDirectory = 0x29,

    /// <summary>
    /// Source code file.
    /// </summary>
    SourceCode = 0x2A,

    /// <summary>
    /// Object code file.
    /// </summary>
    ObjectCode = 0x2B,

    /// <summary>
    /// Interpreted code file.
    /// </summary>
    InterpretedCode = 0x2C,

    /// <summary>
    /// Language data file.
    /// </summary>
    LanguageData = 0x2D,

    /// <summary>
    /// ProDOS-8 code module file.
    /// </summary>
    ProDOS8CodeModule = 0x2E,

    /// <summary>
    /// Optical Character Recognition file.
    /// </summary>
    OpticalCharacterRecognition = 0x41,

    /// <summary>
    /// File type definitions file.
    /// </summary>
    FileTypeDefinitions = 0x42,

    /// <summary>
    /// Apple IIgs word processing file.
    /// </summary>
    AppleIIgsWordProcessing = 0x50,

    /// <summary>
    /// Apple IIgs spreadsheet file.
    /// </summary>
    AppleIIgsSpreadsheet = 0x51,

    /// <summary>
    /// Apple IIgs database file.
    /// </summary>
    AppleIIgsDatabase = 0x52,

    /// <summary>
    /// Apple IIgs object oriented graphics file.
    /// </summary>
    AppleIIgsObjectOrientedGraphics = 0x53,

    /// <summary>
    /// Apple IIgs Desktop Publishing file.
    /// </summary>
    AppleIIgsDesktopPublishing = 0x54,

    /// <summary>
    /// HyperMedia file.
    /// </summary>
    HyperMedia = 0x55,

    /// <summary>
    /// Educational Program Data file.
    /// </summary>
    EducationalProgramData = 0x56,

    /// <summary>
    /// Stationary file.
    /// </summary>
    Stationary = 0x57,

    /// <summary>
    /// Help file.
    /// </summary>
    Help = 0x58,

    /// <summary>
    /// Communications file.
    /// </summary>
    Communications = 0x59,

    /// <summary>
    /// Configuration file.
    /// </summary>
    Configuration = 0x5A,

    /// <summary>
    /// Animation file.
    /// </summary>
    Animation = 0x5B,

    /// <summary>
    /// Multimedia file.
    /// </summary>
    Multimedia = 0x5C,

    /// <summary>
    /// Entertainment file.
    /// </summary>
    Entertainment = 0x5D,

    /// <summary>
    /// Development Utility file.
    /// </summary>
    DevelopmentUtility = 0x5E,

    /// <summary>
    /// PC pre-boot file.
    /// </summary>
    PCPreBoot1 = 0x60,

    /// <summary>
    /// ProDOS File Navigator Command file
    /// </summary>
    ProDOSFileNavigatorCommand = 0x66,

    /// <summary>
    /// PC BIOS file.
    /// </summary>
    PCBIOS = 0x6B,

    /// <summary>
    /// PC driver file.
    /// </summary>
    PCDriver = 0x6D,

    /// <summary>
    /// PC pre-boot file.
    /// </summary>
    PCPreBoot2 = 0x6E,

    /// <summary>
    /// PC hard disk file.
    /// </summary>
    PCHardDisk = 0x6F,

    /// <summary>
    /// GES system file.
    /// </summary>
    GESSystemFile = 0x80,

    /// <summary>
    /// GEADesk Accessory file.
    /// </summary>
    GEADeskAccessory = 0x81,

    /// <summary>
    /// GEO Application file.
    /// </summary>
    GEOApplication = 0x82,

    /// <summary>
    /// GED Document file.
    /// </summary>
    GEDDocument = 0x83,

    /// <summary>
    /// GEF Font file.
    /// </summary>
    GEFFont = 0x84,

    /// <summary>
    /// GEP Printer Driver file.
    /// </summary>
    GEPPrinterDriver = 0x85,

    /// <summary>
    /// GEI Input Driver file.
    /// </summary>
    GEIInputDriver = 0x86,

    /// <summary>
    /// GEX Auxiliary Driver file.
    /// </summary>
    GEXAuxiliaryDriver = 0x87,

    /// <summary>
    /// GEV Swap File.
    /// </summary>
    GEVSwapFile = 0x89,

    /// <summary>
    /// GEC Clock Driver file.
    /// </summary>
    GECClockDriver = 0x8B,

    /// <summary>
    /// GEK Interface Card Driver file.
    /// </summary>
    GEKInterfaceCardDriver = 0x8C,

    /// <summary>
    /// GEW Formatting Data file.
    /// </summary>
    GEWFormattingData = 0x8D,

    /// <summary>
    /// WordPerfect file.
    /// </summary>
    WordPerfect = 0xA0,

    /// <summary>
    /// Apple IIgs BASIC Program file.
    /// </summary>
    AppleIIgsBASICProgram = 0xAB,

    /// <summary>
    /// Apple IIgs BASIC TDF file.
    /// </summary>
    AppleIIgsBASICTDF = 0xAC,

    /// <summary>
    /// Apple IIgs BASIC Data file.
    /// </summary>
    AppleIIgsBASICData = 0xAD,

    /// <summary>
    /// Apple IIgs source code file.
    /// </summary>
    AppleIIgsSourceCode = 0xB0,

    /// <summary>
    /// Apple IIgs object code file.
    /// </summary>
    AppleIIgsObjectCode = 0xB1,

    /// <summary>
    /// Apple IIgs library file.
    /// </summary>
    AppleIIgsLibrary = 0xB2,

    /// <summary>
    /// Apple IIgs application program file.
    /// </summary>
    AppleIIgsApplicationProgram = 0xB3,

    /// <summary>
    /// Apple IIgs runtime library file.
    /// </summary>
    AppleIIgsRuntimeLibrary = 0xB4,

    /// <summary>
    /// Apple IIgs shell script file.
    /// </summary>
    AppleIIgsShellScript = 0xB5,

    /// <summary>
    /// Apple IIgs permanent INIT file.
    /// </summary>
    AppleIIgsPermanentINIT = 0xB6,

    /// <summary>
    /// Apple IIgs temporary INIT file.
    /// </summary>
    AppleIIgsTemporaryINIT = 0xB7,

    /// <summary>
    /// Apple IIgs New Desk Accessory file.
    /// </summary>
    AppleIIgsNewDeskAccessory = 0xB8,

    /// <summary>
    /// Apple IIgs Classic Desk Accessory file.
    /// </summary>
    AppleIIgsClassicDeskAccessory = 0xB9,

    /// <summary>
    /// Apple IIgs Tool
    /// </summary>
    AppleIIgsTool = 0xBA,

    /// <summary>
    /// Apple IIgs Device Driver file.
    /// </summary>
    AppleIIgsDeviceDriver = 0xBB,

    /// <summary>
    /// Apple IIgs Generic Load File
    /// </summary>
    AppleIIgsGenericLoadFile = 0xBC,

    /// <summary>
    /// Apple IIgs File System Translator
    /// </summary>
    AppleIIgsFileSystemTranslator = 0xBD,

    /// <summary>
    /// Apple IIgs Document file.
    /// </summary>
    AppleIIgsDocument = 0xBF,

    /// <summary>
    /// Apple IIgs high-resolution graphics file.
    /// </summary>
    AppleIIgsPackedSuperHiRes = 0xC0,

    /// <summary>
    /// Apple IIgs super high-resolution graphics file.
    /// </summary>
    AppleIIgsSuperHiRes = 0xC1,

    /// <summary>
    /// PaintWorks animation file.
    /// </summary>
    PaintWorksAnimation = 0xC2,

    /// <summary>
    /// PaintWorks palette file.
    /// </summary>
    PaintWorksPalette = 0xC3,

    /// <summary>
    /// Object Oriented Graphics file.
    /// </summary>
    ObjectOrientedGraphics = 0xC4,

    /// <summary>
    /// Script file.
    /// </summary>
    Script = 0xC6,

    /// <summary>
    /// Apple IIgs Control Panel file.
    /// </summary>
    AppleIIgsControlPanel = 0xC7,

    /// <summary>
    /// Apple IIgs font file.
    /// </summary>
    AppleIIgsFont = 0xC8,

    /// <summary>
    /// Apple IIgs Finder data file.
    /// </summary>
    AppleIIgsFinderData = 0xC9,

    /// <summary>
    /// Apple IIgs icon file.
    /// </summary>
    AppleIIgsIconFile = 0xCA,

    /// <summary>
    /// Audio music file.
    /// </summary>
    Music = 0xD5,

    /// <summary>
    /// Audio instrument file.
    /// </summary>
    Instrument = 0xD6,

    /// <summary>
    /// Audio MIDI file.
    /// </summary>
    MIDI = 0xD7,

    /// <summary>
    /// Apple IIgs audio file.
    /// </summary>
    AppleIIgsAudio = 0xD8,

    /// <summary>
    /// Audio DBMaster document file.
    /// </summary>
    DBMasterDocument = 0xD9,

    /// <summary>
    /// LBR archive file.
    /// </summary>
    LBRArchive = 0xE0,

    /// <summary>
    /// AppleTalk data file.
    /// </summary>
    AppleTalkData = 0xE2,

    /// <summary>
    /// EDASM 816 relocatable code file.
    /// </summary>
    EDASM816RelocatableCode = 0xEE,

    /// <summary>
    /// Pascal area.
    /// </summary>
    PascalArea = 0xEF,

    /// <summary>
    /// ProDOS CI added command file.
    /// </summary>
    ProDosAddedCommand = 0xF0,

    /// <summary>
    /// ProDOS user defined file 1.
    /// </summary>
    UserDefined1 = 0xF1,

    /// <summary>
    /// ProDOS user defined file 2.
    /// </summary>
    UserDefined2 = 0xF2,

    /// <summary>
    /// ProDOS user defined file 3.
    /// </summary>
    UserDefined3 = 0xF3,

    /// <summary>
    /// ProDOS user defined file 4.
    /// </summary>
    UserDefined4 = 0xF4,

    /// <summary>
    /// ProDOS user defined file 5.
    /// </summary>
    UserDefined5 = 0xF5,

    /// <summary>
    /// ProDOS user defined file 6.
    /// </summary>
    UserDefined6 = 0xF6,

    /// <summary>
    /// ProDOS user defined file 7.
    /// </summary>
    UserDefined7 = 0xF7,

    /// <summary>
    /// ProDOS user defined file 8.
    /// </summary>
    UserDefined8 = 0xF8,

    /// <summary>
    /// ProDOS-16 system file.
    /// </summary>
    ProDos16System = 0xF9,

    /// <summary>
    /// Integer BASIC program file.
    /// </summary>
    IntegerBasicProgram = 0xFA,

    /// <summary>
    /// Integer BASIC variable file.
    /// </summary>
    IntegerBasicVariables = 0xFB,

    /// <summary>
    /// Applesoft program file.
    /// </summary>
    ApplesoftProgram = 0xFC,

    /// <summary>
    /// Applesoft variables file.
    /// </summary>
    ApplesoftVariables = 0xFD,

    /// <summary>
    /// Relocatable code file (EDASM).
    /// </summary>
    RelocatableCode = 0xFE,

    /// <summary>
    /// ProDOS-8 system file.
    /// </summary>
    ProDos8System = 0xFF
}
