using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources;

/// <summary>
/// Control List Record
/// </summary>
public readonly struct ControlListRecord
{
    /// <summary>
    /// Minimum size of Control List Record in bytes.
    /// </summary>
    public const int MinSize = 4;

    /// <summary>
    /// Gets the Resource IDs of the controls in the list.
    /// </summary>
    public List<uint> Controls { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlListRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Control List Record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ControlListRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-6
        int offset = 0;

        // List of resource IDs for control template definitions. The last entry
        // must be set to NIL.
        var controls = new List<uint>();
        while (offset <= data.Length)
        {
            if (offset + 4 > data.Length)
            {
                throw new ArgumentException("Data length is insufficient for control ID.", nameof(data));
            }

            var control = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            if (control == 0)
            {
                break;
            }

            controls.Add(control);
            offset += 4;
        }

        Controls = controls;
        Debug.Assert(offset <= data.Length, "Did not consume all data for Control List Record.");
    }
}
