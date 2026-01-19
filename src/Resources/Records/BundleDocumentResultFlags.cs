using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document Result Flags structure.
/// </summary>
public struct BundleDocumentResultFlags
{
    /// <summary>
    /// Size of BundleDocumentResultFlags structure in bytes.
    /// </summary>
    public const int Size = 2;

    /// <summary>
    /// Gets the raw 16-bit value of the flags.
    /// </summary>
    public ushort RawValue { get; }

    /// <summary>
    /// Gets a value indicating whether to launch this document (bit 0).
    /// </summary>
    public readonly bool LaunchThis => (RawValue & 0x0001) != 0;

    /// <summary>
    /// Gets the voting clout (bits 4-7).
    /// </summary>
    public readonly ushort VotingClout => (ushort)((RawValue & 0x00F0) >> 4);

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentResultFlags"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentResultFlags.</param>
    /// <exception cref="ArgumentException">Thrown when data length is incorrect.</exception>
    public BundleDocumentResultFlags(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"BundleDocumentResultFlags requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for BundleDocumentResultFlags.");
    }
}