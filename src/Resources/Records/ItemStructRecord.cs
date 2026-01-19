using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Item Struct Record
/// </summary>
public readonly struct ItemStructRecord
{
    /// <summary>
    /// Size of an Item Struct Record in bytes.
    /// </summary>
    public const int Size = 10;

    /// <summary>
    /// Gets the Item Struct Flags.
    /// </summary>
    public ItemStructFlags Flags { get; }

    /// <summary>
    /// Gets the reference to the title (name) of the item.
    /// </summary>
    public uint TitleReference { get; }

    /// <summary>
    /// Gets the reference to the icon data structure.
    /// </summary>
    public uint IconReference { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemStructRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw record data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ItemStructRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid data length for ItemStructRecord: expected {Size}, got {data.Length}", nameof(data));
        }

        // Structure documented in https://apple2.gs/downloads/library/Apple%20IIgs%20Toolbox%20Changes%20for%20System%20Software%206.0.pdf
        // p49 to p50
        int offset = 0;

        // bit 15 Indicates whether or not there is an icon associated
        // with the menu item.
        // 0 = No icon
        // 1 = There is an icon
        // bits 14-2 Reserved. Must be set to 0. In the future these bits will
        // define additonal fields that may be added to this record.
        // bits 1-0 Defines how the icon is referenced
        // 00 = Reference is by pointer
        // 01 = Reference is by handle
        // 10 = Reference is by resource ID
        // 11 = Invalid value
        Flags = new ItemStructFlags(data.Slice(offset, ItemStructFlags.Size));
        offset += ItemStructFlags.Size;

        // Since the reference to the itemStruct record is now stored in
        // the itemName field of the item record, the reference to the item’s
        // name has been moved here. The bits that normally define how this
        // field will be referenced are still controlled in the itemFlag field
        // of the item record.
        TitleReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // This is the reference to the icon data structure. The structure itself
        // is defined in Appendix E, page 48, of the Toolbox Reference Vol.
        // 3. 
        IconReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all data for ItemStructRecord");
    }
}
