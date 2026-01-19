using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Window Param 2 record.
/// </summary>
public readonly struct WindowParam2Record
{
    /// <summary>
    /// Minimum size of WindowParam2Record structure in bytes.
    /// </summary>
    public const int MinSize = 6;

    /// <summary>
    /// Gets the version.
    /// </summary>
    public ushort Version { get; }

    /// <summary>
    /// Gets the definition procedure handle.
    /// </summary>
    public uint DefinitionProcedureHandle { get; }

    /// <summary>
    /// Gets the definition procedure data.
    /// </summary>
    public byte[] DefinitionProcedureData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowParam2Record"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the WindowParam2Record.</param>
    /// <exception cref="ArgumentException">>Thrown when the data length is invalid.</exception>
    public WindowParam2Record(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-78
        int offset = 0;

        // The resource template version. Must be set to NIL.
        Version = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (Version != 0)
        {
            throw new ArgumentException($"Invalid WindowParam2Record version: {Version}", nameof(data));
        }

        // Pointer to the definition procedure for the window. When using the
        // rWindParam2 window template, you must pass a pointer to a valid
        // definition procedure, either in the template or with the defProcPtr
        // parameter to the NewWindow2 Window Manager tool call. On disk,
        // this field does not contain valid value.
        DefinitionProcedureHandle = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Window definition data required by the routine pointed to by
        // p2DefProc. The format and content of this field are determined by
        // the window definition procedure.
        DefinitionProcedureData = data[offset..].ToArray();
        offset += DefinitionProcedureData.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for WindowParam2Record");
    }
}
