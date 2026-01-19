using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Extended structure.
/// </summary>
public readonly struct BundleDocumentExtended
{
    /// <summary>
    /// Size of the BundleDocumentExtended structure in bytes.
    /// </summary>
    public const int Size = 4;

    /// <summary>
    /// Gets the mask.
    /// </summary>
    public ushort Mask { get; }

    /// <summary>
    /// Gets a value indicating whether this is extended.
    /// </summary>
    public ushort IsExtended { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentExtended"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentExtended.</param>
    /// <exception cref="ArgumentException">Thrown when data is the wrong size.</exception>
    public BundleDocumentExtended(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"BundleDocumentExtended requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        Mask = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        IsExtended = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for BundleDocumentExtended.");
    }
}
