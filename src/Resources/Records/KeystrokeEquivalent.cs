using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Keystroke Equivalent within a Control Template Record
/// </summary>
public readonly struct KeystrokeEquivalent
{
    /// <summary>
    /// Size of Keystroke Equivalent in bytes.
    /// </summary>
    public const int Size = 6;

    /// <summary>
    /// Gets the first key ASCII code.
    /// </summary>
    public byte Key1 { get; }

    /// <summary>
    /// Gets the second key ASCII code.
    /// </summary>
    public byte Key2 { get; }

    /// <summary>
    /// Gets the modifiers required for the keystroke equivalent.
    /// </summary>
    public ushort Modifiers { get; }

    /// <summary>
    /// Gets the care bits for the keystroke equivalent.
    /// </summary>
    public ushort CareBits { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeystrokeEquivalent"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Keystroke Equivalent.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public KeystrokeEquivalent(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-12
        int offset = 0;

        // This is the ASCII code for the uppercase or lowercase of the key
        // equivalent.
        Key1 = data[offset];
        offset += 1;

        // This is the ASCII code for the uppercase or lowercase of the key
        // equivalent. Taken with key1, this field completely defines the values
        // against which key equivalents will be tested. If only a single key code
        // is valid, then set key1 and key2 to the samevalue.
        Key2 = data[offset];
        offset += 1;

        // These are the modifiers that must be set to 1 for the equivalence test
        // to pass. The format of this flag word corresponds to that defined for
        // the event record in Chapter 7, “Event Manager,” in Volume 1 of the
        // Toolbox Reference. Note that only the modifiers in the high-order byte
        // are used here.
        Modifiers = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // These are the modifiers that must match for the equivalencetest to
        // pass. The format of this word corresponds to that of
        // keyModifiers. This word allows you to discriminate between
        // double-modified keystrokes. For example, if you want Control-7 to
        // be an equivalent, but not Option-Control-7, you set the controlKey bit
        // in keyModifiers and boththe optionKey and the controlKeybits in
        // keyCareBits to 1. If you want Return and Enter to have the same
        // effect, you should set the keyPadbit to 0.
        CareBits = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for KeystrokeEquivalent.");
    }
}
