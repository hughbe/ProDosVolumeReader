using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader;

/// <summary>
/// Represents the Mac HFS Finder Info (FInfo) for a file.
/// This is used by GS/OS for extended file information.
/// </summary>
public readonly struct MacFinderInfo
{
    /// <summary>
    /// The size in bytes of the Finder data itself.
    /// </summary>
    public const int Size = 16;

    /// <summary>
    /// Entry type for FInfo.
    /// </summary>
    public const byte FInfoType = 1;

    /// <summary>
    /// Entry type for FXInfo.
    /// </summary>
    public const byte FXInfoType = 2;

    /// <summary>
    /// Gets the file type as a 4-character OSType.
    /// </summary>
    public uint FileType { get; }

    /// <summary>
    /// Gets the file creator as a 4-character OSType.
    /// </summary>
    public uint Creator { get; }

    /// <summary>
    /// Gets the Finder flags.
    /// </summary>
    public ushort Flags { get; }

    /// <summary>
    /// Gets the file's vertical location in its window.
    /// </summary>
    public short LocationV { get; }

    /// <summary>
    /// Gets the file's horizontal location in its window.
    /// </summary>
    public short LocationH { get; }

    /// <summary>
    /// Gets the directory ID that contains the file.
    /// </summary>
    public short Folder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MacFinderInfo"/> struct.
    /// </summary>
    /// <param name="data">The 16-byte Finder data.</param>
    public MacFinderInfo(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be exactly {Size} bytes for MacFinderInfo", nameof(data));
        }

        int offset = 0;

        // fdType:     4 bytes - file type
        FileType = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        // fdCreator:  4 bytes - file creator
        Creator = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        // fdFlags:    2 bytes - Finder flags
        Flags = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        // fdLocation: 4 bytes - Point (v, h each 2 bytes)
        LocationV = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        LocationH = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        // fdFldr:     2 bytes - directory ID
        Folder = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for MacFinderInfo");
    }

    /// <summary>
    /// Gets the file type as a string.
    /// </summary>
    public string FileTypeString => GetOSTypeString(FileType);

    /// <summary>
    /// Gets the creator as a string.
    /// </summary>
    public string CreatorString => GetOSTypeString(Creator);

    private static string GetOSTypeString(uint osType)
    {
        Span<byte> bytes =
        [
            (byte)((osType >> 24) & 0xFF),
            (byte)((osType >> 16) & 0xFF),
            (byte)((osType >> 8) & 0xFF),
            (byte)(osType & 0xFF),
        ];
        return Encoding.ASCII.GetString(bytes);
    }
}
