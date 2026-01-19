using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A Menu record in a GS/OS Resource Fork.
/// </summary>
public readonly struct MenuRecord
{
    /// <summary>
    /// The minimum size of Menu record structure in bytes.
    /// </summary>
    public const int MinSize = 14;

    /// <summary>
    /// Gets the version number of the menu.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the menu ID.
    /// </summary>
    public ushort ID { get; }

    /// <summary>
    /// Gets the menu flags.
    /// </summary>
    public MenuFlags Flags { get; }

    /// <summary>
    /// Gets the title reference.
    /// </summary>
    public uint TitleReference { get; }

    /// <summary>
    /// Gets the item reference.
    /// </summary>
    public List<uint> ItemReferences { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Menu record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public MenuRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-52 to E-54
        int offset = 0;

        // The version of the menu template. The Menu Managerusesthisfield to
        // distinguish between different revisions of the template. Must be set to 0.
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Version != 0)
        {
            throw new ArgumentException($"Unsupported Menu record version: {Version}", nameof(data));
        }

        // Unique identifier for the menu. See Chapter 13, “Menu Manager,” in
        // Volume 1 of the Toolbox Reference for information on valid values for
        // menulID.
        ID = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Bit flags controlling the display and processing attributes of the menu.
        // Valid values for menuFlag are
        // titleRefType bits 15-14 Defines the type of reference in menuTitleRef.
        // 00 = Reference is by pointer
        // 01 = Reference is by handle
        // 10 = Reference is by resource ID
        // 11 = Invalid value
        // itemRefType bits 13-12 Defines the type of reference in each entry of
        // itemRefArray (all array entries must be of the same
        // type).
        // 00 = Reference is by pointer
        // 01 = Reference is by handle
        // 10 = Referenceis by resource ID
        // 11 = Invalid value
        // Reserved bits 11-9 Must beset to 0.
        // alwaysCallmChoose
        // bit 8 Causes the Menu Managerto call a custom menu
        // defProc mChooseroutine even whenthe pointeris
        // not in the menu rectangle (supports tear-off menus).
        // 0 = Do not always call mchoose routine
        // 1 = Always call mchooseroutine
        // disabled bit 7 Enablesor disables the menu.
        // 0 = Menu enabled
        // 1 = Menudisabled
        // Reserved bit 6 Mustbesetto 0.
        // XOR bit 5 Controls how selection highlighting is performed.
        // 0 = Do not use XORto highlight item
        // 1 = Use XORto highlight item
        // custom bit 4 Indicates whether menu is custom or standard.
        // 0 = Standard menu
        // 1 = Custom menu
        // allowCache bit 3 Controls menu caching.
        // 0 = No menucachingallowed
        // 1 = Menucaching allowed
        // Reserved bits 2~0 Must besetto 0.
        Flags = new MenuFlags(data.Slice(offset, MenuFlags.Size));
        offset += MenuFlags.Size;

        // Reference to title string of menu. The titleRefType bits in
        // menuFlag indicate whether menuTitleRef contains a pointer, a
        // handle, or a resource ID. If menuTitleRef is a pointer, then the title
        // string must be a Pascal string. Otherwise, the Menu Manager can
        // retrieve the string length from control information in the handle.
        TitleReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Array of references to the items in the menu. The itemRefType bits
        // in menuFlag indicate whether the entries in the array are pointers,
        // handles, or resource IDs. Note that all array entries must be of the
        // same reference type. The last entry in the array must be set to
        // $00000000.
        var itemReferences = new List<uint>();
        while (offset < data.Length)
        {
            if (offset + 4 > data.Length)
            {
                throw new ArgumentException("Invalid data length while reading item references.", nameof(data));
            }

            uint itemRef = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            if (itemRef == 0)
            {
                break;
            }

            itemReferences.Add(itemRef);
            offset += 4;
        }

        ItemReferences = itemReferences;

        Debug.Assert(offset <= data.Length, "Read beyond end of data.");
    }
}
