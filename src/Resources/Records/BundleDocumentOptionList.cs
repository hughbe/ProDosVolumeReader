using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents a Bundle Document Option List.
/// </summary>
public readonly struct BundleDocumentOptionList
{
    /// <summary>
    /// The minimum size of a BundleDocumentOptionList (without any options).
    /// </summary>
    public const int MinSize = 14;

    /// <summary>
    /// Gets the count of group options.
    /// </summary>
    public ushort GroupOptionsCount { get; }

    /// <summary>
    /// Gets the list of group options.
    /// </summary>
    public List<BundleDocumentOption> GroupOptions { get; }

    /// <summary>
    /// Gets the option list offset.
    /// </summary>
    public ushort OptionListOffset { get; }

    /// <summary>
    /// Gets the mask.
    /// </summary>
    public uint Mask { get; }

    /// <summary>
    /// Gets the comparison value.
    /// </summary>
    public uint ComparisonValue { get; }

    /// <summary>
    /// Gets the comparison.
    /// </summary>
    public ushort ComparisonType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocumentOptionList"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocumentOptionList.</param>
    /// <param name="bytesRead">The number of bytes read from the data.</param>
    /// <exception cref="ArgumentException">Thrown when data is too short.</exception>
    public BundleDocumentOptionList(ReadOnlySpan<byte> data, out int bytesRead)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length {data.Length} is less than minimum size {MinSize}.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        GroupOptionsCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        var groupOptions = new List<BundleDocumentOption>(GroupOptionsCount);
        for (int i = 0; i < GroupOptionsCount; i++)
        {
            groupOptions.Add(new BundleDocumentOption(data[offset..], out var groupOptionBytesRead));
            offset += groupOptionBytesRead;
        }

        GroupOptions = groupOptions;

        OptionListOffset = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Mask = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        ComparisonValue = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        ComparisonType = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        bytesRead = offset;
        Debug.Assert(bytesRead <= data.Length, "All data should be read for BundleDocumentOptionList.");
    }
}
