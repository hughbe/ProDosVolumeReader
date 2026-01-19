using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Simple Button within a Control Template Record
/// </summary>
public readonly struct SimpleButtonControlTemplate
{
    /// <summary>
    /// Minimum size of Simple Button Control Template in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the title text reference.
    /// </summary>
    public uint TitleTextReference { get; }

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Gets the keystroke equivalent.
    /// </summary>
    public KeystrokeEquivalent? KeyEquivalent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleButtonControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Simple Button Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public SimpleButtonControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.SimpleButton)
        {
            throw new ArgumentException($"Invalid control procedure for SimpleButton: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 7 || header.ParameterCount > 9)
        {
            throw new ArgumentException($"Invalid parameter count for SimpleButton: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-13 to E-14
        int offset = 0;

        TitleTextReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (header.ParameterCount >= 8)
        {
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        if (header.ParameterCount >= 9)
        {
            // Keystroke equivalent information stored at keyEquivalentis
            // formatted as shown in Figure E-6.
            KeyEquivalent = new KeystrokeEquivalent(data.Slice(offset, KeystrokeEquivalent.Size));
            offset += KeystrokeEquivalent.Size;
        }
        else
        {
            KeyEquivalent = null;
        }

        bytesRead = offset;
        Debug.Assert(bytesRead <= data.Length, "Did not read more data than available.");
    }
}
