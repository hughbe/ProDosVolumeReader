using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// List controls record.
/// </summary>
public readonly struct ListControlsRecord
{
    /// <summary>
    /// Gets the list of controls.
    /// </summary>
    public List<ListControl> Controls { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListControlsRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the ListControlsRecord record.</param>
    public ListControlsRecord(ReadOnlySpan<byte> data)
    {
        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-51
        int offset = 0;

        var controls = new List<ListControl>();
        while (offset < data.Length)
        {
            var control = new ListControl(data[offset..]);
            controls.Add(control);
            offset += ListControl.MinSize + control.Data.Length;
        }

        Controls = controls;

        Debug.Assert(offset == data.Length, "Did not consume all data for ListControlsRecord record.");
    }
}
