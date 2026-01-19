using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Text Edit Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct TextEditControlTemplate
{
    /// <summary>
    /// Minimum size of Text Edit Control Template in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the text edit flags.
    /// </summary>
    public TextEditFlags Flags { get; }

    /// <summary>
    /// Gets the indent rectangle.
    /// </summary>
    public RECT? IndentRect { get; }

    /// <summary>
    /// Gets the vertical scroll bar handle.
    /// </summary>
    public uint? VerticalScrollBarHandle { get; }

    /// <summary>
    /// Gets the vertical scroll amount.
    /// </summary>
    public ushort? VerticalScrollAmount { get; }

    /// <summary>
    /// Gets the horizontal scroll bar handle.
    /// </summary>
    public uint? HorizontalScrollBarHandle { get; }

    /// <summary>
    /// Gets the horizontal scroll amount.
    /// </summary>
    public ushort? HorizontalScrollAmount { get; }

    /// <summary>
    /// Gets the style reference.
    /// </summary>
    public uint? StyleReference { get; }

    /// <summary>
    /// Gets the text descriptor.
    /// </summary>
    public ushort? TextDescriptor { get; }

    /// <summary>
    /// Gets the initial text reference.
    /// </summary>
    public uint? InitialTextReference { get; }

    /// <summary>
    /// Gets the initial text length.
    /// </summary>
    public uint? InitialTextLength { get; }

    /// <summary>
    /// Gets the maximum text length.
    /// </summary>
    public uint? MaximumTextLength { get; }

    /// <summary>
    /// Gets the maximum lines.
    /// </summary>
    public uint? MaximumLines { get; }

    /// <summary>
    /// Gets the maximum characters per line.
    /// </summary>
    public ushort? MaximumCharactersPerLine { get; }

    /// <summary>
    /// Gets the maximum height.
    /// </summary>
    public ushort? MaximumHeight { get; }

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Gets the draw mode.
    /// </summary>
    public ushort? DrawMode { get; }

    /// <summary>
    /// Gets the filter procedure.
    /// </summary>
    public uint? FilterProcedure { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextEditControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Text Edit Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public TextEditControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.TextEdit)
        {
            throw new ArgumentException($"Invalid control procedure for TextEdit: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 7 || header.ParameterCount > 23)
        {
            throw new ArgumentException($"Invalid parameter count for TextEdit: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-40 to E-43
        int offset = 0;

        // Valid values for textFlags are
        // fNotControl bit 31 Must be set to 0
        // fSingleFormat bit 30 Must be set to 1
        // fSingleStyle bit 29 Allows you to restrict the style options available to
        // the user:
        // 1 = Allow only one style in the text
        // 0 = Do not restrict the number of styles in the text
        // fNoWordWrap bit 28 Allows you to control TextEdit word wrap behavior:
        // 1 = Do not word wrap the text; only break lines on CR
        // ($0D) characters
        // 0 = Perform word wrap to fit the ruler
        // fNoScroll bit 27 Controls user access to scrolling:
        // 1 = Do not allow either manual or auto-scrolling
        // 0 = Scrolling permitted
        // fReadOnly bit 26 Restricts the text in the window to read-only
        // operations (copying from the window will still be
        // allowed):
        // 1 = No editing allowed
        // 0 = Editing permitted
        // fSmartCutPaste bit 25 Controls TextEdit support for smart cut and paste
        // see Chapter 49, "TextEdit," for details on smart cut
        // and paste support):
        // 1 = Use smart cut and paste
        // 0 = Do not use smart cut and paste
        // fTabSwitch bit 24 Defines behavior of the Tab key (see
        // Chapter 49, "TextEdit," for details):
        // 1 = Tab to next control in the window
        // 0 = Tab inserted in TextEdit document
        // fDrawBounds bit 23 Tells TextEdit whether to draw a box around the edit
        // window, just inside rect; the pen for this box is two
        // pixels wide and one pixel high
        // 1 = Draw rectangle
        // 0 = Do not draw rectangle
        // fColorHilight bit 22
        // fGrowRuler bit 21 Tells TextEdit whether to resize the ruler in response
        // to the user resizing the edit window; if set to 1,
        // TextEdit will automatically adjust the right margin
        // value for the ruler:
        // 1 = Resize the ruler
        // 0 = Do not resize the ruler
        // fDisableSelection bit 20 Controls whether user can select text:
        // 1 = User cannot select text
        // 0 = User can select text
        // fDrawInactiveSelection bit 19 Controls how inactive selected text is displayed:
        // 1 = TextEdit draws a box around inactive selections
        // 0 = TextEdit does not display inactive selections
        // Reserved bits 0-18 Must be set to 0
        Flags = (TextEditFlags)BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (header.ParameterCount >= 8)
        {
            // A rectangle whose coordinates specify the amount, in pixels, of white
            // space to leave between the boundary rectangle for the control and the
            // text itself. Default values are (2,6,2,4) in 640 mode and (2,4,2,2) in 320
            // mode. Each indentation coordinate may be specified individually. To
            // assert the default for any coordinate, specify its value as $FFFF.
            IndentRect = new RECT(data.Slice(offset, RECT.Size));
            offset += RECT.Size;
        }
        else
        {
            IndentRect = null;
        }

        if (header.ParameterCount >= 9)
        {
            // Handle of the vertical scroll bar to use for the TextEdit window.If you
            // do not want a scroll bar at all, then set this field to NIL. If you want
            // TextEdit to create a scroll bar, just inside the right edge of the
            // boundary rectangle for the control, then set this field to $FFFFFFFF.
            VerticalScrollBarHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            VerticalScrollBarHandle = null;
        }

        if (header.ParameterCount >= 10)
        {
            // The number of pixels to scroll whenever the user presses the up or
            // down arrow on the vertical scroll bar. To use the default value (9
            // pixels), set this field to $0000.
            VerticalScrollAmount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            VerticalScrollAmount = null;
        }

        if (header.ParameterCount >= 11)
        {
            // Must be set to NIL.
            HorizontalScrollBarHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            HorizontalScrollBarHandle = null;
        }

        if (header.ParameterCount >= 12)
        {
            // Must be set to 0.
            HorizontalScrollAmount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            HorizontalScrollAmount = null;
        }

        if (header.ParameterCount >= 13)
        {
            // Reference to initial style information for the text. See the description
            // of the TEFormat record in Chapter 49, “TextEdit Tool Set,” for
            // information about the format and content of a style descriptor. Bits 1
            // and 0 of moreF lags define the type of reference (pointer, handle,
            // resource ID). To use the default style and ruler information,set this
            // field to NIL.
            StyleReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            StyleReference = null;
        }

        if (header.ParameterCount >= 14)
        {
            // Input text descriptor that defines the reference type for theinitial
            // text (which is defined in the textRef field) and the format of that
            // text. See Chapter 49, “TextEdit ToolSet,” for detailed information on
            // text and reference formats.
            TextDescriptor = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            TextDescriptor = null;
        }

        if (header.ParameterCount >= 15)
        {
            // Reference to initial text for the edit window.If you are not supplying
            // anyinitial text, then set this field to NIL.
            InitialTextReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            InitialTextReference = null;
        }

        if (header.ParameterCount >= 16)
        {
            // The length of the initial text. If text Ref is a pointer to the initial
            // text, then this field must contain the length oftheinitial text. For
            // other reference types, TextEdit extracts the length from the reference
            // itself.
            InitialTextLength = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            InitialTextLength = null;
        }

        if (header.ParameterCount >= 17)
        {
            // Maximum numberofcharacters allowed in the text. If you do not want
            // to define any limit to the number of characters, then set this field to NIL.
            MaximumTextLength = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            MaximumTextLength = null;
        }

        if (header.ParameterCount >= 18)
        {
            // Must be set to 0.
            MaximumLines = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            MaximumLines = null;
        }

        if (header.ParameterCount >= 19)
        {
            // Must be set to NIL.
            MaximumCharactersPerLine = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            MaximumCharactersPerLine = null;
        }

        if (header.ParameterCount >= 20)
        {
            // Must be set to 0.
            MaximumHeight = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            MaximumHeight = null;
        }

        if (header.ParameterCount >= 21)
        {
            // Reference to the color table for the text. This is a TextEdit color table
            // (see Chapter 49, “TextEdit Tool Set,” for the format and content of
            // TEColorTable), Bits 2 and 3 of moreFlags define the type of
            // reference stored here.
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        if (header.ParameterCount >= 22)
        {
            // The text mode used by QuickDraw II for drawing text. See
            // Chapter 16, “QuickDrawII,” in Volume 2 of the Toolbox Reference for
            // details on valid text modes.
            DrawMode = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            DrawMode = null;
        }

        if (header.ParameterCount >= 23)
        {
            // Pointer to a filter routine for the control. See Chapter 49,
            // “TextEdit Tool Set,” for details on TextEdit generic filter routines. If you
            // do not want to use a filter routine for the control, set this field to NIL.
            FilterProcedure = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            FilterProcedure = null;
        }

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Read beyond end of data.");
    }
}
