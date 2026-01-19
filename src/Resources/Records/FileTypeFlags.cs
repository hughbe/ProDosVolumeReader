namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// File Type Flags
/// </summary>
[Flags]
public enum FileTypeFlags : ushort
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Bit 15: This record matches this file type and any auxiliary type.
    /// Example: file type $FF (ProDOS 8 application) would use this.
    /// </summary>
    MatchFileTypeAnyAux = 0x8000,

    /// <summary>
    /// Bit 14: This record matches this auxiliary type and any file type.
    /// </summary>
    MatchAuxTypeAnyFile = 0x4000,

    /// <summary>
    /// Bit 13: This record is the beginning of a range of file types
    /// and auxiliary types to match this string. Any combination falling
    /// linearly between this record and the record with <see cref="RangeEnd"/>
    /// set should be given this string by default if no specific match is found.
    /// </summary>
    RangeStart = 0x2000,

    /// <summary>
    /// Bit 12: This record is the end of a range of file types and auxiliary types
    /// to match this string. Any combination falling linearly between the record
    /// with <see cref="RangeStart"/> set and this record should be given this
    /// string by default if no specific match is found.
    /// </summary>
    RangeEnd = 0x1000,

    /// <summary>
    /// Bits 11-0 are reserved and must be zero when creating files.
    /// This mask may be used to validate that reserved bits are clear.
    /// </summary>
    ReservedMask = 0x0FFF
}
