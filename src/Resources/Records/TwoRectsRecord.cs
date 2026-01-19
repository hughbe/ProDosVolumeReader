using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing two RECT structures.
/// </summary>
public readonly struct TwoRectsRecord
{
    /// <summary>
    /// Size of TwoRectsRecord structure in bytes.
    /// </summary>
    public const int Size = 16;

    /// <summary>
    /// First RECT structure.
    /// </summary>
    public RECT First { get; }

    /// <summary>
    /// Second RECT structure.
    /// </summary>
    public RECT Second { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwoRectsRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the TwoRectsRecord.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public TwoRectsRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < Size)
        {
            throw new ArgumentException($"TwoRectsRecord requires at least {Size} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-71
        int offset = 0;

        // First rectangle.
        First = new RECT(data.Slice(offset, RECT.Size));
        offset += RECT.Size;

        // Second rectangle.
        Second = new RECT(data.Slice(offset, RECT.Size));
        offset += RECT.Size;

        Debug.Assert(offset <= data.Length, "Did not consume all data for TwoRectsRecord.");
    }
}
