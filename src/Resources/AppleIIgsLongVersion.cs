using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace ProDosVolumeReader.Resources;

/// <summary>
/// Represents an Apple IIgs long version number.
/// </summary>
public readonly struct AppleIIgsLongVersion
{
    /// <summary>
    /// Size of AppleIIgsLongVersion structure in bytes.
    /// </summary>
    public const int Size = 4;

    /// <summary>
    /// Stage kind enumeration.
    /// </summary>
    public enum StageKind : byte
    {
        /// <summary>
        /// Unknown stage.
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// Development stage.
        /// </summary>
        Develop = 0b001,

        /// <summary>
        /// Alpha stage.
        /// </summary>
        Alpha = 0b010,

        /// <summary>
        /// Beta stage.
        /// </summary>
        Beta = 0b011,

        /// <summary>
        /// Final stage.
        /// </summary>
        Final = 0b100,

        /// <summary>
        /// Release stage.
        /// </summary>
        Release = 0b110,
    }

    /// <summary>
    /// Gets the raw 32-bit version value.
    /// </summary>
    public uint RawValue { get; }

    /// <summary>
    /// Gets the major version.
    /// </summary>
    public int Major => 
        // Major is stored in the most-significant byte as BCD (two digits)
        (((int)((RawValue >> 24) & 0xFF) >> 4) * 10) + ((int)((RawValue >> 24) & 0xFF) & 0xF);

    /// <summary>
    /// Gets the minor version.
    /// </summary>
    public int Minor => 
        // Minor is a single BCD digit stored in bits 23..20 (upper nibble of the high-word low byte)
        (((int)((RawValue >> 20) & 0xF) >> 4) * 10) + ((int)((RawValue >> 20) & 0xF) & 0xF);
    
    /// <summary>
    /// Gets the bug fix version.
    /// </summary>
    public int Bug => 
        // Bug is stored in bits 19..16 (lower nibble of the high-word low byte)
        (((int)((RawValue >> 16) & 0xF) >> 4) * 10) + ((int)((RawValue >> 16) & 0xF) & 0xF);

    /// <summary>
    /// Gets the stage kind.
    /// </summary>
    public StageKind Stage =>
        // Stage is stored in bits 15..13 of the low word
        Enum.IsDefined(typeof(StageKind), (StageKind)((RawValue >> 13) & 0x7)) ? (StageKind)((RawValue >> 13) & 0x7) : StageKind.Unknown;

    /// <summary>
    /// Gets the release version.
    /// </summary>
    public int Release => 
        // Release version is a two-digit BCD stored in the low byte
        (((int)(RawValue & 0xFF) >> 4) * 10) + ((int)(RawValue & 0xFF) & 0xF);

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleIIgsLongVersion"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the AppleIIgsLongVersion.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is not exactly 4 bytes.</exception>
    public AppleIIgsLongVersion(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"AppleIIgsLongVersion requires exactly {Size} bytes.", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20050425130030/http://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.100.html
        int offset = 0;

        RawValue = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all bytes for AppleIIgsLongVersion.");
    }
}
