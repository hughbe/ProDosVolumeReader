namespace ProDosVolumeReader.Resources.Records;
 
/// <summary>
/// Represents the type of a list control in a resource.
/// </summary>
public enum ListControlType : ushort
{
	/// <summary>
	/// No flags set. Reserved bits must be zero.
	/// </summary>
	None = 0,

	/// <summary>
	/// Defines the type of strings used to define list items.
	/// 0 = Pascal strings, 1 = C strings ($00-terminated).
	/// Maps to bit 0 (0x0001).
	/// </summary>
	fListString = 1 << 0,

	/// <summary>
	/// Controls type of selection options available to the user.
	/// 0 = Arbitrary and range selection allowed, 1 = Only single selection allowed.
	/// Maps to bit 1 (0x0002).
	/// </summary>
	fListSelect = 1 << 1,

	/// <summary>
	/// Allows control of where the scroll bar for the list is drawn.
	/// 0 = Scroll bar drawn on outside of boundary rectangle.
	/// 1 = Scroll bar drawn on inside of boundary rectangle.
	/// Maps to bit 2 (0x0004).
	/// </summary>
	fListScrollBar = 1 << 2,
}
