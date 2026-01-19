using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Scroll Bar Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct ScrollBarControlTemplate
{
    /// <summary>
    /// Minimum size of Scroll Bar Control Template in bytes.
    /// </summary>
    public const int MinSize = 6;

    /// <summary>
    /// Gets the maximum size.
    /// </summary>
    public ushort MaxSize { get; }

    /// <summary>
    /// Gets the view size.
    /// </summary>
    public ushort ViewSize { get; }

    /// <summary>
    /// Gets the initial value.
    /// </summary>
    public ushort InitialValue { get; }

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollBarControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Scroll Bar Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ScrollBarControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.ScrollBar)
        {
            throw new ArgumentException($"Invalid control procedure for ScrollBar: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 9 || header.ParameterCount > 10)
        {
            throw new ArgumentException($"Invalid parameter count for ScrollBar: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-34 to E-35
        int offset = 0;

        MaxSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        ViewSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        InitialValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (header.ParameterCount >= 10)
        {
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        bytesRead = offset;
        Debug.Assert(bytesRead <= data.Length, "Read beyond end of data.");
    }
}
