using System.Diagnostics;

namespace ProDosVolumeReader.Resources;

/// <summary>
/// The GS/OS Resource Fork Map.
/// </summary>
public readonly struct GsOsResourceForkMap
{
    /// <summary>
    /// Gets the Resource Fork Map Header.
    /// </summary>
    public GsOsResourceForkMapHeader Header { get; }

    /// <summary>
    /// Gets the array of Free Blocks in the Resource Fork Map.
    /// </summary>
    public GsOsResourceForkFreeBlock[] FreeBlocks { get; }

    /// <summary>
    /// Gets the array of Resource Reference Records in the Resource Fork Map.
    /// </summary>
    public GsOsResourceForkReferenceRecord[] ReferenceRecords { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GsOsResourceForkMap"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Resource Fork Map.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public GsOsResourceForkMap(ReadOnlySpan<byte> data)
    {
        if (data.Length < GsOsResourceForkMapHeader.Size)
        {
            throw new ArgumentException($"Data length must be at least {GsOsResourceForkMapHeader.Size} bytes.", nameof(data));
        }

        // Structure documented in https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
        // and https://ciderpress2.com/formatdoc/ResourceFork-notes.html
        int offset = 0;

        Header = new GsOsResourceForkMapHeader(data.Slice(offset, GsOsResourceForkMapHeader.Size));
        offset += GsOsResourceForkMapHeader.Size;

        // Array of resource free blocks, which describe free space in the
        // resource file.
        // Note: it isn't clear if the mapFreeList immediately follows the mapHeader,
        // but this is the only interpretation that makes sense.
        var freeBlocks = new GsOsResourceForkFreeBlock[Header.MapFreeListUsed];
        for (int i = 0; i < Header.MapFreeListUsed; i++)
        {
            freeBlocks[i] = new GsOsResourceForkFreeBlock(data.Slice(offset, GsOsResourceForkFreeBlock.Size));
            offset += GsOsResourceForkFreeBlock.Size;
        }

        FreeBlocks = freeBlocks;

        // Move to the start of the Reference Records
        if (Header.MapToIndex > data.Length)
        {
            throw new ArgumentException("MapToIndex offset is beyond end of data.", nameof(data));
        }

        offset = Header.MapToIndex;

        // Array of resource reference records, which contain control information
        // about the resources in the resource file.
        var referenceRecords = new GsOsResourceForkReferenceRecord[Header.MapIndexUsed];
        for (int i = 0; i < Header.MapIndexUsed; i++)
        {
            referenceRecords[i] = new GsOsResourceForkReferenceRecord(data.Slice(offset, GsOsResourceForkReferenceRecord.Size));
            offset += GsOsResourceForkReferenceRecord.Size;

            if (referenceRecords[i].IsEndOfList)
            {
                // Stop processing further records
                break;
            }
        }

        ReferenceRecords = referenceRecords;

        Debug.Assert(offset <= data.Length, "Did not consume all data for ResourceForkMap.");
    }
}