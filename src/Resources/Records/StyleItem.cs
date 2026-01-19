using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A style item used in text style records.
/// </summary>
public readonly struct StyleItem
{
    /// <summary>
    /// The size of a StyleItem in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// The total number of text characters that use this style.
    /// </summary>
    public uint Length { get; }

    /// <summary>
    /// Offset, in bytes, into the StyleList array to the TEStyle record
    /// </summary>
    public uint Offset { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StyleItem"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the StyleItem.</param>
    /// <exception cref="ArgumentException">Thrown if the data is invalid.</exception>
    public StyleItem(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data length must be exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // 49-55
        int offset = 0;

        // The total numberof text characters that use this style. These
        // characters begin where the previous StyleItem left off. A value of
        // $FFFFFFFF indicates an unused entry.
        Length = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Offset, in bytes, into theStyleList array to the TEStyle record
        // defining the characteristics of the style in question. The styleList
        // array is stored in the TEFormat record.
        Offset = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all StyleItem data.");
    }
}
