using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// F-Sequence Record
/// </summary>
public readonly struct FSequenceRecord
{
    /// <summary>
    /// Minimum size of an F-Sequence record in bytes.
    /// </summary>
    public const int MinSize = 8;

    /// <summary>
    /// Gets the format of the F-Sequence.
    /// </summary>
    public ushort Format { get; }

    /// <summary>
    /// Gets the tempo of the F-Sequence.
    /// </summary>
    public ushort Tempo { get; }

    /// <summary>
    /// Gets the number of tracks used in the F-Sequence.
    /// </summary>
    public ushort TracksUsed { get; }

    /// <summary>
    /// Gets the total number of tracks in the F-Sequence.
    /// </summary>
    public ushort TracksCount { get; }

    /// <summary>
    /// Gets the list of track IDs in the F-Sequence.
    /// </summary>
    public List<uint> Tracks { get; }

    /// <summary>
    /// Gets the sequence items in the F-Sequence.
    /// </summary>
    public List<uint> SequenceItems { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FSequenceRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw record data.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid or format is unsupported.</exception>
    public FSequenceRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length must be at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L561-L573
        int offset = 0;

        Format = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Format != 0x0001)
        {
            throw new ArgumentException($"Unsupported F-Sequence format: {Format}.", nameof(data));
        }

        Tempo = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        TracksUsed = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        TracksCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (offset + (TracksCount * 4) > data.Length)
        {
            throw new ArgumentException("Data length is insufficient for the specified number of tracks.", nameof(data));
        }

        var tracks = new List<uint>();
        for (int i = 0; i < TracksCount; i++)
        {
            tracks.Add(BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)));
            offset += 4;
        }

        Tracks = tracks;

        var sequenceItems = new List<uint>();
        while (offset + 4 <= data.Length)
        {
            sequenceItems.Add(BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)));
            offset += 4;
        }

        SequenceItems = sequenceItems;

        Debug.Assert(offset <= data.Length, "All data should be consumed.");
    }
}
