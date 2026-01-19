using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// List Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct ListControlTemplate
{
    /// <summary>
    /// Minimum size of List Control Template in bytes.
    /// </summary>
    public const int MinSize = 20;

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public ushort NumberOfItems { get; }

    /// <summary>
    /// Gets the number of visible items in the list.
    /// </summary>
    public ushort NumberOfVisibleItems { get; }

    /// <summary>
    /// Gets the type of the list.
    /// </summary>
    public ListControlType Type { get; }

    /// <summary>
    /// Gets the first visible item index.
    /// </summary>
    public ushort FirstVisibleItem { get; }

    /// <summary>
    /// Gets the item drawing routine.
    /// </summary>
    public uint ItemDrawingRoutine { get; }

    /// <summary>
    /// Gets the item height.
    /// </summary>
    public ushort ItemHeight { get; }

    /// <summary>
    /// Gets the item size.
    /// </summary>
    public ushort ItemSize { get; }

    /// <summary>
    /// Gets the items reference.
    /// </summary>
    public uint ItemsReference { get; }

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the List Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ListControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.List)
        {
            throw new ArgumentException($"Invalid control procedure for List: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 14 || header.ParameterCount > 15)
        {
            throw new ArgumentException($"Invalid parameter count for List: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-23 to E-25
        int offset = 0;

        NumberOfItems = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        NumberOfVisibleItems = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Valid values for listType are as follows:
        // Reserved bits 15-3 Must be set to 0.
        // fListScrollBar bit 2 Allows you to control where the scroll bar for the list is
        // drawn.
        // 0 = Scroll bar drawn on outside of boundary rectangle
        // 1 = Scroll bar drawn on inside of boundary rectangle;
        // the List Manager calculates space needed, adjusts
        // dimensions of boundary rectangle, and resets this flag
        // fListSelect bit 1 Controls type of selection options available to the user.
        // 0 = Arbitrary and range selection allowed
        // 1 = Only single selection allowed
        // fListString bit 0 Defines the type of strings used to define list items.
        // 0 = Pascal strings
        // 1 = C strings ($00-terminated)
        Type = (ListControlType)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        FirstVisibleItem = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        ItemDrawingRoutine = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        ItemHeight = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        ItemSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        ItemsReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (header.ParameterCount >= 15)
        {
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Read beyond end of data.");
    }
}
