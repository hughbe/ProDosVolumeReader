using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Local Access structure.
/// </summary>
public readonly struct BundleDocumentLocalAccess
{
    /// <summary>
    /// Size of the BundleDocumentLocalAccess structure in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// Gets the access mask.
    /// </summary>
    public uint Mask { get; }

    /// <summary>
    /// Gets the access comparison.
    /// </summary>
    public uint ComparisonValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentLocalAccess"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentLocalAccess.</param>
    /// <exception cref="ArgumentException">Thrown when data is the wrong size.</exception>
    public BundleDocumentLocalAccess(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"BundleDocumentLocalAccess requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        Mask = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        ComparisonValue = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all data for BundleDocumentLocalAccess.");
    }
}
