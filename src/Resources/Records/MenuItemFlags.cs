using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bitfield struct for menu item display attributes.
/// </summary>
public readonly struct MenuItemFlags
{
    /// <summary>
    /// Size of MenuItemFlags structure in bytes.
    /// </summary>
	public const int Size = 2;

    /// <summary>
    /// The raw ushort value of the flags.
    /// </summary>
	public ushort Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemFlags"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the MenuItemFlags.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
	public MenuItemFlags(ReadOnlySpan<byte> data)
	{
		if (data.Length != Size)
        {
			throw new ArgumentException($"MenuItemFlags requires {Size} bytes.", nameof(data));
        }

        // Structure documented in https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
        // E-54 to E-55
        int offset = 0;

		Value = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for MenuItemFlags.");
	}

	/// <summary>
	/// Title reference type (bits 14-15)
	/// </summary>
	public ReferenceType TitleRefType => (ReferenceType)((Value >> 14) & 0x3);

	/// <summary>
	/// Shadow (bit 12)
	/// </summary>
	public bool Shadow => (Value & (1 << 12)) != 0;

	/// <summary>
	/// Outline (bit 11)
	/// </summary>
	public bool Outline => (Value & (1 << 11)) != 0;

	/// <summary>
	/// Disabled (bit 7)
	/// </summary>
	public bool Disabled => (Value & (1 << 7)) != 0;

	/// <summary>
	/// Divider (bit 6)
	/// </summary>
	public bool Divider => (Value & (1 << 6)) != 0;

	/// <summary>
	/// XOR highlight (bit 5)
	/// </summary>
	public bool XorHighlight => (Value & (1 << 5)) != 0;

	/// <summary>
	/// Underline (bit 2)
	/// </summary>
	public bool Underline => (Value & (1 << 2)) != 0;

	/// <summary>
	/// Italic (bit 1)
	/// </summary>
	public bool Italic => (Value & (1 << 1)) != 0;
}
