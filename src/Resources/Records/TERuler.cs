using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A text ruler structure used in text records.
/// </summary>
public readonly struct TERuler
{
    /// <summary>
    /// The size of a TERuler in bytes.
    /// </summary>
    public const int MinSize = 18;

    /// <summary>
    /// Gets the number of pixels to indent from the left edge of the text rectangle
    /// </summary>
    public ushort LeftMargin { get; }

    /// <summary>
    /// Gets the number of pixels to indent from the left edge of the text rectangle
    /// </summary>
    public ushort LeftIndent { get; }

    /// <summary>
    /// Gets the maximum line length, expressed as the number of pixels from the left
    /// </summary>
    public ushort RightMargin { get; }

    /// <summary>
    /// Gets the text justification.
    /// </summary>
    public TextJustification Justification { get; }

    /// <summary>
    /// Gets the line spacing, expressed as the number of pixels to add between lines
    /// </summary>
    public short ExtraLineSpacing { get; }

    /// <summary>
    /// Gets the reserved flags.
    /// </summary>
    public ushort Flags { get; }

    /// <summary>
    /// Gets the application-specific data.
    /// </summary>
    public uint UserData { get; }

    /// <summary>
    /// Gets the type of tab data.
    /// </summary>
    public TabType TabType { get; }

    /// <summary>
    /// Gets the tab stops.
    /// </summary>
    public List<ushort>? TabStops { get; }

    /// <summary>
    /// Gets the tab terminator.
    /// </summary>
    public ushort? TabTerminator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TERuler"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the TERuler.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown if the data is invalid.</exception>
    public TERuler(ReadOnlySpan<byte> data, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length must be at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // 49-39 to 49-40
        int offset = 0;

        // The number of pixels to indent from the left edge of the text rectangle
        // (viewRect in TERecord) for all text lines except those that start
        // paragraphs.
        LeftMargin = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The number of pixels to indentfrom the left edge of the text rectangle
        // for text lines that start paragraphs.
        LeftIndent = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Maximum line length, expressed as the number of pixels from the left
        // edge of the text rectangle.
        RightMargin = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Text justification.
        // 0 Left justification—all text lines start flush with left margin
        // -1 Right justification—all text lines start flush with right margin
        // 1 Center justification—all text lines are centered betweenleft
        // and right margins
        // 2 Full justification—text is blocked flush with both left and
        // right margins; TextEdit pads spaces with extra pixels to
        // justify the text
        Justification = (TextJustification)BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Line spacing, expressed as the number of pixels to add betweenlines
        // of text. Negative values result in text overlap.
        ExtraLineSpacing = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Reserved
        Flags = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Application-specific data.
        UserData = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // The type of tab data, as follows:
        // 0 No tabs are set—tabType is the last field in the structure
        // 1 Regular tabs—tabs are set at regular pixel intervals,
        // specified by the value of the tabTerminator field;
        // the Tabs is omitted from the structure
        // 2 Absolute tabs—tabs are set at absolute, irregular pixel
        // locations; the Tabs defines those locations;
        // tabTerminator marks the end of the Tabs
        TabType = (TabType)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // If tabType is set to 2, this is an array of TabItem Structures defining
        // the absolute pixel positions for the various tab stops. The
        // tabTerminator field, with a value of $FFFF, marks the end of this
        // array. For other values of tabType, this field is omitted from the
        // structure.
        // If tabType is set to 0, this field is omitted from the structure.If
        // tabType is set to 1, then theTabs is omitted, and this field contains
        // the numberof pixels corresponding to the tab interval for the regular
        // tabs. If tabType is set to 2, tabTerminator is set to $FFFF and
        // marks the end of theTabs array.
        if (TabType == TabType.None)
        {
            TabStops = null;
        }
        else
        {
            if (TabType == TabType.Regular)
            {
                if (offset + 2 > data.Length)
                {
                    throw new ArgumentException("Insufficient data for TabStops.", nameof(data));
                }

                TabTerminator = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
                offset += 2;
            }
            else
            {
                var tabStops = new List<ushort>();
                while (offset < data.Length)
                {
                    if (offset + 2 > data.Length)
                    {
                        throw new ArgumentException("Insufficient data for TabStops.", nameof(data));
                    }

                    ushort tabStop = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
                    offset += 2;

                    if (tabStop != 0xFFFF)
                    {
                        tabStops.Add(tabStop);
                    }
                    else
                    {
                        TabTerminator = tabStop;
                        break;
                    }
                }

                TabStops = tabStops;
            }
        }

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Did not consume all data.");
    }
}
