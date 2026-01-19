using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Resource name structure.
/// </summary>
public readonly struct ResourceName
{
    /// <summary>
    /// The minimum size of ResourceName structure in bytes.
    /// </summary>
    public const int MinSize = 5;

    /// <summary>
    /// Gets the resource ID.
    /// </summary>
    public uint ResourceID { get; }

    /// <summary>
    /// Gets the resource name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceName"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the ResourceName.</param>
    /// <param name="bytesRead"> The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ResourceName(ReadOnlySpan<byte> data, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-60.
        int offset = 0;

        // ID of the resource for this name.
        ResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Name string for the resource.
        byte nameLength = data[offset];
        offset += 1;

        if (offset + nameLength > data.Length)
        {
            throw new ArgumentException($"Invalid data length for ResourceName: {data.Length}", nameof(data));
        }

        Name = Encoding.ASCII.GetString(data.Slice(offset, nameLength));
        offset += nameLength;

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Did not consume all data for ResourceName.");
    }
}
