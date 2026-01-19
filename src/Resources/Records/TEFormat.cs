using System.Buffers.Binary;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A text format structure used in text records.
/// </summary>
public readonly struct TEFormat
{
    /// <summary>
    /// The minimum size of a TEFormat in bytes.
    /// </summary>
    public const int MinSize = 14;

    /// <summary>
    /// Gets the version number for TEFormat.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the length, in bytes, of the RulerList.
    /// </summary>
    public uint RulerListLength { get; }

    /// <summary>
    /// Gets the ruler data for the text record.
    /// </summary>
    public List<TERuler> Rulers { get; }

    /// <summary>
    /// Gets the length, in bytes, of the StyleList.
    /// </summary>
    public uint StyleListLength { get; }

    /// <summary>
    /// Gets the list of all unique styles for the text record.
    /// </summary>
    public List<TEStyle> Styles { get; }

    /// <summary>
    /// Gets the number of StyleItems contained in the Styles.
    /// </summary>
    public uint NumberOfStyles { get; }

    /// <summary>
    /// Gets the array of StyleItems specifying which actual styles apply to which text.
    /// </summary>
    public List<StyleItem> StyleItems { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TEFormat"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the TEFormat.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown if the data is invalid.</exception>
    /// <exception cref="NotSupportedException">Thrown if the TEFormat version is unsupported.</exception>
    public TEFormat(ReadOnlySpan<byte> data, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length must be at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // 49-31 to 49-32
        int offset = 0;

        // Version number corresponding to the layout of this TEFormat
        // structure. The number of this version of the structure is $0000.
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Version != 0)
        {
            throw new NotSupportedException($"Unsupported TEFormat version: {Version}.");
        }

        // The length, in bytes, of the RulerList.
        RulerListLength = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (offset + RulerListLength > data.Length)
        {
            throw new ArgumentException("Insufficient data for Rulers.", nameof(data));
        }

        // Ruler data for the text record. The TERuler structure is embedded in
        // the TEFormat Structure at this location.
        ReadOnlySpan<byte> rulerData = data.Slice(offset, (int)RulerListLength);
        int rulerDataOffset = 0;

        var rulers = new List<TERuler>();
        while (rulerDataOffset < rulerData.Length)
        {
            // Seen padding bytes at the end of the ruler data
            if (rulerDataOffset + TERuler.MinSize > rulerData.Length)
            {
                break;
            }

            rulers.Add(new TERuler(rulerData[rulerDataOffset..], out var rulerBytesRead));
            rulerDataOffset += rulerBytesRead;
        }

        Debug.Assert(rulerDataOffset <= rulerData.Length, "Did not consume all ruler data.");
        Rulers = rulers;

        offset += (int)RulerListLength;

        // The length, in bytes, of theStyleList.
        StyleListLength = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (StyleListLength % TEStyle.Size != 0)
        {
            throw new ArgumentException("Style list length is not a multiple of TEStyle size.", nameof(data));
        }
        if (offset + StyleListLength > data.Length)
        {
            throw new ArgumentException("Insufficient data for Styles.", nameof(data));
        }

        // List of all unique styles for the text record. The TEStyle structures
        // are embedded in the TEFormat structure at this location. Each
        // TEStyle structure must define a unique style—there must be no
        // duplicate style entries. To apply the same style to multiple blocks of
        // text, you should create additional styleItems for each block of
        // text and make each item refer to the same style in this array.
        ReadOnlySpan<byte> styleData = data.Slice(offset, (int)StyleListLength);
        int styleDataOffset = 0;

        var styleListCount = (int)(StyleListLength / TEStyle.Size);

        var styles = new List<TEStyle>(styleListCount);
        while (styleDataOffset < styleData.Length)
        {
            styles.Add(new TEStyle(styleData.Slice(styleDataOffset, TEStyle.Size)));
            styleDataOffset += TEStyle.Size;
        }

        Debug.Assert(styleDataOffset == styleData.Length, "Did not consume all style data.");
        Styles = styles;

        offset += (int)StyleListLength;

        // The number of StyleItems contained in the Styles.
        NumberOfStyles = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (NumberOfStyles * StyleItem.Size + offset > data.Length)
        {
            throw new ArgumentException("Insufficient data for StyleItems.", nameof(data));
        }

        // Array of StyleItems specifying which actual styles (stored in
        // theStyleList) apply to which text within the TextEdit record.
        var styleItems = new List<StyleItem>((int)NumberOfStyles);
        for (int i = 0; i < NumberOfStyles; i++)
        {
            styleItems.Add(new StyleItem(data.Slice(offset, StyleItem.Size)));
            offset += StyleItem.Size;
        }

        StyleItems = styleItems;

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Did not consume all data.");
    }
}
