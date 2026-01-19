using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources;

/// <summary>
/// A resource reference record in a GS/OS Resource Fork Map.
/// </summary>
public readonly struct GsOsResourceForkReferenceRecord
{
    /// <summary>
    /// Size of Resource Reference Record structure in bytes.
    /// </summary>
    public const int Size = 0x14;

    /// <summary>
    /// Gets the resource type; will be zero if this is the end of the list.
    /// </summary>
    public GsOsResourceForkType Type { get; }

    /// <summary>
    /// Gets the resource ID.
    /// </summary>
    public uint ResourceID { get; }

    /// <summary>
    /// Gets the offset, in bytes, to the resource from the start of the resource file.
    /// </summary>
    public uint DataOffset { get; }

    /// <summary>
    /// Gets the resource attributes.
    /// </summary>
    public GsOsResourceAttributes Attributes { get; }

    /// <summary>
    /// Gets the size, in bytes, of the resource.
    /// </summary>
    public uint DataSize { get; }

    /// <summary>
    /// Gets the zeroes (reserved for memory handle).
    /// </summary>
    public uint Handle { get; }

    /// <summary>
    /// Gets the purge level (0-3) extracted from the attributes.
    /// </summary>
    public int PurgeLevel => ((int)Attributes >> 8) & 0x3;

    /// <summary>
    /// Gets a value indicating whether this record marks the end of the list.
    /// </summary>
    public bool IsEndOfList => Type == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="GsOsResourceForkReferenceRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the reference record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public GsOsResourceForkReferenceRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < Size)
        {
            throw new ArgumentException($"Data length must be at least {Size} bytes.", nameof(data));
        }

        // Structure documented in https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
        // and https://ciderpress2.com/formatdoc/ResourceFork-notes.html
        int offset = 0;

        // Resource type. NIL value indicates last used entry in the array.
        Type = (GsOsResourceForkType)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Resource ID.
        ResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Offset, in bytes, to the resource from the start of the resource file.
        DataOffset = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Resource attributes. See "Resource attributes" earlier in this chapter
        // for bit flag definitions.
        Attributes = (GsOsResourceAttributes)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Size, in bytes, of the resource in the resource file. Note that the size
        // of the resource in memory may differ, due to changes made to the resource
        // by application programs or by resource converter routines.
        DataSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Handle of resource in memory. NIL value indicates that the resource
        // has not been loaded into memory. Your program can determine the in-
        // memory size of the resource by examining the size of this handle.
        Handle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == Size, "Did not consume all data for ResourceForkReferenceRecord.");
    }
}
