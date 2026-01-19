using System;

namespace ProDosVolumeReader.Resources.Records
{
    /// <summary>
    /// Cursor flags (16-bit). Only bit 7 is defined:
    /// Bit 7: 1 = 640 Mode, 0 = 320 Mode. All other bits are reserved and must be zero.
    /// Stored as a 16-bit value in the resource; use the underlying type `ushort`.
    /// </summary>
    [Flags]
    public enum CursorFlags : ushort
    {
        /// <summary>No flags set (320 mode).</summary>
        None = 0,

        /// <summary>640 mode (bit 7).</summary>
        Mode640 = 1 << 7,
    }
}
