using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Aux Type structure.
/// </summary>
public readonly struct BundleDocumentAuxType
{
    /// <summary>
    /// Size of the BundleDocumentAuxType structure in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// Gets the aux type mask.
    /// </summary>
    public uint Mask { get; }

    /// <summary>
    /// Gets the aux type comparison.
    /// </summary>
    public uint Comparison { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentAuxType"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentAuxType.</param>
    /// <exception cref="ArgumentException">Thrown when data is the wrong size.</exception>
    public BundleDocumentAuxType(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"BundleDocumentAuxType requires exactly {Size} bytes.", nameof(data));
        }

        int offset = 0;

        Mask = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Comparison = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all data for BundleDocumentAuxType.");
    }
}
