using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Tool startup record.
/// </summary>
public readonly struct ToolStartupRecord
{
    /// <summary>
    /// Minimum size of ToolStartupRecord structure in bytes.
    /// </summary>
    public const int MinSize = 12;

    /// <summary>
    /// Gets the flags field.
    /// </summary>
    public ushort Flags { get; }

    /// <summary>
    /// Gets the video mode for QuickDraw II.
    /// </summary>
    public ushort VideoMode { get; }

    /// <summary>
    /// Gets the resource file ID.
    /// </summary>
    public ushort ResourceFileID { get; }

    /// <summary>
    /// Gets the page handle.
    /// </summary>
    public uint PageHandle { get; }

    /// <summary>
    /// Gets the number of tools to be started.
    /// </summary>
    public ushort NumberOfTools { get; }

    /// <summary>
    /// Gets the array of tools to be started.
    /// </summary>
    public List<Tool> Tools { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolStartupRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the ToolStartupRecord.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public ToolStartupRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-69 to E-70
        int offset = 0;

        // Flag word-must be set to 0
        Flags = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Flags != 0)
        {
            throw new ArgumentException($"Invalid ToolStartupRecord flags: {Flags}", nameof(data));
        }

        // Defines the masterSCB for QuickDraw II. See Chapter 43,
        // “QuickDraw II Update,” in this book for valid values.
        VideoMode = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The StartUpTools Call sets this field, which Shut DownTools
        // requires as input.
        ResourceFileID = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The StartUpTools call sets this field, which ShutDownTools
        // requires as input.
        PageHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Each entry defines a toolset to be started. The numTools field
        // specifies the number of entries in this array.
        NumberOfTools = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (offset + (NumberOfTools * Tool.Size) > data.Length)
        {
            throw new ArgumentException($"Data length {data.Length} is insufficient for {NumberOfTools} tools.", nameof(data));
        }

        var tools = new List<Tool>();
        for (int i = 0; i < NumberOfTools; i++)
        {
            tools.Add(new Tool(data.Slice(offset, Tool.Size)));
            offset += Tool.Size;
        }
        
        Tools = tools;

        Debug.Assert(offset <= data.Length, "Did not consume all data for ToolStartupRecord.");
    }
}
