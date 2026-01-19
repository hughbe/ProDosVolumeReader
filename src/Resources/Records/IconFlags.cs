namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Flags for icon records.
/// </summary>
[Flags]
public enum IconFlags : ushort
{
	/// <summary>
	/// No flags set.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates whether the icon contains a color or black-and-white image.
	/// 1 = Icon is color, 0 = Icon is black and white.
	/// </summary>
	Color = 1 << 15,
}
