namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing executable 68k code.
/// </summary>
public readonly struct CodeRecord
{
    /// <summary>
    /// Gets the raw data for the CodeRecord.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the CodeRecord.</param>
    public CodeRecord(ReadOnlySpan<byte> data)
    {
        Data = data.ToArray();
    }
}
