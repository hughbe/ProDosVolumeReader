using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Icon Record
/// </summary>
public readonly struct IconRecord
{
    /// <summary>
    /// Minimum size of an Icon Record in bytes.
    /// </summary>
    public const int MinSize = 8;

    /// <summary>
    /// Icon Type
    /// </summary>
    public IconFlags Flags { get; }

    /// <summary>
    /// Size of the Icon Record in bytes.
    /// </summary>
    public ushort Size { get; }

    /// <summary>
    /// Gets the height of the icon in pixels.
    /// </summary>
    public ushort Height { get; }

    /// <summary>
    /// Gets the width of the icon in pixels.
    /// </summary>
    public ushort Width { get; }

    /// <summary>
    /// Gets the Image Data
    /// </summary>
    public byte[] ImageData { get; }

    /// <summary>
    /// Gets the Mask Data
    /// </summary>
    public byte[] MaskData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IconRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Icon Record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public IconRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Data length {data.Length} is less than minimum size {MinSize}.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-48
        int offset = 0;

        // Flags defining the type of icon stored in the icon record.
        // Color indicator bit 15 Indicates whether the icon contains a color or
        // black-and-white image.
        // 0 = Icon is black and white
        // 1 = Icon is color
        Flags = (IconFlags)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The size, in bytes, of the icon image stored at iconImage.
        Size = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The height, in pixels, of the icon.
        Height = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The width,in pixels, of the icon.
        Width = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // iconSize bytes of icon image data.
        if (offset + Size > data.Length)
        {
            throw new ArgumentException("Data length is insufficient for image data.", nameof(data));
        }

        ImageData = data.Slice(offset, Size).ToArray();
        offset += ImageData.Length;

        if (offset + Size > data.Length)
        {
            throw new ArgumentException("Data length is insufficient for mask data.", nameof(data));
        }

        // iconSize bytes of mask data to be applied to the image located at
        // iconImage.
        MaskData = data.Slice(offset, Size).ToArray();
        offset += MaskData.Length;

        Debug.Assert(offset <= data.Length, "Did not consume all data for Icon Record.");
    }
}
