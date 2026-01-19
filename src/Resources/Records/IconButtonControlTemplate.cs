using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Icon Button Control within a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct IconButtonControlTemplate
{
    /// <summary>
    /// Minimum size of Icon Button Control Template in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the icon reference.
    /// </summary>
    public uint IconReference { get; }

    /// <summary>
    /// Gets the title reference.
    /// </summary>
    public uint? TitleReference { get; }

    /// <summary>
    /// Gets the color table reference.
    /// </summary>
    public uint? ColorTableReference { get; }

    /// <summary>
    /// Gets the display mode.
    /// </summary>
    public IconButtonDisplayMode? DisplayMode { get; }

    /// <summary>
    /// Gets the keystroke equivalent.
    /// </summary>
    public KeystrokeEquivalent? KeyEquivalent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IconButtonControlTemplate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Icon Button Control Template.</param>
    /// <param name="header">The Control Template header.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public IconButtonControlTemplate(ReadOnlySpan<byte> data, ControlTemplateHeader header, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }
        if (header.Procedure != ControlProcedure.IconButton)
        {
            throw new ArgumentException($"Invalid control procedure for IconButton: {header.Procedure}", nameof(data));
        }
        if (header.ParameterCount < 7 || header.ParameterCount > 11)
        {
            throw new ArgumentException($"Invalid parameter count for IconButton: {header.ParameterCount}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-17 to E-19
        int offset = 0;

        IconReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        if (header.ParameterCount >= 8)
        {
            // Reference to thetitle string, which must be a Pascalstring. If you are
            // not using a title but are specifying other optional fields, set
            // moreFlags bits 0 and 1 to 0, and set this field to 0.
            TitleReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            TitleReference = null;
        }

        if (header.ParameterCount >= 9)
        {
            ColorTableReference = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            offset += 4;
        }
        else
        {
            ColorTableReference = null;
        }

        if (header.ParameterCount >= 10)
        {
            // Passed directly to the DrawIcon routine, a field defining the display
            // mode for the icon. Thefield is defined as follows (for more
            // information on icons, see Chapter 17, “QuickDraw II Auxiliary,” in
            // Volume2 of the Toolbox Reference).
            // Background color bits 15-12 Defines the background color to apply to black part
            // of black-and-white icons.
            // Foreground color bits 11-8 Defines the foreground color to apply to white part
            // of black-and-white icons.
            // Reserved bits 7-3 Mustbeset to 0.
            // offLine bit 2 0 = Don’t perform the AND operation on the image
            // 1 = Perform the logical AND operation with light-gray
            // pattern and image being copied
            // openicon
            // bit 1 0 = Don’t copylight-gray pattern
            // 1 = Copylight-gray pattern instead of image
            // selectedIcon
            // bit 0 0 = Don’t invert image
            // 1 = Invert image before copying
            // Color values (both foreground and background) are indexes into the
            // current color table. See Chapter 16, “QuickDrawII,” in Volume 2 of the
            // Toolbox Reference for details about the format and contentof these
            // color tables.
            DisplayMode = new IconButtonDisplayMode(data.Slice(offset, IconButtonDisplayMode.Size));
            offset += IconButtonDisplayMode.Size;
        }
        else
        {
            DisplayMode = null;
        }

        if (header.ParameterCount >= 11)
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
        Debug.Assert(offset <= data.Length, "Should not have read past the end of the data.");
    }
}
