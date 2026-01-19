using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record defining the colors used in a window.
/// </summary>
public readonly struct WindowColorTableRecord
{
    /// <summary>
    /// Size of WindowColorTableRecord structure in bytes.
    /// </summary>
    public const int Size = 10;

    /// <summary>
    /// Gets the frame color.
    /// </summary>
    public WindowFrameColor FrameColor { get; }

    /// <summary>
    /// Gets the title color.
    /// </summary>
    public WindowTitleColor TitleColor { get; }

    /// <summary>
    /// Gets the bar color.
    /// </summary>
    public WindowBarColor BarColor { get; }

    /// <summary>
    /// Gets the grow color.
    /// </summary>
    public WindowGrowColor GrowColor { get; }

    /// <summary>
    /// Gets the info color.
    /// </summary>
    public WindowInfoColor InfoColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowColorTableRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Window Color Table record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public WindowColorTableRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < Size)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-72 to E-73
        int offset = 0;

        // Color of the window frame and of the alert frame.
        // Reserved bits 8-15 Must be set to 0
        // windowFrame bits 4-7 Color of window frame-value is an index into the
        // active color table
        // Reserved bits 0-3 Must be set to 0
        FrameColor = new WindowFrameColor(data.Slice(offset, WindowFrameColor.Size));
        offset += WindowFrameColor.Size;

        // Colors of inactive title bar, inactive title, and active title:
        // Reserved bits 12-15 Must be set to 0
        // inactiveTitleBar bits 8-11 Color of inactive title bars-value is an index into the
        // active color table
        // inactiveTitle bits 4-7 Color of inactive titles-value is an index into the
        // active color table
        // activeTitle bits 0-3 Color of active titles, close box, and zoom boxvalue is an index into the active color table.
        TitleColor = new WindowTitleColor(data.Slice(offset, WindowTitleColor.Size));
        offset += WindowTitleColor.Size;

        // Color and pattern information for active title bar:
        // pattern bits 8-15 Defines pattern for title bar:
        // 00 = Solid
        // 01 = Dither
        // 02 = Lined
        // patternColor bits 4-7 Color for pattern-value is an index into the active
        // color table
        // backColor bits 0-3 Background color-value is an index into the active
        // color table.
        BarColor = new WindowBarColor(data.Slice(offset, WindowBarColor.Size));
        offset += WindowBarColor.Size;

        // Color of size box and alert frame's middle outline:
        // alertMidFrame bits 12-15 Color of alert frame middle outline-value is an index
        // into the active color table
        // Reserved bits 8-11 Must be set to 0
        // sizeUnselected bits 4-7 Color for unselected size box-value is an index into
        // the active color table
        // sizeSelected bits 0-3 Color for selected size box-value is an index into the
        // active color table.
        GrowColor = new WindowGrowColor(data.Slice(offset, WindowGrowColor.Size));
        offset += WindowGrowColor.Size;

        // Color of information bar and alert frame's inside outline:
        // alertMidFrame bits 12-15 Color of alert frame inside outline-value is an index
        // into the active color table
        // Reserved bits 8-11 Must be set to 0
        // infoBar bits 4-7 Color for information bar-value is an index into the
        // active color table
        // Reserved bits 0-3 Must be set to 0
        InfoColor = new WindowInfoColor(data.Slice(offset, WindowInfoColor.Size));
        offset += WindowInfoColor.Size;

        Debug.Assert(offset <= data.Length, "Did not consume all data for WindowColorTableRecord.");
    }
}
    