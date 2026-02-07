using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// CDEV Flags Record structure in a Resource Fork.
/// </summary>
public readonly struct CDEVFlagsRecord
{
    /// <summary>
    /// Size of CDEV Flags Record in bytes.
    /// </summary>
    public const int Size = 72;

    /// <summary>
    /// Gets the flags.
    /// </summary>
    public ushort Flags { get; }

    /// <summary>
    /// Gets a value indicating whether the CDEV is enabled.
    /// </summary>
    public byte Enabled { get; }

    /// <summary>
    /// Gets the version.
    /// </summary>
    public byte Version { get; }

    /// <summary>
    /// Gets the machine type.
    /// </summary>
    public byte Machine { get; }

    /// <summary>
    /// Gets the reserved byte.
    /// </summary>
    public byte Reserved { get; }

    /// <summary>
    /// Gets the data rectangle.
    /// </summary>
    public RECT DataRectangle { get; }

    /// <summary>
    /// Gets the CDEV name.
    /// </summary>
    public string CDEVName { get; }

    /// <summary>
    /// Gets the author name.
    /// </summary>
    public string AuthorName { get; }

    /// <summary>
    /// Gets the version name.
    /// </summary>
    public string VersionName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CDEVFlagsRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the CDEV Flags Record.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public CDEVFlagsRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < Size)
        {
            throw new ArgumentException($"Data length must be at least {Size} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L627-L638
        int offset = 0;

        Flags = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Enabled = data[offset];
        offset += 1;

        Version = data[offset];
        offset += 1;

        Machine = data[offset];
        offset += 1;

        Reserved = data[offset];
        offset += 1;

        DataRectangle = new RECT(data.Slice(offset, RECT.Size));
        offset += RECT.Size;

        byte cdevNameLength = data[offset];
        offset += 1;

        if (cdevNameLength > 15)
        {
            throw new ArgumentException("CDEV name length exceeds maximum of 15 characters.", nameof(data));
        }

        CDEVName = Encoding.ASCII.GetString(data.Slice(offset, cdevNameLength));
        offset += 15;

        byte authorNameLength = data[offset];
        offset += 1;

        if (authorNameLength > 32)
        {
            throw new ArgumentException("Author name length exceeds maximum of 32 characters.", nameof(data));
        }

        AuthorName = Encoding.ASCII.GetString(data.Slice(offset, authorNameLength));
        offset += 32;

        byte versionNameLength = data[offset];
        offset += 1;

        if (versionNameLength > 8)
        {
            throw new ArgumentException("Version name length exceeds maximum of 8 characters.", nameof(data));
        }
        
        VersionName = Encoding.ASCII.GetString(data.Slice(offset, versionNameLength));
        offset += 8;

        Debug.Assert(offset <= data.Length, "Did not consume all data for CDEVFlagsRecord");
    }
}
