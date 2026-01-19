using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Modification Date structure.
/// </summary>
public readonly struct BundleDocumentModificationDate
{
    /// <summary>
    /// Size of the BundleDocumentModificationDate structure in bytes.
    /// </summary>
    public const int Size = 10;

    /// <summary>
    /// Gets the comparison type.
    /// </summary>
    public ushort Comparison { get; }

    /// <summary>
    /// Gets the date value as a string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentModificationDate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentModificationDate.</param>
    /// <exception cref="ArgumentException">Thrown when data is the wrong size.</exception>
    public BundleDocumentModificationDate(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"BundleDocumentModificationDate requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        Comparison = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Value = Encoding.ASCII.GetString(data.Slice(offset, 8));
        offset += 8;

        Debug.Assert(offset == data.Length, "Did not consume all data for BundleDocumentModificationDate.");
    }
}
