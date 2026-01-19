using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Tool within a <see cref="ToolStartupRecord"/>.
/// </summary>
public readonly struct Tool
{
    /// <summary>
    /// Size of the Tool record in bytes.
    /// </summary>
    public const int Size = 4;

    /// <summary>
    /// Gets the tool number.
    /// </summary>
    public ushort ToolNumber { get; }

    /// <summary>
    /// Gets the minimum version required for the tool.
    /// </summary>
    public ushort MinVersion { get;  }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tool"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Tool record.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public Tool(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Tool record requires {Size} bytes.", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-70
        int offset = 0;

        // The tool set to be loaded. Valid tool set numbers are discussed
        // in Chapter 51, “Tool Locator Update,” in this book.
        ToolNumber = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // The minimum acceptable version for the tool set. See
        // Chapter 24, “Tool Locator,” in Volume 2 of the Toolbox Reference
        // for the format of this field.
        MinVersion = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not consume all data for Tool record.");
    }
}
