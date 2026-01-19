using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Menu bar record.
/// </summary>
public readonly struct MenuBarRecord
{
    /// <summary>
    /// Minimum size of MenuBarRecord structure in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the version number of the menu bar.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the flags for the menu bar.
    /// </summary>
    public MenuBarFlags Flags { get; }

    /// <summary>
    /// Gets the array of menu references.
    /// </summary>
    public List<uint> MenuReferences { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuBarRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the MenuBarRecord.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public MenuBarRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-55
        int offset = 0;

        // The version of the menu bar template. The Menu Manager uses this
        // field to distinguish between different revisions of the template. Must
        // be set to 0.
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Version != 0)
        {
            throw new ArgumentException($"Unsupported MenuBarRecord version: {Version}", nameof(data));
        }

        // Bit flags controlling the display and processing attributes of the menu
        // bar. Valid values for menuBarFlag are
        // menuRefType bits 15-14 Defines the type of reference in each entry of
        // menuRefArray (all array entries must be of the same
        // type).
        // 00 = Reference is by pointer
        // 01 = Reference is by handle
        // 10 = Reference is by resource ID
        // 11 = Invalid value
        // Reserved bits 13-0 Must be set to 0.
        Flags = new MenuBarFlags(data.Slice(offset, MenuBarFlags.Size));
        offset += MenuBarFlags.Size;

        // Array of references to the menus in the menu bar. The menuRefType
        // bits in menuBarFlag indicate whether the entries in the array are
        // pointers, handles, or resource IDs. Note that all array entries must be
        // of the same reference type. The last entry in the array must be set to
        // $00000000.
        var menusReference = new List<uint>();
        while (offset < data.Length)
        {
            uint menuRef = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            if (menuRef == 0)
            {
                break;
            }

            menusReference.Add(menuRef);
            offset += 4;
        }

        MenuReferences = menusReference;

        Debug.Assert(offset <= data.Length, "Did not consume all data for MenuBarRecord.");
    }
}
