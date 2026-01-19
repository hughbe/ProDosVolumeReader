using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing a sampled sound.
/// </summary>
public readonly struct SampledSoundRecord
{
    /// <summary>
    /// Minimum size of SampledSoundRecord structure in bytes.
    /// </summary>
    public const int MinSize = 10;

    /// <summary>
    /// Gets the format of the sampled sound.
    /// </summary>
    public ushort Format { get; }

    /// <summary>
    /// Gets the wave size in pages.
    /// </summary>
    public ushort WaveSize { get; }

    /// <summary>
    /// Gets the relative pitch.
    /// </summary>
    public ushort RelativePitch { get; }

    /// <summary>
    /// Gets the stereo setting.
    /// </summary>
    public ushort Stereo { get; }

    /// <summary>
    /// Gets the sample rate in Hertz.
    /// </summary>
    public ushort SampleRate { get; }

    /// <summary>
    /// Gets the sampled sound data.
    /// </summary>
    public byte[] SoundData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SampledSoundRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the SampledSoundRecord.</param>
    public SampledSoundRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"SampledSoundRecord requires at least {MinSize} bytes.", nameof(data));
        }
        
        // Structure documented in https://mirrors.apple2.org.za/Apple%20II%20Documentation%20Project/Computers/Apple%20II/Apple%20IIGS/Documentation/Apple%20IIGS%20Technical%20Notes%2065-79.pdf
        int offset = 0;

        // This must always be zero.
        Format = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Format != 0)
        {
            throw new NotSupportedException($"Unsupported SampledSoundRecord format: {Format}");
        }

        // Sample size in pages (256 bytes per page). For example, an 8K sample takes 32 pages; a 128K sample requires $200 pages.
        WaveSize = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The high byte of this word is a semitone value; the low byte is a fractional
        // semitone. These values are used to tune the sample to correct pitch. See
        // HyperCard IIgs Technical Note #3, Pitching Sampled Sound.
        RelativePitch = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The output channel for this sound is in the low nibble of this word.
        Stereo = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The sampling rate of the sound, in Hertz (Hz).
        SampleRate = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The sampled sound data. The bytes are all 8-bit samples. The sample starts here and continues until the end of the resource.
        SoundData = data[offset..].ToArray();
        offset += SoundData.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for SampledSoundRecord.");
    }
}
