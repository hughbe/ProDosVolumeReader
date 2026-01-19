
/// <summary>
/// Represents the control flags word (ctlHilite and ctlFlag) for a control record.
/// </summary>
public readonly struct ControlFlags
{
    /// <summary>
    /// The raw 16-bit value.
    /// </summary>
    public ushort Value { get; }

    /// <summary>
    /// Gets the highlight style (high byte, bits 8-15).
    /// </summary>
    public byte Highlight => (byte)(Value >> 8);

    /// <summary>
    /// Gets the part code of highlighted part (1-254), or 255 for inactive, or 0 for active/no highlight.
    /// </summary>
    public byte HighlightPartCode => Highlight;

    /// <summary>
    /// Gets whether the control is inactive (highlight == 255).
    /// </summary>
    public bool IsInactive => Highlight == 255;

    /// <summary>
    /// Gets whether the control is visible (bit 7 of low byte == 0).
    /// </summary>
    public bool IsVisible => (Value & 0x80) == 0;

    /// <summary>
    /// Gets the variable bits (bits 0-6 of low byte).
    /// </summary>
    public byte VariableBits => (byte)(Value & 0x7F);

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlFlags"/> struct.
    /// </summary>
    /// <param name="value">The raw 16-bit value.</param>
    public ControlFlags(ushort value)
    {
        Value = value;
    }
}
