using System.Buffers.Binary;
using System.Diagnostics;
using ProDosVolumeReader.Utilities;

namespace ProDosVolumeReader;

/// <summary>
/// Represents the Mac HFS Extended Finder Info (FXInfo) for a file.
/// </summary>
public readonly struct MacExtendedFinderInfo
{
    /// <summary>
    /// The size of the extended Finder info data in bytes (16 bytes).
    /// </summary>
    public const int Size = 16;

    /// <summary>
    /// Gets the icon ID.
    /// </summary>
    public short IconID { get; }

    /// <summary>
    /// Gets the unused bytes (6 bytes reserved for future use).
    /// </summary>
    public ByteArray6 Unused { get; }

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
        if (data.Length != Size)
        {
            throw new ArgumentException("Extended Finder Info must be exactly 16 bytes.", nameof(data));
        }

        // Structure documented in https://developer.apple.com/library/archive/documentation/mac/pdf/MacintoshToolboxEssentials.pdf
        // 7-73
        int offset = 0;

        // icon ID
        IconID = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        // unused but reserved 6 bytes
        Unused = new ByteArray6(data.Slice(offset, ByteArray6.Size));
        offset += ByteArray6.Size;

        // script flag and code
        Script = (sbyte)data[offset];
        offset += 1;

        // reserved
        XFlags = (sbyte)data[offset];
        offset += 1;

        // comment ID
        CommentID = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
        offset += 2;

        // home directory ID
        PutAway = BinaryPrimitives.ReadInt32BigEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "All bytes of the extended Finder info should be consumed.");
    }
}
