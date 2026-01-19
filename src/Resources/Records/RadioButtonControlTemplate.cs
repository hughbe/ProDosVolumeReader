using System.Buffers.Binary;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Radio Button Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct RadioButtonControlTemplate
{
    /// <summary>
    /// Minimum size of Radio Button Control Template in bytes.
    /// </summary>
    public const int MinSize = 6;

    /// <summary>
    /// Gets the title reference.
    /// </summary>
    public uint TitleReference { get; }

    /// <summary>
    /// Gets the initial value.
    /// </summary>
    public ushort InitialValue { get; }

    /// <summary>
    /// Gets the color table reference, if present.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Gets the keystroke equivalent, if present.
    /// </summary>
    public KeystrokeEquivalent? KeyEquivalent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadioButtonControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Radio Button Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">>The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public RadioButtonControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.RadioButton)
        {
            throw new ArgumentException($"Invalid control procedure for RadioButton: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 8 || header.ParameterCount > 10)
        {
            throw new ArgumentException($"Invalid parameter count for RadioButton: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-32 to E-33
        int offset = 0;

        TitleReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        InitialValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (header.ParameterCount >= 9)
        {
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        if (header.ParameterCount == 10)
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
    }
}
