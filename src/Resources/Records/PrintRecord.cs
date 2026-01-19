using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Print Record structure in a Resource Fork.
/// </summary>
public readonly struct PrintRecord
{
    /// <summary>
    /// Size of Print Record in bytes.
    /// </summary>
    public const int Size = 160;

    /// <summary>
    /// Gets the data.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Print Record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is not equal to the expected size.</exception>
    public PrintRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid PrintRecord size. Expected {Size} bytes, got {data.Length} bytes.", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.076.html
        int offset = 0;

        Data = data.Slice(offset, Size).ToArray();
        offset += Size;

        Debug.Assert(offset == data.Length, "Did not consume all data for PrintRecord.");
    }
}
