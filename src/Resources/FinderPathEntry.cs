using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// An entry in a Finder Path.
/// </summary>
public readonly struct FinderPathEntry
{
    /// <summary>
    /// Size of FinderPathEntry structure in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// Gets the rVersion resource ID.
    /// </summary>
    public uint VersionResourceID { get; }

    /// <summary>
    /// Gets the rVersion resource handle.
    /// </summary>
    public uint VersionResourceHandle { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinderPathEntry"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the FinderPathEntry.</param>
    /// <exception cref="ArgumentException">Thrown when data is the wrong size.</exception>
    public FinderPathEntry(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"FinderPathEntry requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1301-L1315
        int offset = 0;

        VersionResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        VersionResourceHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;
        
        Debug.Assert(offset == data.Length, "Did not consume all data for FinderPathEntry.");
    }
}
