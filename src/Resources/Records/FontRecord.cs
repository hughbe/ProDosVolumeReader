namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Font Record structure in a Resource Fork.
/// </summary>
public readonly struct FontRecord
{
    /// <summary>
    /// Gets the data.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Font Record.</param>
    public FontRecord(ReadOnlySpan<byte> data)
    {
        Data = data.ToArray();
#if DEBUG
        throw new NotImplementedException("FontRecord parsing not yet implemented.");
#endif
    }
}
