namespace ProDosVolumeReader.Resources;

/// <summary>
/// GS/OS Resource Fork
/// </summary>
public class GsOsResourceFork
{
    private Stream _stream;

    /// <summary>
    /// Gets the Resource Fork Header.
    /// </summary>
    public GsOsResourceForkHeader Header { get; }

    /// <summary>
    /// Gets the Resource Fork Map.
    /// </summary>
    public GsOsResourceForkMap Map { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GsOsResourceFork"/> class.
    /// </summary>
    /// <param name="stream">The stream containing the Resource Fork data.</param>
    /// <exception cref="InvalidDataException">Thrown when the Resource Fork data is invalid.</exception>
    public GsOsResourceFork(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanSeek || !stream.CanRead)
        {
            throw new ArgumentException("Stream must be seekable and readable.", nameof(stream));
        }

        _stream = stream;

        // Structure documented in https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
        // and https://ciderpress2.com/formatdoc/ResourceFork-notes.html
        Span<byte> headerData = stackalloc byte[GsOsResourceForkHeader.Size];
        stream.ReadExactly(headerData);

        Header = new GsOsResourceForkHeader(headerData);

        if (Header.MapOffset >= stream.Length)
        {
            throw new InvalidDataException("Resource Fork map offset is beyond end of stream.");
        }
        
        stream.Seek(Header.MapOffset, SeekOrigin.Begin);
        Span<byte> mapData = Header.MapSize <= 1024
            ? stackalloc byte[(int)Header.MapSize]
            : new byte[Header.MapSize];
        stream.ReadExactly(mapData);
        Map = new GsOsResourceForkMap(mapData);
    }

    /// <summary>
    /// Gets the resource data for the specified resource reference record.
    /// </summary>
    /// <param name="record">The resource reference record.</param>
    /// <returns>The resource data as a byte array.</returns>
    public byte[] GetResourceData(GsOsResourceForkReferenceRecord record)
    {
        using var memoryStream = new MemoryStream((int)record.DataSize);
        GetResourceData(record, memoryStream);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Gets the resource data for the specified resource reference record and writes it to the provided output stream.
    /// </summary>
    /// <param name="record">The resource reference record.</param>
    /// <param name="outputStream">The output stream to write the resource data to.</param>
    /// <returns>The number of bytes written to the output stream.</returns>
    /// <exception cref="InvalidDataException">Thrown when the resource data offset is invalid.</exception>
    public int GetResourceData(GsOsResourceForkReferenceRecord record, Stream outputStream)
    {
        ArgumentNullException.ThrowIfNull(outputStream);

        if (record.DataOffset >= _stream.Length)
        {
            throw new InvalidDataException("Resource data offset is beyond end of stream.");
        }

        _stream.Seek(record.DataOffset, SeekOrigin.Begin);
        if (record.DataSize == 0)
        {
            return 0;
        }
        
        _stream.CopyTo(outputStream, (int)record.DataSize);
        return (int)record.DataSize;
    }
}