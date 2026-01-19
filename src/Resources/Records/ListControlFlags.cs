using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Control flags for a list control member.
/// </summary>
public readonly struct ListControlFlags
{
    /// <summary>
    /// Size of ListControlFlags structure in bytes.
    /// </summary>
    public const int Size = 1;

    /// <summary>
    /// Gets the raw byte value of the flags.
    /// </summary>
    public byte RawValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListControlFlags"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the ListControlFlags.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public ListControlFlags(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"ListControlFlags requires {Size} bytes.", nameof(data));
        }

        int offset = 0;

        RawValue = data[0];
        offset += 1;

        Debug.Assert(offset == data.Length, "Did not consume all data for ListControlFlags.");
    }

    /// <summary>
    /// Gets the selection state of the item (bits 6-7).
    /// </summary>
    public ListControlSelectionState SelectionState => (ListControlSelectionState)((RawValue >> 6) & 0x3);
}
