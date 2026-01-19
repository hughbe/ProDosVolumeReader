using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document End of File structure.
/// </summary>
public readonly struct BundleDocumentEOF
{
    /// <summary>
    /// Size of the BundleDocumentEOF structure in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// Gets the comparison type.
    /// </summary>
    public uint ComparisonType { get; }

    /// <summary>
    /// Gets the eof comparison.
    /// </summary>
    public uint ComparisonValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentEOF"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentEOF.</param>
    /// <exception cref="ArgumentException">Thrown when data is the wrong size.</exception>
    public BundleDocumentEOF(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"BundleDocumentEOF requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        ComparisonType = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        ComparisonValue = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all data for BundleDocumentEOF.");
    }
}
