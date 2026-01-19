namespace ProDosVolumeReader;

/// <summary>
/// Represents an extended key block for GS/OS forked files.
/// The block contains mini-entries for the data fork at +$0000 and resource fork at +$0100,
/// plus optional HFS Finder info.
/// </summary>
public readonly struct ExtendedKeyBlock
{
    /// <summary>
    /// The offset within the block where the data fork entry is located.
    /// </summary>
    public const int DataForkOffset = 0x00;

    /// <summary>
    /// The offset within the block where the resource fork entry is located.
    /// </summary>
    public const int ResourceForkOffset = 0x100;

    /// <summary>
    /// The offset within the data fork section where optional Finder info starts.
    /// </summary>
    public const int FinderInfoOffset = 0x08;

    /// <summary>
    /// Gets the data fork entry.
    /// </summary>
    public ExtendedKeyBlockEntry DataFork { get; }

    /// <summary>
    /// Gets the resource fork entry.
    /// </summary>
    public ExtendedKeyBlockEntry ResourceFork { get; }

    /// <summary>
    /// Gets the Finder info (FInfo), if present.
    /// </summary>
    public MacFinderInfo? FinderInfo { get; }

    /// <summary>
    /// Gets the extended Finder info (FXInfo), if present.
    /// </summary>
    public MacExtendedFinderInfo? ExtendedFinderInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedKeyBlock"/> struct.
    /// </summary>
    /// <param name="blockData">The 512-byte block data.</param>
    public ExtendedKeyBlock(ReadOnlySpan<byte> blockData)
    {
        // Data fork mini-entry at +$0000
        DataFork = new ExtendedKeyBlockEntry(blockData[..ExtendedKeyBlockEntry.Size]);

        // Resource fork mini-entry at +$0100
        ResourceFork = new ExtendedKeyBlockEntry(blockData.Slice(ResourceForkOffset, ExtendedKeyBlockEntry.Size));

        // Try to parse optional Finder info entries starting at +$08
        // Format:
        //   +$08 / 1: size of first entry (must be 18)
        //   +$09 / 1: type of entry (1 for FInfo, 2 for FXInfo)
        //   +$0a /16: 16 bytes of Finder data
        //   +$1a / 1: size of second entry (must be 18)
        //   +$1b / 1: type of entry (1 for FInfo, 2 for FXInfo)
        //   +$1c /16: 16 bytes of Finder data
        MacFinderInfo? finderInfo = null;
        MacExtendedFinderInfo? extFinderInfo = null;

        int offset = FinderInfoOffset;
        for (int i = 0; i < 2; i++)
        {
            byte entrySize = blockData[offset];
            offset += 1;

            byte entryType = blockData[offset];
            offset += 1;

            if (entrySize != MacFinderInfo.Size + 2)
            {
                // No more valid entries
                break;
            }

            var finderData = blockData.Slice(offset, MacFinderInfo.Size);

            if (entryType == MacFinderInfo.FInfoType)
            {
                finderInfo = new MacFinderInfo(finderData);
            }
            else if (entryType == MacFinderInfo.FXInfoType)
            {
                extFinderInfo = new MacExtendedFinderInfo(finderData);
            }
            else
            {
                throw new NotSupportedException($"Unknown Finder info entry type: {entryType}");
            }

            offset += MacFinderInfo.Size + 2;
        }

        FinderInfo = finderInfo;
        ExtendedFinderInfo = extFinderInfo;
    }
}
