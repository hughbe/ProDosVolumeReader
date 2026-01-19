using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Size Box Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct SizeBoxControlTemplate
{
    /// <summary>
    /// Minimum size of Size Box Control Template in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeBoxControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Size Box Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public SizeBoxControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.SizeBox)
        {
            throw new ArgumentException($"Invalid control procedure for SizeBox: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 4 || header.ParameterCount > 5)
        {
            throw new ArgumentException($"Invalid parameter count for SizeBox: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-36
        int offset = 0;

        if (header.ParameterCount == 5)
        {
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        bytesRead = offset;
        Debug.Assert(bytesRead <= data.Length, "bytesRead should not exceed data length");
    }
}
