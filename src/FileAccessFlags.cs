namespace ProDosVolumeReader;

/// <summary>
/// Determines whether a file can be read, written, destroyed, and renamed,
/// and whether the file needs to be backed up.
/// </summary>
[Flags]
public enum FileAccessFlags : byte
{
    /// <summary>
    /// No access flags are set.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Read-enabled.
    /// </summary>
    ReadEnabled = 0x01,

    /// <summary>
    /// Write-enabled.
    /// </summary>
    WriteEnabled = 0x02,

    /// <summary>
    /// File-invisible (GS/OS addition).
    /// </summary>
    FileInvisible = 0x04,

    /// <summary>
    /// Backup-needed.
    /// </summary>
    BackupNeeded = 0x20,

    /// <summary>
    /// Rename-enabled.
    /// </summary>
    RenameEnabled = 0x40,

    /// <summary>
    /// Destroy-enabled.
    /// </summary>
    DestroyEnabled = 0x80,
}
