using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Pop-Up Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct PopUpControlTemplate
{
    /// <summary>
    /// Minimum size of Pop-Up Control Template in bytes.
    /// </summary>
    public const int MinSize = 6;

    /// <summary>
    /// Gets the title width.
    /// </summary>
    public ushort TitleWidth { get; }

    /// <summary>
    /// Gets the menu reference.
    /// </summary>
    public uint MenuReference { get; }

    /// <summary>
    /// Gets the initial item ID.
    /// </summary>
    public ushort InitialItemID { get; }

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PopUpControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Pop-Up Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public PopUpControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.PopUp)
        {
            throw new ArgumentException($"Invalid control procedure for PopUp: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 9 || header.ParameterCount > 10)
        {
            throw new ArgumentException($"Invalid parameter count for PopUp: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-28 to E-31
        int offset = 0;

        // A parameter providing additional control over placement of the menu
        // on the screen. The tit lewidth field defines an offset from the left
        // edge of the control (boundary) rectangle to the left edge of the popup rectangle. If you are creating a series of pop-up menus and you
        // want to align them vertically, give all menus the same x] coordinate
        // and titlewidth value. You may use titleWidth for this even if
        // you are not going to display the title (fDontDrawTit1eflagis set to
        // 1 in flag). If you set titleWidth to 0, then the Menu Manager
        // determines its value according to the length of the menu title, and the
        // pop-up rectangle immediately follows the title string. If the width of
        // your title exceeds the value of titleWidth, results are
        // unpredictable.
        TitleWidth = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Reference to menu definition (see Chapter 13, “Menu Manager,” in
        // Volume 1 of the Toolbox Reference and Chapter 37, “Menu Manager
        // Update,” in this book for details on menu templates). The type of
        // reference contained in menuRef is defined by the menu reference bits
        // in moreFlags.
        MenuReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // The initial value to be displayed for the menu.The initial value is the
        // default value for the menu and is displayed in the pop-up rectangle of
        // unselected menus. You specify an item by its ID, that is, its relative
        // position within the array of items for the menu (see Chapter 37, “Menu
        // Manager Update,” for information on the layout and content of the
        // pop-up menu template). If you pass an invalid item ID, then no item is
        // displayed in the pop-uprectangle.
        InitialItemID = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (header.ParameterCount >= 10)
        {
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        bytesRead = offset;
        Debug.Assert(bytesRead <= data.Length, "Read beyond end of data.");
    }
}
