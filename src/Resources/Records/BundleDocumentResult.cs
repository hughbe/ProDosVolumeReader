using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Result structure.
/// </summary>
public readonly struct BundleDocumentResult
{
    /// <summary>
    /// Size of BundleDocumentResult structure in bytes.
    /// </summary>
    public const int MinSize = 2;

    /// <summary>
    /// Gets the flags for the document result.
    /// </summary>
    public BundleDocumentResultFlags Flags { get; }

    /// <summary>
    /// Gets the Finder path resource ID.
    /// </summary>
    public uint FinderPathResourceID { get; }

    /// <summary>
    /// Gets the handle to the Finder path resource.
    /// </summary>
    public uint FinderPathResourceHandle { get; }

    /// <summary>
    /// Gets the large icon resource ID.
    /// </summary>
    public uint LargeIconResourceID { get; }

    /// <summary>
    /// Gets the handle to the large icon resource.
    /// </summary>
    public uint LargeIconResourceHandle { get; }

    /// <summary>
    /// Gets the small icon resource ID.
    /// </summary>
    public uint SmallIconResourceID { get; }

    /// <summary>
    /// Gets the handle to the small icon resource.
    /// </summary>
    public uint SmallIconResourceHandle { get; }

    /// <summary>
    /// Gets the type string.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentResult"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentResult.</param>
    /// <param name="resultCount">The number of results in the document.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when data is too small.</exception>
    public BundleDocumentResult(ReadOnlySpan<byte> data, int resultCount, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"BundleDocumentResult requires at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        Flags = new BundleDocumentResultFlags(data.Slice(offset, BundleDocumentResultFlags.Size));
        offset += BundleDocumentResultFlags.Size;

        FinderPathResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        FinderPathResourceHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        LargeIconResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;
        
        LargeIconResourceHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        SmallIconResourceID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        SmallIconResourceHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        byte typeLength = data[offset];
        offset += 1;

        if (offset + typeLength > data.Length)
        {
            if (resultCount == 5)
            {
                throw new ArgumentException("BundleDocumentResult type string exceeds available data length.", nameof(data));
            }
            else
            {
                // For result counts other than 5, the type string may be omitted.
                Type = string.Empty;
                bytesRead = offset;
                return;
            }
        }

        Type = Encoding.ASCII.GetString(data.Slice(offset, typeLength));
        offset += typeLength;

        bytesRead = offset;
        Debug.Assert(offset <= data.Length, "Did not consume all data for BundleDocumentResult.");
    }
}
