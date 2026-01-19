using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents a Dead Key Validation entry in a Keystroke Translation Table.
/// </summary>
public readonly struct DeadKeyValidation
{
    /// <summary>
    /// Size of the DeadKeyValidation structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// The character code for the dead key.
    /// </summary>
    public byte CharacterCode { get; }

    /// <summary>
    /// Byte offset from beginning of deadKeyTable into the relevant
    /// subarray in replacementTable, divided by 2.
    /// </summary>
    public byte OffsetToReplacementTable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeadKeyValidation"/> struct.
    /// </summary>
    /// <param name="data">The byte span containing the DeadKeyValidation data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is less than the required size.</exception>
    public DeadKeyValidation(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data length {data.Length} does not match expected size {Size}.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-50
        int offset = 0;

        // The character code for the dead key. The system uses this value
        // to check for user input of a dead key. The system compares this
        // value with the first user keystroke.
        CharacterCode = data[offset];
        offset += 1;

        // Byte offset from beginning of deadKeyTab1einto the relevant
        // subarray in replacement Tab1e, divided by 2. The system
        // uses this value to access the valid replacement values for the
        // dead key in question.
        OffsetToReplacementTable = data[offset];
        offset += 1;
        
        Debug.Assert(offset == data.Length, "Did not consume all data.");
    }
}
