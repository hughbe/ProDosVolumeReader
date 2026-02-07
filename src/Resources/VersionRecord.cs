using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A record containing version information.
/// </summary>
public readonly struct VersionRecord
{
    /// <summary>
    /// Minimum size of VersionRecord structure in bytes.
    /// </summary>
    public const int MinSize = 8;

    /// <summary>
    /// Gets the Apple IIgs long version number.
    /// </summary>
    public AppleIIgsLongVersion Version { get; }

    /// <summary>
    /// Gets the version region.
    /// </summary>
    public VersionRegion Region { get; }

    /// <summary>
    /// Gets the version name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets more info about the version.
    /// </summary>
    public string MoreInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the VersionRecord.</param>
    /// <exception cref="ArgumentException">Thrown if the data is invalid.</exception>
    public VersionRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"VersionRecord requires at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.076.html
        int offset = 0;

        // The application's version number, in Apple IIgs Long Version format.
        // See Apple IIgs Technical Note #100, VersionVille, for more details.
        Version = new AppleIIgsLongVersion(data.Slice(offset, AppleIIgsLongVersion.Size));
        offset += AppleIIgsLongVersion.Size;

        // An international country version code. Possible values are as follows:
        // verUS: 0
        // verFrance: 1
        // verBritain: 2
        // verGermany: 3
        // verItaly: 4
        // verNetherlands: 5
        // verBelgiumLux: 6
        // verSweden: 7
        // verSpain: 8
        // verDenmark: 9
        // verPortugal: 10
        // verFrCanada: 11
        // verNorway: 12
        // verIsrael: 13
        // verJapan: 14
        // verAustralia: 15
        // verArabia: 16
        // verFinland: 17
        // verFrSwiss: 18
        // verGrSwiss: 19
        // verGreece: 20
        // verIceland: 21
        // verMalta: 22
        // verCyprus: 23
        // verTurkey: 24
        // verYugoslavia: 25
        // verIreland: 50
        // verKorea: 51
        // verChina: 52
        // verTaiwan: 53
        // verThailand: 54
        Region = (VersionRegion)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Pascal string containing the desired name. May be the null string.
        byte nameLength = data[offset];
        offset += 1;

        if (offset + nameLength > data.Length)
        {
            throw new ArgumentException("VersionRecord contains invalid name length.", nameof(data));
        }

        Name = Encoding.ASCII.GetString(data.Slice(offset, nameLength));
        offset += nameLength;

        // Additional information to be displayed, such as a copyright notice.
        // May be the null string. Recommended maximum length is about two lines
        // of 35 characters each. May contain a carriage return (character $0D).
        byte moreInfoLength = data[offset];
        offset += 1;

        if (offset + moreInfoLength > data.Length)
        {
            throw new ArgumentException("VersionRecord contains invalid more info length.", nameof(data));
        }

        MoreInfo = Encoding.ASCII.GetString(data.Slice(offset, moreInfoLength));
        offset += moreInfoLength;

        Debug.Assert(offset <= data.Length, "Did not consume all VersionRecord data.");
    }
}
