using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Pattern List structure in a Resource Fork.
/// </summary>
public readonly struct PatternListRecord
{
    /// <summary>
    /// Gets the patterns.
    /// </summary>
    public List<byte[]> Patterns { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PatternListRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Pattern List.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public PatternListRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length % 32 != 0)
        {
            throw new ArgumentException("Invalid PatternListRecord data length.", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.076.html.
        int offset = 0;

        // firstPattern (+000): 32 Bytes
        // First QuickDraw II pattern structure.
        // secondPattern (+032): 32 Bytes
        // Second QuickDraw II pattern structure.
        var count = data.Length / 32;
        var patterns = new List<byte[]>(count);
        for (int i = 0; i < count; i++)
        {
            patterns.Add(data.Slice(offset, 32).ToArray());
            offset += 32;
        }

        Patterns = patterns;

        Debug.Assert(offset == data.Length, "Did not consume all data for PatternListRecord.");
    }
}
