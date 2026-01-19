using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Static Text within a Control Template Record
/// </summary>
public readonly struct StaticTextControlTemplate
{
    /// <summary>
    /// Minimum size of Static Text Control Template in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the text reference.
    /// </summary>
    public uint TextReference { get; }

    /// <summary>
    /// Gets the text size.
    /// </summary>
    public ushort? TextSize { get; }

    /// <summary>
    /// Gets the initial justification.
    /// </summary>
    public TextJustification? InitialJustification { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticTextControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Static Text Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public StaticTextControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.StaticText)
        {
            throw new ArgumentException($"Invalid control procedure for StaticText: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 7 || header.ParameterCount > 9)
        {
            throw new ArgumentException($"Invalid parameter count for StaticText: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-38 to E-39
        int offset = 0;

        TextReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (header.ParameterCount >= 8)
        {
            // The size of the referenced text in characters, but onlyif the text
            // reference in text Ref is a pointer. If the text referenceis either a
            // handle or a resource ID, then the Control Managercan extract the
            // length from the handle.
            TextSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            TextSize = null;
        }

        if (header.ParameterCount >= 9)
        {
            // The justification word passed to LEText Box2 (see Chapter10,
            // “LineEdit Tool Set,” in Volume 1 of the Toolbox Referencefor details
            // on the LETextBox2 toolcall) and used to set the initial justification
            // for the text being drawn. Valid values for just are
            // leftJustify 0 Text is left justified in the display window
            // centerJustify 1 Text is centered in the display window
            // rightJustify -1 Text is right justified in the display window
            // fulldustify 2 Text is fully justified (both left and right) in
            // the display window
            InitialJustification = (TextJustification)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
            offset += 2;
        }
        else
        {
            InitialJustification = null;
        }

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Did not consume all data for StaticTextControlTemplate.");
    }
}
