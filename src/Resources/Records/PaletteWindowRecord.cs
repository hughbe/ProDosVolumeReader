using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents a Palette Window record.
/// </summary>
public readonly struct PaletteWindowRecord
{
    /// <summary>
    /// Gets the raw data of the Palette Window record.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaletteWindowRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Palette Window record.</param>
    public PaletteWindowRecord(ReadOnlySpan<byte> data)
    {
        // The structure is undocumented; store raw data
        int offset = 0;

        Data = data[offset..].ToArray();
        offset += Data.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for PaletteWindow record.");
    }
}
