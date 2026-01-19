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
    /// SOS reserved.
    /// </summary>
    SosReserved3 = 0x16,

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
    /// ProDOS reserved.
    /// </summary>
    ProDosReserved = 0xF9,

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
    /// ProDOS system file.
    /// </summary>
    ProDosSystem = 0xFF
}
