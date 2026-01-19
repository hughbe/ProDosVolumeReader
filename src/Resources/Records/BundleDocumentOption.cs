using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents a Bundle Document Option.
/// </summary>
public readonly struct BundleDocumentOption
{
    /// <summary>
    /// The minimum size of a BundleDocumentOption (without any file systems).
    /// </summary>
    public const int MinSize = 2;

    /// <summary>
    /// Gets the count of file systems.
    /// </summary>
    public ushort FileSystemsCount { get; }

    /// <summary>
    /// Gets the list of file systems.
    /// </summary>
    public List<ushort> FileSystems { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentOption"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentOption.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when data is too short.</exception>
    public BundleDocumentOption(ReadOnlySpan<byte> data, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length {data.Length} is less than minimum size {MinSize}.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        FileSystemsCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (offset + FileSystemsCount * 2 > data.Length)
        {
            throw new ArgumentException("Data is too short to read all file systems.", nameof(data));
        }

        var fileSystems = new List<ushort>(FileSystemsCount);
        for (int i = 0; i < FileSystemsCount; i++)
        {
            fileSystems.Add(BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)));
            offset += 2;
        }

        FileSystems = fileSystems;

        bytesRead = offset;
        Debug.Assert(bytesRead <= data.Length, "Should not read more bytes than are available in data.");
    }
}
