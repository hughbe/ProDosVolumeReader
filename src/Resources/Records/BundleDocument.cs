using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Bundle Document structure.
/// </summary>
public readonly struct BundleDocument
{
    /// <summary>
    /// Minimum size of BundleDocument structure in bytes.
    /// </summary>
    public const int MinSize = 8;

    /// <summary>
    /// Gets the size of the BundleDocument structure.
    /// </summary>
    public ushort Size { get; }

    /// <summary>
    /// Gets the offset to the match flags.
    /// </summary>
    public ushort MatchFlagsOffset { get; }

    /// <summary>
    /// Gets the number of results in the document.
    /// </summary>
    public ushort ResultCount { get; }

    /// <summary>
    /// Gets the list of results in the document.
    /// </summary>
    public object Result { get; }

    /// <summary>
    /// Gets the match flags.
    /// </summary>
    public BundleDocumentMatchFlags MatchFlags { get; }

    /// <summary>
    /// Gets the optional match file type (if present).
    /// </summary>
    public ushort? MatchFileType { get; }

    /// <summary>
    /// Gets the optional match aux type (if present).
    /// </summary>
    public BundleDocumentAuxType? MatchAuxType { get; }

    /// <summary>
    /// Gets the optional match file name (if present).
    /// </summary>
    public string? MatchFileName { get; }

    /// <summary>
    /// Gets the optional match creation date (if present).
    /// </summary>
    public BundleDocumentCreationDate? MatchCreationDate { get; }

    /// <summary>
    /// Gets the optional match modification date (if present).
    /// </summary>
    public BundleDocumentModificationDate? MatchModificationDate { get; }

    /// <summary>
    /// Gets the optional match extended information (if present).
    /// </summary>
    public BundleDocumentExtended? MatchExtended { get; }

    /// <summary>
    /// Gets the optional match local access (if present).
    /// </summary>
    public BundleDocumentLocalAccess? MatchLocalAccess { get; }

    /// <summary>
    /// Gets the optional match network access (if present).
    /// </summary>
    public BundleDocumentNetworkAccess? MatchNetworkAccess { get; }

    /// <summary>
    /// Gets the optional match HFS file type (if present).
    /// </summary>
    public uint? MatchHFSFileType { get; }

    /// <summary>
    /// Gets the optional match HFS creator (if present).
    /// </summary>
    public uint? MatchHFSCreator { get; }

    /// <summary>
    /// Gets the list of options (if present).
    /// </summary>
    public BundleDocumentOptionList? MatchOptionList { get; }

    /// <summary>
    /// Gets the optional EOF match (if present).
    /// </summary>
    public BundleDocumentEOF? MatchEOF { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BundleDocument"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the BundleDocument.</param>
    /// <exception cref="ArgumentException">Thrown when data is too small.</exception>
    public BundleDocument(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"BundleDocument requires at least {MinSize} bytes.", nameof(data));
        }

        // Structure documented in https://github.com/ksherlock/prez/blob/master/Types.rez#L1052-L1299
        int offset = 0;

        Size = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Size > data.Length)
        {
            throw new ArgumentException($"BundleDocument size field {Size} exceeds available data length {data.Length}.", nameof(data));
        }

        ReadOnlySpan<byte> documentData = data[..Size];

        MatchFlagsOffset = BinaryPrimitives.ReadUInt16LittleEndian(documentData.Slice(offset, 2));
        offset += 2;

        ResultCount = BinaryPrimitives.ReadUInt16LittleEndian(documentData.Slice(offset, 2));
        offset += 2;

        Result = new BundleDocumentResult(documentData[offset..], ResultCount, out var bytesRead);
        offset += bytesRead;

        Debug.Assert(offset <= documentData.Length, "All fixed fields should be read before accessing MatchFlagsOffset.");

        if (MatchFlagsOffset > Size)
        {
            throw new ArgumentException("MatchFlagsOffset exceeds BundleDocument Size.", nameof(data));
        }

        offset = MatchFlagsOffset;

        MatchFlags = (BundleDocumentMatchFlags)BinaryPrimitives.ReadUInt32LittleEndian(documentData.Slice(offset, 4));
        offset += 4;

        while (offset < Size)
        {
            BundleDocumentMatchKey key = (BundleDocumentMatchKey)BinaryPrimitives.ReadUInt16LittleEndian(documentData.Slice(offset, 2));
            offset += 2;

            switch (key)
            {
                case BundleDocumentMatchKey.Empty:
                    // No data associated with this key.
                    break;
                case BundleDocumentMatchKey.MatchFileType:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.FileType))
                    {
                        throw new ArgumentException("File Type key found but FileType flag is not set.", nameof(data));
                    }

                    MatchFileType = BinaryPrimitives.ReadUInt16LittleEndian(documentData.Slice(offset, 2));
                    offset += 2;
                    break;
                case BundleDocumentMatchKey.MatchAuxType:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.AuxType))
                    {
                        throw new ArgumentException("Aux Type key found but AuxType flag is not set.", nameof(data));
                    }

                    MatchAuxType = new BundleDocumentAuxType(documentData.Slice(offset, BundleDocumentAuxType.Size));
                    offset += BundleDocumentAuxType.Size;
                    break;
                case BundleDocumentMatchKey.MatchFileName:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.FileName))
                    {
                        throw new ArgumentException("File Name key found but FileName flag is not set.", nameof(data));
                    }

                    byte fileNameLength = documentData[offset];
                    offset += 1;

                    if (offset + fileNameLength > documentData.Length)
                    {
                        throw new ArgumentException("File Name length exceeds available data.", nameof(data));
                    }

                    MatchFileName = Encoding.ASCII.GetString(documentData.Slice(offset, fileNameLength));
                    offset += fileNameLength;
                    break;
                case BundleDocumentMatchKey.MatchCreationDate:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.CreateDateTime))
                    {
                        throw new ArgumentException("Creation Date key found but CreateDateTime flag is not set.", nameof(data));
                    }

                    MatchCreationDate = new BundleDocumentCreationDate(documentData.Slice(offset, BundleDocumentCreationDate.Size));
                    offset += BundleDocumentCreationDate.Size;
                    break;
                case BundleDocumentMatchKey.MatchModificationDate:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.ModDateTime))
                    {
                        throw new ArgumentException("Modification Date key found but ModDateTime flag is not set.", nameof(data));
                    }

                    MatchModificationDate = new BundleDocumentModificationDate(documentData.Slice(offset, BundleDocumentModificationDate.Size));
                    offset += BundleDocumentModificationDate.Size;
                    break;
                case BundleDocumentMatchKey.MatchExtended:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.Where))
                    {
                        throw new ArgumentException("Aux Type key found but Where flag is not set.", nameof(data));
                    }

                    MatchExtended = new BundleDocumentExtended(documentData.Slice(offset, BundleDocumentExtended.Size));
                    offset += BundleDocumentExtended.Size;
                    break;
                case BundleDocumentMatchKey.MatchLocalAccess:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.LocalAccess))
                    {
                        throw new ArgumentException("Local Access key found but LocalAccess flag is not set.", nameof(data));
                    }

                    MatchLocalAccess = new BundleDocumentLocalAccess(documentData.Slice(offset, BundleDocumentLocalAccess.Size));
                    offset += BundleDocumentLocalAccess.Size;
                    break;
                case BundleDocumentMatchKey.MatchNetworkAccess:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.NetworkAccess))
                    {
                        throw new ArgumentException("Network Access key found but NetworkAccess flag is not set.", nameof(data));
                    }

                    MatchNetworkAccess = new BundleDocumentNetworkAccess(documentData.Slice(offset, BundleDocumentNetworkAccess.Size));
                    offset += BundleDocumentNetworkAccess.Size;
                    break;
                case BundleDocumentMatchKey.MatchHFSFileType:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.HFSFileType))
                    {
                        throw new ArgumentException("HFS File Type key found but HFSFileType flag is not set.", nameof(data));
                    }

                    MatchHFSFileType = BinaryPrimitives.ReadUInt32LittleEndian(documentData.Slice(offset, 4));
                    offset += 4;
                    break;
                case BundleDocumentMatchKey.MatchHFSCreator:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.HFSCreator))
                    {
                        throw new ArgumentException("HFS Creator key found but HFSCreator flag is not set.", nameof(data));
                    }

                    MatchHFSCreator = BinaryPrimitives.ReadUInt32LittleEndian(documentData.Slice(offset, 4));
                    offset += 4;
                    break;
                case BundleDocumentMatchKey.MatchOptionList:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.OptionList))
                    {
                        throw new ArgumentException("Option List key found but OptionList flag is not set.", nameof(data));
                    }

                    MatchOptionList = new BundleDocumentOptionList(documentData[offset..], out var optionListBytesRead);
                    offset += optionListBytesRead;
                    break;
                case BundleDocumentMatchKey.MatchEOF:
                    if (!MatchFlags.HasFlag(BundleDocumentMatchFlags.EOF))
                    {
                        throw new ArgumentException("EOF key found but EOF flag is not set.", nameof(data));
                    }

                    MatchEOF = new BundleDocumentEOF(documentData.Slice(offset, BundleDocumentEOF.Size));
                    offset += BundleDocumentEOF.Size;
                    break;
                default:
                    throw new NotImplementedException($"Optional field with key {key} is not implemented.");
            }
        }

        Debug.Assert(offset <= documentData.Length, "All fields read should not exceed data length.");
    }
}
