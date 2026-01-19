using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents a Keystroke Translation Table in a resource.
/// </summary>
public readonly struct KeystrokeTranslationTableRecord
{
    /// <summary>
    /// Minimum size of the KeystrokeTranslationTable structure in bytes.
    /// </summary>
    public const int MinSize = 256;

    /// <summary>
    /// Gets the translation table mapping ASCII codes to character values.
    /// </summary>
    public byte[] TranslationTable { get; }

    /// <summary>
    /// Gets the array of Dead Key Validation entries.
    /// </summary>
    public List<DeadKeyValidation> DeadKeyValidationArray { get; }

    /// <summary>
    /// Gets the array of Dead Key Replacement entries.
    /// </summary>
    public List<DeadKeyReplacement> DeadKeyReplacementArray { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeystrokeTranslationTableRecord"/> struct.
    /// </summary>
    /// <param name="data">The byte span containing the KeystrokeTranslationTable data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is less than the minimum size.</exception>
    public KeystrokeTranslationTableRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length {data.Length} is less than minimum size {MinSize}.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-49 to E-50
        int offset = 0;

        // A packed array of bytes used to map the ASCII codes produced by
        // the keyboard into the character value to be generated. Each cell in the
        // array corresponds directly to the ASCII code that is equivalent to the
        // cell offset. For example, the transTablecell at offset $0D (13
        // decimal) contains the character replacement value for keyboard code
        // $0D, which, for a straight ASCII translation table, is a carriage return
        // (CR). Cells 128 to 255 ($80 to $FF) of the transTable contain values
        // for Option-key sequences (such as Option-S).
        TranslationTable = data.Slice(offset, 256).ToArray();
        offset += 256;

        // Table containing entries used to validate dead keys. Dead keys are
        // keystrokes used to introduce multikey sequences that produce single
        // characters. For example, pressing Option-U followed bye yields é.
        // There is one entry in deadKeyTablefor each defined dead key. The
        // last entry must be set to $0000.
        var deadKeyValidationList = new List<DeadKeyValidation>();
        while (offset < data.Length)
        {
            if (offset + DeadKeyValidation.Size > data.Length)
            {
                throw new ArgumentException("Insufficient data for DeadKeyValidation entry.", nameof(data));
            }

            var deadKeyValidation = new DeadKeyValidation(data.Slice(offset, DeadKeyValidation.Size));
            offset += DeadKeyValidation.Size;

            // The last entry must be set to $0000.
            if (deadKeyValidation.CharacterCode == 0 && deadKeyValidation.OffsetToReplacementTable == 0)
            {
                break;
            }

            deadKeyValidationList.Add(deadKeyValidation);
        }

        DeadKeyValidationArray = deadKeyValidationList;

        // Table containing the valid replacement values for each dead key
        // combination. This table is made up of a series of variable-length
        // subarrays, each relevant to a particular dead key. The last entry in each
        // subarray must be set to $0000.
        var deadKeyReplacementList = new List<DeadKeyReplacement>();
        while (offset < data.Length)
        {
            if (offset + DeadKeyReplacement.Size > data.Length)
            {
                throw new ArgumentException("Insufficient data for DeadKeyReplacement entry.", nameof(data));
            }
            
            var deadKeyReplacement = new DeadKeyReplacement(data.Slice(offset, DeadKeyReplacement.Size));
            offset += DeadKeyReplacement.Size;

            // The last entry in each subarray must be set to $0000.
            if (deadKeyReplacement.ScanKey == 0 && deadKeyReplacement.ReplacementCharacter == 0)
            {
                break;
            }

            deadKeyReplacementList.Add(deadKeyReplacement);
        }

        DeadKeyReplacementArray = deadKeyReplacementList;

        Debug.Assert(offset <= data.Length, "Did not consume all data.");
    }
}
