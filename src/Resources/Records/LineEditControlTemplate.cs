using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Line Edit within a Control Template Record
/// </summary>
public readonly struct LineEditControlTemplate
{
    /// <summary>
    /// Size of Line Edit Control Template in bytes.
    /// </summary>
    public const int Size = 6;

    /// <summary>
    /// Gets the maximum size of the line edit field.
    /// </summary>
    public ushort MaxSize { get; }

    /// <summary>
    /// Gets the default text reference.
    /// </summary>
    public uint DefaultTextReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineEditControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Line Edit Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public LineEditControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < Size)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.LineEdit)
        {
            throw new ArgumentException($"Invalid control procedure for LineEdit: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount != 8)
        {
            throw new ArgumentException($"Invalid parameter count for LineEdit: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-21 to E-22
        int offset = 0;

        // The maximum number of characters allowed in the LineEdit field.
        // Valid values lie in the range from 1 to 255.
        // The high-order bit indicates whether the LineEdit field is a password
        // field. Password fields protect user input by echoing asterisks rather
        // than the actual user input. If this bit is set to 1, then the LineEdit field
        // is a passwordfield.
        // Note that LineEdit controls do not support color tables.
        MaxSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        DefaultTextReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Should not have read past the end of the data.");
    }
}
