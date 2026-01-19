using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle record structure.
/// </summary>
public readonly struct BundleRecord
{
    /// <summary>
    /// Minimum size of the Bundle record in bytes.
    /// </summary>
    public const int MinSize = 18;

    /// <summary>
    /// Gets the version of the Bundle record.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the offset to the documents section.
    /// </summary>
    public ushort DocumentsOffset { get; }

    /// <summary>
    /// Gets the icon resource ID.
    /// </summary>
    public uint IconResourceID { get; }

    /// <summary>
    /// Gets the bundle resource ID.
    /// </summary>
    public uint BundleResourceID { get; }

    /// <summary>
    /// Gets the bundle resource handle.
    /// </summary>
    public uint BundleResourceHandle { get; }

    /// <summary>
    /// Gets the number of documents in the bundle.
    /// </summary>
    public ushort NumberOfDocuments { get; }

    /// <summary>
    /// Gets the list of documents in the bundle.
    /// </summary>
    public List<BundleDocument> Documents { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleRecord"/> struct from the given data.
    /// </summary>
    /// <param name="data">The raw data for the Bundle record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is less than the minimum size.</exception>
    /// <exception cref="NotImplementedException">Thrown when the Bundle record version is not supported.</exception>
    public BundleRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length {data.Length} is less than minimum size {MinSize} for Bundle record.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Version != 0)
        {
            throw new NotImplementedException($"Bundle record version {Version} is not supported.");
        }

        DocumentsOffset = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        IconResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        BundleResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        BundleResourceHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset <= data.Length, "All fixed fields should be read before accessing DocumentsOffset.");

        if (DocumentsOffset > data.Length)
        {
            throw new ArgumentException("DocumentsOffset exceeds data length.", nameof(data));
        }

        offset = DocumentsOffset;

        NumberOfDocuments = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        var documents = new List<BundleDocument>(NumberOfDocuments);
        for (int i = 0; i < NumberOfDocuments; i++)
        {
            var document = new BundleDocument(data[offset..]);
            documents.Add(document);
            offset += document.Size;
        }

        Documents = documents;

        Debug.Assert(offset <= data.Length, "All fields read should not exceed data length.");
    }
}
