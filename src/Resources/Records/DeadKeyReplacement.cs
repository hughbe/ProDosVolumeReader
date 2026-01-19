using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents a Dead Key Replacement entry in a Keystroke Translation Table.
/// </summary>
public readonly struct DeadKeyReplacement
{
    /// <summary>
    /// Size of the DeadKeyReplacement structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the valid character code for dead key replacement.
    /// </summary>
    public byte ScanKey { get; }

    /// <summary>
    /// Gets the replacement character for the specified scan key.
    /// </summary>
    public byte ReplacementCharacter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeadKeyReplacement"/> struct.
    /// </summary>
    /// <param name="data">The byte span containing the DeadKeyReplacement data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is less than the required size.</exception>
    public DeadKeyReplacement(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data length {data.Length} does not match expected size {Size}.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-50
        int offset = 0;

        // A valid character code for dead key replacement. The system
        // uses this field to determine whether the user entered a valid dead
        // key combination. The system compares this value with the
        // second user keystroke.
        ScanKey = data[offset];
        offset += 1;

        // The replacementvalue for the character specified in scankey
        // for this entry. The system delivers this value as the replacement
        // for a valid dead key combination.
        ReplacementCharacter = data[offset];
        offset += 1;

        Debug.Assert(offset == data.Length, "Did not consume all data.");
    }
}
