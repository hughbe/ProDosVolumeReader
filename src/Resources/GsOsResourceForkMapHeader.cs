using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources;

/// <summary>
/// The header structure for a GS/OS Resource Fork Map.
/// </summary>
public readonly struct GsOsResourceForkMapHeader
{
    /// <summary>
    /// Size of Resource Fork Map Header structure in bytes.
    /// </summary>
    public const int Size = 32;

    /// <summary>
    /// Zeroes (reserved for handle to next resource file).
    /// </summary>
    public uint MapNext { get; }

    /// <summary>
    /// Control flags; really just a "dirty" bit when in memory.
    /// </summary>
    public ushort MapFlag { get; }

    /// <summary>
    /// Offset, in bytes, to resource map (same as rFileToMap when first loaded).
    /// </summary>
    public uint MapOffset { get; }

    /// <summary>
    /// Size, in bytes, of resource map (same as rFileMapSize when first loaded).
    /// </summary>
    public uint MapSize { get; }

    /// <summary>
    /// Offset from start of map to start of mapIndex array.
    /// </summary>
    public ushort MapToIndex { get; }

    /// <summary>
    /// Zeroes (reserved for GS/OS file reference number).
    /// </summary>
    public ushort MapFileNum { get; }

    /// <summary>
    /// Zeroes (reserved for resource manager file ID for open resource file).
    /// </summary>
    public ushort MapID { get; }

    /// <summary>
    /// Total number of resource reference records in mapIndex.
    /// </summary>
    public uint MapIndexSize { get; }

    /// <summary>
    /// Number of used resource reference records in mapIndex.
    /// </summary>
    public uint MapIndexUsed { get; }

    /// <summary>
    /// Number of resource free blocks in mapFreeList.
    /// </summary>
    public ushort MapFreeListSize { get; }

    /// <summary>
    /// Number of used resource free blocks in mapFreeList.
    /// </summary>
    public ushort MapFreeListUsed { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GsOsResourceForkMapHeader"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the map header.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public GsOsResourceForkMapHeader(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data length must be {Size} bytes.", nameof(data));
        }

        // Structure documented in https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
        // and https://ciderpress2.com/formatdoc/ResourceFork-notes.html
        int offset = 0;

        // Handle to resource map for next resource file in the search chain. Set
        // to NIL if last file in chain. This field is only valid when the map is in
        // memory.
        MapNext = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Contains control flags defining the state of the resource file.
        // Reserved bits 2-15 Set to 0
        // mapChanged bit 1 Indicates whether the resource map has been
        // modified, and must therefore be written to disk
        // when the file is closed:
        // 1 - Map changed
        // 0 - Map not changed
        // Reserved bit 0 Set to 0
        MapFlag = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Offset, in bytes, to the resource map from the beginning of the
        // resource file.
        MapOffset = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Size, in bytes, of the resource map on disk. Note that the memory
        // image of the map may have a different size due to resource or
        // resource file changes during program execution.
        MapSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Offset, in bytes, from beginning of map to the beginning of the
        // mapIndex array of resource reference records.
        MapToIndex = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // GS/OS file reference number. This field is valid only in memory.
        MapFileNum = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Resource Manager file ID for the open resource file. This field is valid
        // only in memory.
        MapID = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Total number of resource reference records in mapIndex.
        MapIndexSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Number of used resource reference records in mapIndex.
        MapIndexUsed = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Total number of resource free blocks in mapF reeList.
        MapFreeListSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Number of used resource free blocks in mapF reeList.
        MapFreeListUsed = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for ResourceForkMapHeader.");
    }
}
