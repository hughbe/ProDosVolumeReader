using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources;

/// <summary>
/// The header structure for a GS/OS Resource Fork.
/// </summary>
public readonly struct GsOsResourceForkHeader
{
    /// <summary>
    /// Size of Resource Fork Header structure in bytes
    /// </summary>
    public const int Size = 140;

    /// <summary>
    /// Version number for the entire file. Always zero. (Mac will be > 127.)
    /// </summary>
    public uint Version { get; }

    /// <summary>
    /// Offset, in bytes, of start of resource map
    /// </summary>
    public uint MapOffset { get; }

    /// <summary>
    /// Size, in bytes, of the resource map
    /// </summary>
    public uint MapSize { get; }

    /// <summary>
    /// Available for application data
    /// </summary>
    public byte[] Memo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GsOsResourceForkHeader"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the header.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public GsOsResourceForkHeader(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data length must be {Size} bytes.", nameof(data));
        }

        // Structure documented in https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
        // and https://ciderpress2.com/formatdoc/ResourceFork-notes.html
        int offset = 0;

        // Version number defining layout of resource file. Currently, only version
        // 0 is supported. This field allows IIGS resource files to be distinguished
        // from Macintosh resource files; the first long in Macintosh resource files
        // must have a value that is greater than 127.
        Version = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Offset, in bytes, to beginning of the resource map. This offset starts
        // from the beginning of the resource file.
        MapOffset = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Size, in bytes, of the resource map.
        MapSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Reserved for application use. The Resource Manager does not provide
        // any facility for reading or writing this field. Your program must use
        // GS/OS file system calls to access the rFileMemo field.
        Memo = data.Slice(offset, 128).ToArray();
        offset += Memo.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for ResourceForkHeader.");
    }
}
