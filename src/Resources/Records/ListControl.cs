using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// List control member record.
/// </summary>
public readonly struct ListControl
{
    /// <summary>
    /// Minimum size of ListControl structure in bytes.
    /// </summary>
    public const int MinSize = 5;

    /// <summary>
    /// Gets the resource ID of the list member.
    /// </summary>
    public uint ResourceID { get; }

    /// <summary>
    /// Gets the control flags for the member.
    /// </summary>
    public ListControlFlags Flags { get; }

    /// <summary>
    /// Gets the application-specific data for the list member.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListControl"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the ListControl record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ListControl(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-51
        int offset = 0;

        // Resource ID of the list member (resource type of rPtring, $8006).
        ResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Control flags for the member.
        // memSelect Bits 6-7 Indicates whether the item is selected
        // 00 = Item is enabled but not selected
        // 01 = Item is disabled (cannot be selected)
        // 10 = Item is selected
        // 11 = Invalid value
        // Reserved Bits 0-5 Must be set to 0
        Flags = new ListControlFlags(data.Slice(offset, ListControlFlags.Size));
        offset += ListControlFlags.Size;

        // Application-specific data for the list member. The ListMemSize
        // field of the list control template specifies the size of this field, plus 5.
        // For example, to assign a 2-byte tag to each list member, you would set
        // listMemSize to 7 (2+5) and place the tag value at item in each list
        // member.
        Data = data[offset..].ToArray();
        offset += Data.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for ListControl.");
    }
}
