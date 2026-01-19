using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record defining the control color table.
/// </summary>
public readonly struct ControlColorTableRecord
{
    /// <summary>
    /// Gets the raw data of the color table.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlColorTableRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Control Color Table record.</param>
    public ControlColorTableRecord(ReadOnlySpan<byte> data)
    {
        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-46
        int offset = 0;

        // Resources of this type store color tables for various tool sets. These resources do not have
        // a consistent internal layout; you must construct these resources according to the needs of
        // the tool set that is to use the colortable.
        Data = data[offset..].ToArray();
        offset += Data.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for ControlColorTableRecord.");
    }
}
