using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Picture Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct PictureControlTemplate
{
    /// <summary>
    /// Size of Picture Control Template in bytes.
    /// </summary>
    public const int Size = 4;

    /// <summary>
    /// Gets the picture reference.
    /// </summary>
    public uint PictureReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PictureControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Picture Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead"> The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public PictureControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < Size)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.Picture)
        {
            throw new ArgumentException($"Invalid control procedure for Picture: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount != 7)
        {
            throw new ArgumentException($"Invalid parameter count for Picture: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-26 to E-27
        int offset = 0;

        PictureReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Should not have read past the end of the data.");
    }
}
