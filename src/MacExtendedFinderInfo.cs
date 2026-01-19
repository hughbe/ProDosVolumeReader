namespace ProDosVolumeReader;

/// <summary>
/// Represents the Mac HFS Extended Finder Info (FXInfo) for a file.
/// </summary>
public readonly struct MacExtendedFinderInfo
{
    /// <summary>
    /// Gets the icon ID.
    /// </summary>
    public short IconID { get; }

    /// <summary>
    /// Gets the script flag and code.
    /// </summary>
    public sbyte Script { get; }

    /// <summary>
    /// Gets the extended flags (reserved).
    /// </summary>
    public sbyte XFlags { get; }

    /// <summary>
    /// Gets the comment ID.
    /// </summary>
    public short CommentID { get; }

    /// <summary>
    /// Gets the home directory ID (put away location).
    /// </summary>
    public int PutAway { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MacExtendedFinderInfo"/> struct.
    /// </summary>
    /// <param name="data">The 16-byte extended Finder data.</param>
    public MacExtendedFinderInfo(ReadOnlySpan<byte> data)
    {
        // FXInfo structure (all big-endian as per Mac conventions):
        // fdIconID:   2 bytes - icon ID
        // fdUnused:   6 bytes - unused (3 integers)
        // fdScript:   1 byte  - script flag and code
        // fdXFlags:   1 byte  - reserved
        // fdComment:  2 bytes - comment ID
        // fdPutAway:  4 bytes - home directory ID
        IconID = (short)((data[0] << 8) | data[1]);
        // Skip 6 bytes of unused data (bytes 2-7)
        Script = (sbyte)data[8];
        XFlags = (sbyte)data[9];
        CommentID = (short)((data[10] << 8) | data[11]);
        PutAway = (data[12] << 24) | (data[13] << 16) | (data[14] << 8) | data[15];
    }
}
