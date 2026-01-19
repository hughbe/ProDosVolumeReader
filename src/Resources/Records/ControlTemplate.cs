using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A control template record in a GS/OS Resource Fork.
/// </summary>
public readonly struct ControlTemplateRecord
{
    /// <summary>
    /// Minimum size of a Control Template record in bytes.
    /// </summary>
    public const int MinSize = ControlTemplateHeader.Size;

    /// <summary>
    /// Gets the header for the control template.
    /// </summary>
    public ControlTemplateHeader Header { get; }

    /// <summary>
    /// Gets additional data specific to the control procedure.
    /// </summary>
    public object? AdditionalData { get; }

    /// <summary>
    /// Initalizes a new instance of the <see cref="ControlTemplateRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the control template.</param>
    /// <exception cref="ArgumentException">Thrown when the data length is invalid.</exception>
    public ControlTemplateRecord(ReadOnlySpan<byte> data)
    {
        if (data.Length < MinSize)
        {
            throw new ArgumentException($"Invalid data length: {data.Length}", nameof(data));
        }

        // Structure documented in file:///Users/hughbellamy/Documents/GitHub/ProDosVolumeReader/docs/Apple_iigs_toolbox_reference_volume_3.pdf
        // E-7 to E-45
        int offset = 0;

        Header = new ControlTemplateHeader(data.Slice(offset, ControlTemplateHeader.Size));
        offset += ControlTemplateHeader.Size;

        if (Header.MoreFlags.HasFlag(ControlMoreFlags.FCtlProcRefNotPtr))
        {
            switch (Header.Procedure)
            {
                case ControlProcedure.SimpleButton:
                {
                    AdditionalData = new SimpleButtonControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.CheckBox:
                {
                    AdditionalData = new CheckBoxControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.IconButton:
                {
                    AdditionalData = new IconButtonControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.LineEdit:
                {
                    AdditionalData = new LineEditControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.List:
                {
                    AdditionalData = new ListControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.Picture:
                {
                    AdditionalData = new PictureControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.PopUp:
                {
                    AdditionalData = new PopUpControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.RadioButton:
                {
                    AdditionalData = new RadioButtonControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.ScrollBar:
                {
                    AdditionalData = new ScrollBarControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.SizeBox:
                {
                    AdditionalData = new SizeBoxControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.StaticText:
                {
                    AdditionalData = new StaticTextControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case ControlProcedure.TextEdit:
                {
                    AdditionalData = new TextEditControlTemplate(data[offset..], Header, out var bytesRead);
                    offset += bytesRead;
                    break;
                }
                case (ControlProcedure)1:
                case (ControlProcedure)0x7FF0002:
                case (ControlProcedure)0x87FF0002:
                case (ControlProcedure)0x87FF0003:
                    break; // Reserved for internal use by Apple, no additional data.
                default:
                    throw new ArgumentException($"Unsupported control procedure: {Header.Procedure}", nameof(data));
            }
        }

        Debug.Assert(offset <= data.Length, "Read beyond end of data.");
    }
}

