using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// The Window Parameter 1 record structure in a GS/OS Resource Fork.
/// </summary>
public readonly struct WindowParam1Record
{
    /// <summary>
    /// The minimum size of Window Parameter 1 record structure in bytes.
    /// </summary>
    public const int MinSize = 80;

    /// <summary>
    /// Gets the length of the template.
    /// </summary>
    public ushort Length { get; }

    /// <summary>
    /// Gets the frame bits.
    /// </summary>
    public ushort Frame { get; }

    /// <summary>
    /// Gets the title reference.
    /// </summary>
    public uint TitleReference { get; }

    /// <summary>
    /// Gets the reference constant.
    /// </summary>
    public uint ReferenceConstant { get; }

    /// <summary>
    /// Gets the zoom rectangle.
    /// </summary>
    public RECT ZoomRect { get; }

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint ColorTableReference { get; }

    /// <summary>
    /// Gets the origin X coordinate.
    /// </summary>
    public ushort OriginX { get; }

    /// <summary>
    /// Gets the origin Y coordinate.
    /// </summary>
    public ushort OriginY { get; }

    /// <summary>
    /// Gets the data height.
    /// </summary>
    public ushort DataHeight { get; }

    /// <summary>
    /// Gets the data width.
    /// </summary>
    public ushort DataWidth { get; }

    /// <summary>
    /// Gets the maximum height.
    /// </summary>
    public ushort MaxHeight { get; }

    /// <summary>
    /// Gets the maximum width.
    /// </summary>
    public ushort MaxWidth { get; }

    /// <summary>
    /// Gets the vertical scroll value.
    /// </summary>
    public ushort ScrollVertical { get; }

    /// <summary>
    /// Gets the horizontal scroll value.
    /// </summary>
    public ushort ScrollHorizontal { get; }

    /// <summary>
    /// Gets the vertical page value.
    /// </summary>
    public ushort PageVertical { get; }

    /// <summary>
    /// Gets the horizontal page value.
    /// </summary>
    public ushort PageHorizontal { get; }

    /// <summary>
    /// Gets the info text reference.
    /// </summary>
    public uint InfoTextReference { get; }

    /// <summary>
    /// Gets the info text height.
    /// </summary>
    public ushort InfoTextHeight { get; }

    /// <summary>
    /// Gets the frame definition procedure.
    /// </summary>
    public uint FrameDefinitionProcedure { get; }

    /// <summary>
    /// Gets the info text definition procedure.
    /// </summary>
    public uint InfoTextDefinitionProcedure { get; }

    /// <summary>
    /// Gets the content definition procedure.
    /// </summary>
    public uint ContentDefinitionProcedure { get; }

    /// <summary>
    /// Gets the position rectangle.
    /// </summary>
    public RECT Position { get; }

    /// <summary>
    /// Gets the plane.
    /// </summary>
    public uint Plane { get; }

    /// <summary>
    /// Gets the control template reference.
    /// </summary>
    public uint ControlTemplateReference { get; }

    /// <summary>
    /// Gets the reference types.
    /// </summary>
    public WindowParam1ReferenceTypes ReferenceTypes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowParam1Record"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Window Parameter 1 record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public WindowParam1Record(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-74 to 
        int offset = 0;

        // The numberof bytes in the template, including the length of
        // p1Length. Must be set to $50.
        Length = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Length != 0x50)
        {
            throw new ArgumentException($"Invalid WindowParam1Record length: {Length}", nameof(data));
        }

        // See NewWindow wFrameBits parameter
        Frame = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Reference to title string for the window. The contents of p1InDesc
        // specify the type of reference stored here. The title must be stored in a
        // Pascal string containing both a leading and trailing space.
        // If p1Title is set to NIL, the Window Manager creates a window
        // without a title bar. If your program is creating a window with title
        // bar, you must specify a title of some sort. To create a window without
        // a title, make p1Title (or fitlePtron the NewWindow2call) refer to a
        // null string.
        // Note that the Window Manager creates a copy of the title string,
        // allowing your program to free the memory occupied by this string
        // after the NewWindowz2call is issued.
        // If you specify a non-NIL value fortitlePtr on the NewWindow?2call,
        // this field is ignored.
        TitleReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // See NewWindow wRefCon parameter
        ReferenceConstant = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // See NewWindow wZoom parameter
        ZoomRect = new RECT(data.Slice(offset, RECT.Size));
        offset += RECT.Size;

        // Reference to the color table for the window. The contents of
        // p1InDesc specify the type of reference stored here. If
        // p1ColorTable is set to NIL, the Window Manager assumes that
        // there is no color table for the window.
        // The format of the color table is defined in Chapter 25, “Window
        // Manager,” in Volume 2 of the Toolbox Reference. If p1ColorTable
        // refers to a resource, then the color table must be defined in a resource
        // of type rWindColor.
        ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // See NewWindow wYOrigin parameter
        OriginX = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wXOrigin parameter
        OriginY = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wDataH parameter
        DataHeight = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wDataw parameter
        DataWidth = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wMaxH parameter
        MaxHeight = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wMaxW parameter
        MaxWidth = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;
        
        // See NewWindow wScrollVer parameter
        ScrollVertical = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wScrollHor parameter
        ScrollHorizontal = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wPageVer parameter
        PageVertical = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wPageHor parameter
        PageHorizontal = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wInfoRefCon parameter
        InfoTextReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // See NewWindow wInfoHeight parameter
        InfoTextHeight = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // See NewWindow wFrameDefProc parameter
        FrameDefinitionProcedure = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // See NewWindow wInfoDefProc parameter
        InfoTextDefinitionProcedure = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // See NewWindow wContDefProc parameter
        ContentDefinitionProcedure = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // See NewWindow wPosition parameter
        Position = new RECT(data.Slice(offset, RECT.Size));
        offset += RECT.Size;

        // See NewWindow wPlane parameter
        Plane = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Reference to the template or templates defining controls for the window. The Window Managerpasses this value to the NewControl2 Control Manager tool call as the reference parameter. Note that pliInDesc contains the data for the NewControl2 referenceDesc
        // parameter. Refer to Chapter 28, “Control Manager Update,” in this
        // book for more information about NewControl2.
        // If this field is set to NIL, then the Window Managerassumesthat there
        // is no controllist for the window and doesnotcall NewControl2.
        ControlTemplateReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // The type of reference stored in p1ColorTable and p1Title. This
        // field also contains the referenceDesc value for NewControl2 that
        // defines the contents of p1ControlList.
        // Reserved bits 15-12 Must be set to 0.
        // colorTableRef bits 11-10 Defines the type of reference stored in
        // p1ColorTable.
        // 00 = Reference is by pointer to color table
        // 01 = Reference is by handle to colortable
        // 10 = Reference is by resource ID of rWwindColor
        // resource
        // 11 = Invalid value
        // titleRef bits 9-8 Defines the type of reference stored in piTitle.
        // 00 = Reference is by pointer to Pascal string
        // 01 = Reference is by handleto Pascalstring
        // 10 = Referenceis by resource ID of rpPString
        // resource
        // 11 = Invalid value
        // controlRef bits 7-0 Defines the type of reference stored in
        // p1ControlList; passed directly to the
        // NewControl2 Control Manager tool call as the
        // referenceDesc parameter. (For valid values, see the
        // description of the NewControl2 tool call in
        // Chapter 28, “Control Manager Update,” earlier in this
        // book.)
        ReferenceTypes = new WindowParam1ReferenceTypes(data.Slice(offset, WindowParam1ReferenceTypes.Size));
        offset += WindowParam1ReferenceTypes.Size;

        Debug.Assert(offset <= data.Length, "Did not consume all data for WindowParam1Record.");
    }
}
