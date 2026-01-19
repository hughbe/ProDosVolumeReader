using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Creation Date structure.
/// </summary>
public readonly struct BundleDocumentCreationDate
{
    /// <summary>
    /// Size of the BundleDocumentCreationDate structure in bytes.
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
    /// Initializes a new instance of the <see cref="BundleDocumentCreationDate"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentCreationDate.</param>
    /// <exception cref="ArgumentException">Thrown when data is the wrong size.</exception>
    public BundleDocumentCreationDate(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"BundleDocumentCreationDate requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        Comparison = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Value = Encoding.ASCII.GetString(data.Slice(offset, 8));
        offset += 8;

        Debug.Assert(offset == data.Length, "Did not consume all data for BundleDocumentCreationDate.");
    }
}
