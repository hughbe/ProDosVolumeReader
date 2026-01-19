using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Resource name record.
/// </summary>
public readonly struct ResourceNameRecord
{
    /// <summary>
    /// Minimum size of ResourceNameRecord structure in bytes.
    /// </summary>
    public const int MinSize = 6;

    /// <summary>
    /// Gets the version number of the resource name record.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the count of names in the resource name record.
    /// </summary>
    public uint NameCount { get; }

    /// <summary>
    /// Gets the list of resource names.
    /// </summary>
    public List<ResourceName> Names { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNameRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the ResourceNameRecord record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ResourceNameRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-60
        int offset = 0;

        // The resource template version. Must be set to 1.
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Version != 1)
        {
            throw new ArgumentException($"Unsupported ResourceNameRecord version: {Version}", nameof(data));
        }

        // Count of entries in the resNames name-definition array.
        NameCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Array of name strings.
        var names = new List<ResourceName>((int)NameCount);
        for (uint i = 0; i < NameCount; i++)
        {
            names.Add(new ResourceName(data[offset..], out int bytesRead));
            offset += bytesRead;
        }

        Names = names;

        Debug.Assert(offset == data.Length, "Did not consume all data for ResourceNameRecord.");
    }
}
