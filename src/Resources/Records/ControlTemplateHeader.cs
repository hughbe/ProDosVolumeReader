using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// The header structure for a Control Template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct ControlTemplateHeader
{
    /// <summary>
    /// Size of Control Template Header structure in bytes.
    /// </summary>
    public const int Size = 26;

    /// <summary>
    /// Gets the count of parameters in the item template, not including the pcount field.
    /// </summary>
    public ushort ParameterCount { get; }

    /// <summary>
    /// Gets the control ID.
    /// </summary>
    public uint ID { get; }

    /// <summary>
    /// Gets the bounds rectangle for the control.
    /// </summary>
    public RECT Bounds { get; }

    /// <summary>
    /// Gets the control procedure.
    /// </summary>
    public ControlProcedure Procedure { get; }

    /// <summary>
    /// Gets the control flags.
    /// </summary>
    public ControlFlags Flags { get; }

    /// <summary>
    /// Gets the control more flags.
    /// </summary>
    public ControlMoreFlags MoreFlags { get; }

    /// <summary>
    /// Gets the reserved field.
    /// </summary>
    public uint Reserved { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlTemplateHeader"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the Control Template header.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ControlTemplateHeader(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-7 to E-11
        int offset = 0;

        // Count of parameters in the item template, not including the pCount
        // field. Minimum value is 6; maximum value varies depending on the
        // type of control template.
        ParameterCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        if (ParameterCount < 6)
        {
            throw new ArgumentException($"Invalid parameter count: {ParameterCount}", nameof(data));
        }

        // Parameter that sets the ctlID field of the control record for the new
        // control. The ctlID field may be used by the application to provide a
        // straightforward mechanism for keeping track of controls. The control
        // ID is a value assigned by your application, which the control “carries
        // around” for your convenience. Your application can use the ID, which
        // has a known value,to identify a particular control.
        ID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // Parameter that sets the ctlRect field of the control record for the
        // new control. Defines the boundary rectangle for the control.
        Bounds = new RECT(data.Slice(offset, RECT.Size));
        offset += RECT.Size;

        // Parameter that sets the ctlProc field of the control record for the
        // new control. This field contains a reference to the control definition
        // procedure for the control. The value of this field is either a pointer to
        // (or a resource ID for) a control definition procedure or the ID of a
        // standard routine. If the ctlProcRefNotPtr flag in the
        // moreFFlags field is set to 0, then procRef must contain a pointer. If
        // the flag is set to 1, then the Control Manager checks the low-order
        // three bytes of procRef.If these bytes are all zero, then procRef
        // must contain the ID for a standard routine; if these bytes are nonzero,
        // procRef contains the resource ID for a control routine.
        // The standard values are
        // simpleButtonControl $80000000 Simple button
        // checkControl $82000000 Check box
        // iconButtonControl $07FF0001 Icon button
        // editLineControl $83000000 LineEdit
        // listControl $89000000 List
        // pictureControl $8D000000 Picture
        // popUpControl $87000000 Pop-up menu
        // radioControl $84000000 Radio button
        // scrollBarControl $86000000 Scroll bar
        // growControl $88000000 Size box
        // statTextControl $81000000 Static text
        // editTextControl $85000000 TextEdit
        // Note: The procRef value for iconButtonControlis nottruly a standard value but
        // rather the resource ID of the standard control definition procedure for icon
        // buttons.
        Procedure = (ControlProcedure)BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        // A word used to set both ctlHilite and ctlFlag in the control
        // record for the new control. Since this is a word, the bytes for
        // ctlHilite and ctlFlag are reversed. The high-order byte of flag
        // contains ctlHilite,and the low-order byte contains ctlFlag. The
        // bits in flag are mapped as follows:
        // Highlight bits 15-8 Indicates highlighting style.
        // 0 = Control active, no highlighted parts
        // 1-254 = Part code of highlighted part
        // 255 = Control inactive
        // Invisible bit 7 Governsvisibility of control.
        // 0 = Control visible
        // 1 = Control invisible
        // Variable bits 6-0 Values and meaning depend upon
        // control type.
        Flags = new ControlFlags(BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)));
        offset += 2;

        // moreFlags Used to set the ct 1MoreFlagsfield of the control record for the
        // new control.
        // The high-order byte is used by the Control Managerto store its own
        // control information. The low-order byte is used by the control
        // definition procedure to define reference types.
        // The defined Control Managerflags are
        // fCtlTarget $8000 If this flag is set to 1, this control is currently the
        // target of any typing or editing commands.
        // fCtlCanBeTarget $4000 If this flag is set to 1, this control can be made the
        // target control.
        // fCtlWantEvents $2000 If this flag is set to 1, this control can be called when
        // events are passed via the SendEvent ToCt1 Control
        // Managercall. (Note that, if the fct 1canBeTarget
        // flag is set to 1, this control will receive events sent to
        // it regardless of the setting ofthis flag.)
        // fCtlProcRefNotPtr $1000 If this flag is set to 1, then the Control Manager
        // expects procRef to contain the ID or resource ID of
        // a control procedure;ifit is set to 0, then procRef
        // contains a pointer to a custom control procedure.
        // fCtlTellAboutSize $0800 If this flag is set to 1, this control needs to be
        // notified when the size of the owning window has
        // changed.This flag allows custom control procedures
        // to resize their associated control images in response
        // to changes in windowsize.
        // fCtlIsMultiPart $0400 If set to 1, then this is a multipart control. This flag
        // allows control definition procedures to manage
        // multipart controls (necessary since the Control
        // Manager does not know about all the parts of a
        // multipart control).
        // The low-order byte uses the following convention to describe
        // references to color tables and titles (note, though, that some control
        // templates do not follow this convention):
        // titleIsPtr $00 Title reference is by pointer
        // titleIsHandle $01 Title reference is by handle
        // titleIsResource $02 Title reference is by resource ID (resource type
        // corresponds to string type)
        // colorTableIsPtr $00 Color table reference is by pointer
        // colorTableIsHandle $04 Color table reference is by handle
        // colorTableIsResource $08 Color table reference is by resource ID (resource type
        // of rctlColorTbl, $800D)
        MoreFlags = (ControlMoreFlags)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        // Used to set the ct LRefCon field of the control record for the new
        // control. Reserved for application use.
        Reserved = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        offset += 4;

        Debug.Assert(offset == data.Length, "Did not consume all data for ControlTemplateHeader.");
    }
}
