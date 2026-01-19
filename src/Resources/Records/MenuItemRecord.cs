using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A menu item record in a GS/OS Resource Fork.
/// </summary>
public readonly struct MenuItemRecord
{
    /// <summary>
    /// The minimum size of Menu Item record structure in bytes.
    /// </summary>
    public const int MinSize = 14;

    /// <summary>
    /// Gets the version number of the menu item.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the menu item ID.
    /// </summary>
    public ushort MenuItemID { get; }

    /// <summary>
    /// Gets the primary keystroke equivalent character.
    /// </summary>
    public byte PrimaryKeystrokeEquivalentCharacter { get; }

    /// <summary>
    /// Gets the alternate keystroke equivalent character.
    /// </summary>
    public byte AlternateKeystrokeEquivalentCharacter { get; }

    /// <summary>
    /// Gets the item checkmark character.
    /// </summary>
    public char ItemCheckmarkCharacter { get; }

    /// <summary>
    /// Gets the menu item flags.
    /// </summary>
    public MenuItemFlags Flags { get; }

    /// <summary>
    /// Gets the title reference.
    /// </summary>
    public uint TitleReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Menu Item record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public MenuItemRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-56 to E-57
        int offset = 0;

        // The version of the menu item template. The Menu Managerusesthis
        // field to distinguish between different revisions of the menu item
        // template. Must be set to 0.
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Unique identifier for the menu item. See Chapter 13, “Menu Manager,”
        // in Volume 1 of the Toolbox Reference for information on valid values
        // for itemID.
        MenuItemID = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Fields defining the keystroke equivalents for the menu item. The user
        // can select the menu item by pressing the Commandkeyalong with the
        // key corresponding to oneofthesefields. Typically, these fields
        // contain the uppercase and lowercase ASCII codes for a particular
        // character. If you have only a single key equivalence, set both fields to
        // that value.
        PrimaryKeystrokeEquivalentCharacter = data[offset];
        offset += 1;

        AlternateKeystrokeEquivalentCharacter = data[offset];
        offset += 1;

        // The character to be displayed next to the item whenit is checked.
        ItemCheckmarkCharacter = (char)data[offset];
        offset += 1;

        // Bit flags controlling the display attributes of the menu item. Valid
        // values for itemFlag are
        // titleRefType bits 15-14 Defines the type of reference in itemTitleRef.
        // 00 = Reference is by pointer
        // 01 = Reference is by handle
        // 10 = Reference is by resource ID
        // 11 = Invalid value
        // Reserved bit 13 Must be set to 0.
        // shadow bit 12 Indicates item shadowing.
        // 0 = No shadow
        // 1 = Shadow
        // outline bit 11 Indicates item outlining.
        // 0 = Not outlined
        // 1 = Outlined
        // Reserved bits 10-8 Must be set to 0.
        // disabled bit 7 Enables or disables the menu item.
        // 0 = Item enabled
        // 1 = Item disabled
        // divider bit 6 Controls drawing of a divider bar below item.
        // 0 = No divider bar
        // 1 = Divider bar
        // XOR bit 5 Controls how highlighting is performed.
        // 0 = Do not use XOR to highlight item
        // 1 = Use XO to highlight item
        // Reserved bits 4-3 Must be set to 0.
        // underline bit 2 Controls item underlining.
        // 0 = Do not underline item
        // 1 = Underline item
        // italic bit 1 Indicates whether item is italicized.
        // 0 = Not italicized
        // 1 = Italicized
        // bold bit 0 Indicates whetheritem is in boldface.
        // 0 = Not bold
        // 1 = Bold
        Flags = new MenuItemFlags(data.Slice(offset, MenuItemFlags.Size));
        offset += MenuItemFlags.Size;

        // Reference to title string of menu item. The titleRefType bits in
        // itemFlag indicate whether itemTitleRef contains a pointer, a
        // handle, or a resource ID. If itemTitleRef is a pointer, then the title
        // string must be a Pascal string. Otherwise, the Menu Manager can
        // retrieve the string length from control information in the handle.
        TitleReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset <= data.Length, "Did not consume all data for MenuItemRecord.");
    }
}
