using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a ProDOS date and time.
/// </summary>
public readonly struct ProDosDateTime
{
    /// <summary>
    /// Gets the size of the ProDOS date/time structure in bytes.
    /// </summary>
    public const int Size = 4;

    /// <summary>
    /// Gets the raw 4-byte ProDOS date/time value.
    /// </summary>
    public uint RawData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProDosDateTime"/> struct.
    /// </summary>
    /// <param name="data">The byte data.</param>
    /// <exception cref="ArgumentException">Thrown if the data length is not correct.</exception>
    public ProDosDateTime(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be exactly {Size} bytes long.", nameof(data));
        }

        int offset = 0;

        RawData = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all bytes in ProDosDateTime");
    }

    /// <summary>
    /// Converts the ProDOS date/time to a <see cref="DateTime"/> object.
    /// </summary>
    /// <returns>>The corresponding <see cref="DateTime"/>.</returns>
    public DateTime ToDateTime()
    {
        if (RawData == 0)
        {
            // ProDOS date/time of 0 means "no date"
            return DateTime.MinValue;
        }

        var date = (int)(RawData & 0x0000FFFF);
        var time = (int)((RawData >> 16) & 0x0000FFFF);

        var year = (date >> 9) & 0x7F;
        if (year < 40)
        {
            year += 100;
        }

        var month = (date >> 5) & 0x0F;
        var day = date & 0x1F;
        var minute = time & 0x3F;
        var hour = (time >> 8) & 0x1F;

        return new DateTime(year, month, day, hour, minute, 0);
    }
}
