using System.Buffers.Binary;
using System.Diagnostics;
using ProDosVolumeReader.Resources;
using ProDosVolumeReader.Resources.Records;

namespace ProDosVolumeReader.Tests;

public class ProDiskVolumeTests
{
    public static TheoryData<string> DiskImages => new()
    {
        "IIgsChildrensWritingCenter.dsk.po",
        "IIgsChildrensWritingCenterStorage2.dsk.po",
        //"appleIISetup.dsk.po",
        "brccGs77PrintShopGS.dsk.po",
        "brccGs78PrintshopFiles.dsk.po",
        "fonts.dsk.po",
        "gs-os5.02.dsk.po",
        "gs-os5.02SystemTools.dsk.po",
        "install.dsk.po",
        //"paulsGP_MIMDBackupEmu_disk.dsk.po",
        //"paulsMaster.dsk.po",
        "pdeGraphicsEducationThemes.dsk.po",
        "printShopGS1.1.dsk.po",
        "printshopGSGraphics.dsk.po",
        "printshopGSGraphicsFromSoftDisk13-19.dsk.po",
        "printshopGSSpecialEditionGraphics.dsk.po",
        "psGraphics.dsk.po",
        "psGraphicsSignsPeopleMapsSportsAnimalsMix.dsk.po",
        "psGraphicsSportsHobbiesGames.dsk.po",
        "psGraphicsSymbolsEmblemsLogos.dsk.po",
        //"sdasCopyFromJose.dsk.po",
        "shanghai.dsk.po",
        "systemDisk.dsk.po",
        "systemTools2.dsk.po",
        "whereInTimeIsCS6T.dsk.po",
        //TODO: "whereInTimeIsCarmenSandiego.dsk.po",
        "gsosinstalldisks/INSTALL.dsk",
        "gsosinstalldisks/SYSTEM.DISK.dsk",
        "gsosinstalldisks/SYSTEMTOOLS1.dsk",
        "gsosinstalldisks/SYSTEMTOOLS2.dsk",
        "gsosinstalldisks/FONTS.dsk",
        "gsosinstalldisks/SYNTHLAB.dsk",
        //"ProDOS_2_4.dsk", // TODO: TRID says it's ProDOS volume, but fails to open.
        "Word Perfect 2.1/Word Perfect Demo.po"
    };

    [Theory]
    [MemberData(nameof(DiskImages))]
    public void Ctor_Stream(string diskName)
    {
        var filePath = Path.Combine("Samples", diskName);
        using var stream = File.OpenRead(filePath);
        var image = new ProDiskVolume(stream);

        // Create output directory based on disk name (without extension)
        string diskNameWithoutExtension = Path.GetFileNameWithoutExtension(diskName);
        string outputDirectory = Path.Combine("Output", diskNameWithoutExtension);

        // Delete the output directory if it exists
        if (Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, true);
        }

        foreach (var entry in image.EnumerateRootContents())
        {
            ExportEntry(image, entry, outputDirectory);
        }
    }

    private static void ExportEntry(ProDiskVolume disk, FileEntry entry, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);

        string safeFileName = string.Join("_", entry.FileName.Split(Path.GetInvalidFileNameChars()));
        string outputFilePath = Path.Combine(outputDirectory, safeFileName);

        if (entry.StorageType == StorageType.Subdirectory)
        {
            foreach (var subEntry in disk.EnumerateSubdirectory(entry))
            {
                ExportEntry(disk, subEntry, outputFilePath);
            }
        }
        else if (entry.StorageType == StorageType.GSOSForkedFile)
        {
            Debug.WriteLine($"Extracting GS/OS forked file: {entry.FileName} ({entry.StorageType} and {entry.Eof} bytes) to {outputFilePath}");
            using var dataForkStream = File.Create(outputFilePath + ".data");
            disk.GetDataFork(entry, dataForkStream);

            using var resourceForkStream = File.Create(outputFilePath + ".res");
            disk.GetResourceFork(entry, resourceForkStream);

            // Read the resource fork.
            resourceForkStream.Seek(0, SeekOrigin.Begin);
            Span<byte> buffer = stackalloc byte[4];
            if (resourceForkStream.Length == 0)
            {
                Debug.WriteLine($"Skipping Resource Fork for {entry.FileName} as it is empty.");
                return;
            }

            resourceForkStream.ReadExactly(buffer);
            uint version = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
            if (version > 127)
            {
                Debug.WriteLine($"Resource Fork {entry.FileName} has Mac format (version {version}), skipping parse.");
                return;
            }

            resourceForkStream.Seek(0, SeekOrigin.Begin);

            GsOsResourceFork resourceFork;
            try
            {
                resourceFork = new GsOsResourceFork(resourceForkStream);
            }
            catch (Exception ex) when (!(ex is FormatException) && !(ex is NotImplementedException))
            {
                Debug.WriteLine($"Failed to parse Resource Fork for {entry.FileName}.");
                throw;
            }

            Debug.WriteLine($"Successfully parsed Resource Fork for {entry.FileName} with {resourceFork.Map.ReferenceRecords.Length} resource index records.");
            foreach (var record in resourceFork.Map.ReferenceRecords)
            {
                Debug.WriteLine($"  Resource Type: {record.Type}, ID: {record.ResourceID}, Offset: {record.DataOffset}, Size: {record.DataSize}, Attr: {record.Attributes}, PurgeLevel: {record.PurgeLevel}");

                // Export the binary data for the resource.
                var resourceOutputPath = $"{outputFilePath}.id_{record.ResourceID}";
                Debug.WriteLine($"    Extracting Resource to {resourceOutputPath}");
                byte[] resourceData = resourceFork.GetResourceData(record);
                File.WriteAllBytes(resourceOutputPath, resourceData);

                switch (record.Type)
                {
                    case GsOsResourceForkType.Icon:
                    {
                        var iconRecord = new IconRecord(resourceData);
                        Debug.WriteLine($"    Icon Record: Width={iconRecord.Width}, Height={iconRecord.Height}, Size={iconRecord.Size} bytes");
                        break;
                    }

                    case GsOsResourceForkType.Picture:
                    {
                        var pictureRecord = new PictureRecord(resourceData);
                        Debug.WriteLine($"    Picture Record: ScanLineControlByte=0x{pictureRecord.ScanLineControlByte:X4}, BoundaryRectangle=({pictureRecord.BoundaryRectangle.Left},{pictureRecord.BoundaryRectangle.Top},{pictureRecord.BoundaryRectangle.Right},{pictureRecord.BoundaryRectangle.Bottom}), Version={pictureRecord.Version}");
                        break;
                    }

                    case GsOsResourceForkType.ControlList:
                    {
                        var controlListRecord = new ControlListRecord(resourceData);
                        Debug.WriteLine($"    Control List Record: ControlCount={controlListRecord.Controls.Count}");
                        foreach (var control in controlListRecord.Controls)
                        {
                            Debug.WriteLine($"      Control: ID={control}");
                        }

                        break;
                    }

                    case GsOsResourceForkType.ControlTemplate:
                    {
                        var controlTemplateRecord = new ControlTemplateRecord(resourceData);
                        Debug.WriteLine($"    Control Template Record: ID={controlTemplateRecord.Header.ID}, Procedure={controlTemplateRecord.Header.Procedure}, Flags=0x{controlTemplateRecord.Header.Flags.Value:X4}, MoreFlags=0x{(int)controlTemplateRecord.Header.MoreFlags:X4}, ParameterCount={controlTemplateRecord.Header.ParameterCount}");
                        break;
                    }

                    case GsOsResourceForkType.Class1InputString:
                    {
                        var class1InputStringRecord = new Class1InputStringRecord(resourceData);
                        Debug.WriteLine($"    Class 1 Input String Record: Length={class1InputStringRecord.Length}, String='{class1InputStringRecord.StringCharacters}'");
                        break;
                    }

                    case GsOsResourceForkType.PascalString:
                    {
                        var pascalStringRecord = new PascalStringRecord(resourceData);
                        Debug.WriteLine($"    Pascal String: '{pascalStringRecord.StringCharacters}'");
                        break;
                    }

                    case GsOsResourceForkType.StringList:
                    {
                        var stringListRecord = new StringListRecord(resourceData);
                        Debug.WriteLine($"    String List Record: StringCount={stringListRecord.Strings.Count}");
                        foreach (var str in stringListRecord.Strings)
                        {
                            Debug.WriteLine($"      String: '{str}'");
                        }

                        break;
                    }

                    case GsOsResourceForkType.ToolStartup:
                    {
                        var toolStartupRecord = new ToolStartupRecord(resourceData);
                        Debug.WriteLine($"    Tool Startup Record: Flags=0x{toolStartupRecord.Flags:X4}, VideoMode=0x{toolStartupRecord.VideoMode:X4}, ResourceFileID=0x{toolStartupRecord.ResourceFileID:X4}, PageHandle=0x{toolStartupRecord.PageHandle:X8}, NumberOfTools={toolStartupRecord.NumberOfTools}");
                        foreach (var tool in toolStartupRecord.Tools)
                        {
                            Debug.WriteLine($"      Tool: Number=0x{tool.ToolNumber:X4}, MinVersion=0x{tool.MinVersion:X4}");
                        }

                        break;
                    }

                    case GsOsResourceForkType.ResourceName:
                    {
                        var resourceNameRecord = new ResourceNameRecord(resourceData);
                        Debug.WriteLine($"    Resource Name: Version={resourceNameRecord.Version}, Count={resourceNameRecord.NameCount})");
                        foreach (var name in resourceNameRecord.Names)
                        {
                            Debug.WriteLine($"      Name: '{name}'");
                        }

                        break;
                    }

                    case GsOsResourceForkType.MenuBar:
                    {
                        var menuBarRecord = new MenuBarRecord(resourceData);
                        Debug.WriteLine($"    Menu Bar Record: Version={menuBarRecord.Version}, Flags=0x{menuBarRecord.Flags.RawValue:X4}, MenusReferenceCount={menuBarRecord.MenuReferences.Count}");
                        foreach (var menuRef in menuBarRecord.MenuReferences)
                        {
                            Debug.WriteLine($"      Menu Reference: 0x{menuRef:X8}");
                        }

                        break;
                    }

                    case GsOsResourceForkType.Menu:
                    {
                        var menuRecord = new MenuRecord(resourceData);
                        Debug.WriteLine($"    Menu Record: ID={menuRecord.ID}, Version={menuRecord.Version}, Flags=0x{menuRecord.Flags.Value:X4}, TitleRef={menuRecord.TitleReference}, ItemReferences={menuRecord.ItemReferences.Count}");
                        foreach (var itemRef in menuRecord.ItemReferences)
                        {
                            Debug.WriteLine($"      Item Reference: 0x{itemRef:X8}");
                        }

                        break;
                    }

                    case GsOsResourceForkType.MenuItem:
                    {
                        var menuItemRecord = new MenuItemRecord(resourceData);
                        Debug.WriteLine($"    Menu Item Record: ID={menuItemRecord.MenuItemID}, Version={menuItemRecord.Version}, PrimaryKeystroke=0x{menuItemRecord.PrimaryKeystrokeEquivalentCharacter:X2}, AlternateKeystroke=0x{menuItemRecord.AlternateKeystrokeEquivalentCharacter:X2}, CheckmarkChar='{menuItemRecord.ItemCheckmarkCharacter}', Flags=0x{menuItemRecord.Flags.Value:X4}, TitleRef={menuItemRecord.TitleReference}");
                        break;
                    }

                    case GsOsResourceForkType.ControlDefinitionProcedure:
                    {
                        var codeRecord = new CodeRecord(resourceData);
                        Debug.WriteLine($"    Control Definition Procedure Record: CodeLength={codeRecord.Data.Length}");
                        break;
                    }

                    case GsOsResourceForkType.WindowParam1:
                    {
                        var windowParam1Record = new WindowParam1Record(resourceData);
                        Debug.WriteLine($"    Window Param1 Record: Length={windowParam1Record.Length} bytes, Frame=0x{windowParam1Record.Frame:X2} TitleReference={windowParam1Record.TitleReference}, InfoTextHeight={windowParam1Record.InfoTextHeight}, FrameDefinitionProcedure=0x{windowParam1Record.FrameDefinitionProcedure:X8}, InfoTextDefinitionProcedure=0x{windowParam1Record.InfoTextDefinitionProcedure:X8}, ContentDefinitionProcedure=0x{windowParam1Record.ContentDefinitionProcedure:X8}, Position=({windowParam1Record.Position.Left},{windowParam1Record.Position.Top},{windowParam1Record.Position.Right},{windowParam1Record.Position.Bottom}), Plane={windowParam1Record.Plane}, ControlTemplateReference={windowParam1Record.ControlTemplateReference}, ReferenceTypes=0x{windowParam1Record.ReferenceTypes.RawValue:X4}");
                        break;
                    }

                    case GsOsResourceForkType.WindowParam2:
                    {
                        var windowParam2Record = new WindowParam2Record(resourceData);
                        Debug.WriteLine($"    Window Param2 Record: Version={windowParam2Record.Version}, DefinitionProcedureHandle=0x{windowParam2Record.DefinitionProcedureHandle:X8}, DefinitionProcedureData Length={windowParam2Record.DefinitionProcedureData.Length} bytes");
                        break;
                    }

                    case GsOsResourceForkType.WindowColorTable:
                    {
                        var windowColorTableRecord = new WindowColorTableRecord(resourceData);
                        Debug.WriteLine($"    Window Color Table Record: FrameColor={windowColorTableRecord.FrameColor}, TitleColor={windowColorTableRecord.TitleColor}, BarColor={windowColorTableRecord.BarColor}, GrowColor={windowColorTableRecord.GrowColor}, InfoColor={windowColorTableRecord.InfoColor}");
                        break;
                    }

                    case GsOsResourceForkType.TextBlock:
                    {
                        var textBlockRecord = new TextBlockRecord(resourceData);
                        Debug.WriteLine($"    Text Block Record: Length={textBlockRecord.StringCharacters.Length} characters");
                        break;
                    }

                    case GsOsResourceForkType.TextEditStyle:
                    {
                        var textStyleRecord = new TextStyleRecord(resourceData);
                        Debug.WriteLine($"    Text Style Record: TEFormat Version={textStyleRecord.Format.Version}, RulerCount={textStyleRecord.Format.Rulers.Count}, StyleCount={textStyleRecord.Format.Styles.Count}, StyleItemCount={textStyleRecord.Format.NumberOfStyles}");
                        foreach (var ruler in textStyleRecord.Format.Rulers)
                        {
                            Debug.WriteLine($"      Ruler: LeftMargin={ruler.LeftMargin}, LeftIndent={ruler.LeftIndent}, RightMargin={ruler.RightMargin}, Justification={ruler.TabType}, TabStopCount={ruler.TabStops?.Count ?? 0}, TabTerminator={ruler.TabTerminator}");
                            foreach (var tabStop in ruler.TabStops ?? Enumerable.Empty<ushort>())
                            {
                                Debug.WriteLine($"        Tab Stop: {tabStop}");
                            }
                        }
                        foreach (var style in textStyleRecord.Format.Styles)
                        {
                            Debug.WriteLine($"      Style: FontID={style.FontID}, ForegroundColor=0x{style.ForegroundColor:X4}, BackgroundColor=0x{style.BackgroundColor:X4}, UserData=0x{style.UserData:X8}");
                        }
                        foreach (var styleItem in textStyleRecord.Format.StyleItems)
                        {
                            Debug.WriteLine($"      Style Item: Length={styleItem.Length}, Offset={styleItem.Offset}");
                        }

                        break;
                    }

                    case GsOsResourceForkType.Code:
                    {
                        var codeRecord = new CodeRecord(resourceData);
                        Debug.WriteLine($"    Code Record: CodeLength={codeRecord.Data.Length}");
                        break;
                    }

                    case GsOsResourceForkType.TextForLETextBox2:
                    {
                        var textBox2Record = new TextForLETextBox2Record(resourceData);
                        Debug.WriteLine($"    Text Box 2 Record: Length={textBox2Record.Length} bytes");
                        break;
                    }

                    case GsOsResourceForkType.ControlColorTable:
                    {
                        var controlColorTableRecord = new ControlColorTableRecord(resourceData);
                        Debug.WriteLine($"    Control Color Table Record: DataLength={controlColorTableRecord.Data.Length} bytes");
                        break;
                    }

                    case GsOsResourceForkType.AlertString:
                    {
                        var alertStringRecord = new AlertStringRecord(resourceData);
                        Debug.WriteLine($"    Alert String Record: '{alertStringRecord.Message}'");
                        break;
                    }

                    case GsOsResourceForkType.Text:
                    {
                        var textRecord = new TextRecord(resourceData);
                        Debug.WriteLine($"    Text Record: '{textRecord.StringCharacters}'");
                        break;
                    }

                    case GsOsResourceForkType.CDEVCode:
                    {
                        var codeRecord = new CodeRecord(resourceData);
                        Debug.WriteLine($"    CDEV Code Record: CodeLength={codeRecord.Data.Length}");
                        break;
                    }

                    case GsOsResourceForkType.CDEVFlags:
                    {
                        var cdevFlagsRecord = new CDEVFlagsRecord(resourceData);
                        Debug.WriteLine($"    CDEV Flags Record: Flags=0x{cdevFlagsRecord.Flags:X4}, Enabled={cdevFlagsRecord.Enabled}, Version={cdevFlagsRecord.Version}, Machine={cdevFlagsRecord.Machine}, Reserved={cdevFlagsRecord.Reserved}, DataRectangle={cdevFlagsRecord.DataRectangle}, CDEVName='{cdevFlagsRecord.CDEVName}', AuthorName='{cdevFlagsRecord.AuthorName}', VersionName='{cdevFlagsRecord.VersionName}'");
                        break;
                    }

                    case GsOsResourceForkType.TwoRects:
                    {
                        var twoRectsRecord = new TwoRectsRecord(resourceData);
                        Debug.WriteLine($"    Two Rects Record: Rect1=({twoRectsRecord.First}, Rect2=({twoRectsRecord.Second})");
                        break;
                    }

                    case GsOsResourceForkType.FileType:
                    {
                        var fileTypeRecord = new FileTypeRecord(resourceData);
                        Debug.WriteLine($"    File Type Record: MajorVersion={fileTypeRecord.Header.MajorVersion}, MinorVersion={fileTypeRecord.Header.MinorVersion}, Flags={fileTypeRecord.Header.Flags}, NumberOfEntries={fileTypeRecord.Header.NumberOfEntries}, Reserved={fileTypeRecord.Header.Reserved}, IndexRecordSize={fileTypeRecord.Header.IndexRecordSize} FirstIndexEntryOffset={fileTypeRecord.Header.FirstIndexEntryOffset}, IndexEntries Count={fileTypeRecord.IndexEntries.Count}, Descriptions Count={fileTypeRecord.Descriptions.Count}");
                        break;
                    }

                    case GsOsResourceForkType.ListControls:
                    {
                        var listControlsRecord = new ListControlsRecord(resourceData);
                        Debug.WriteLine($"    List Controls Record: ControlCount={listControlsRecord.Controls.Count}");
                        foreach (var control in listControlsRecord.Controls)
                        {
                            Debug.WriteLine($"      Control: ID={control.ResourceID}, Flags=0x{control.Flags.RawValue:X4}, DataLength={control.Data.Length} bytes");
                        }

                        break;
                    }

                    case GsOsResourceForkType.CString:
                    {
                        var cStringRecord = new CStringRecord(resourceData);
                        Debug.WriteLine($"    C String Record: '{cStringRecord.StringCharacters}'");
                        break;
                    }

                    case GsOsResourceForkType.XCMD:
                    {
                        var xcmdRecord = new CodeRecord(resourceData);
                        Debug.WriteLine($"    XCMD Record: CodeLength={xcmdRecord.Data.Length}");
                        break;
                    }
                    
                    case GsOsResourceForkType.XFCN:
                    {
                        var xfcnRecord = new CodeRecord(resourceData);
                        Debug.WriteLine($"    XFCN Record: CodeLength={xfcnRecord.Data.Length}");
                        break;
                    }

                    case GsOsResourceForkType.ErrorString:
                    {
                        var errorStringRecord = new ErrorStringRecord(resourceData);
                        Debug.WriteLine($"    Error String Record: '{errorStringRecord.Message}'");
                        break;
                    }

                    case GsOsResourceForkType.KeystrokeTranslationTable:
                    {
                        var keystrokeTranslationTableRecord = new KeystrokeTranslationTableRecord(resourceData);
                        Debug.WriteLine($"    Keystroke Translation Table Record: TranslationTable Length={keystrokeTranslationTableRecord.TranslationTable.Length} bytes, DeadKeyValidation Count={keystrokeTranslationTableRecord.DeadKeyValidationArray.Count}, DeadKeyReplacement Count={keystrokeTranslationTableRecord.DeadKeyReplacementArray.Count}");
                        break;
                    }

                    case GsOsResourceForkType.WideString:
                    {
                        // Duplicates the Class1OutputStringRecord.
                        var wideStringRecord = new Class1OutputStringRecord(resourceData);
                        Debug.WriteLine($"    Wide String Record: Length={wideStringRecord.StringLength}, String='{wideStringRecord.StringCharacters}'");
                        break;
                    }

                    case GsOsResourceForkType.Class1OutputString:
                    {
                        var class1OutputStringRecord = new Class1OutputStringRecord(resourceData);
                        Debug.WriteLine($"    Class 1 Output String Record: Length={class1OutputStringRecord.StringLength}, String='{class1OutputStringRecord.StringCharacters}'");
                        break;
                    }

                    case GsOsResourceForkType.SampledSound:
                    {
                        var sampledSoundRecord = new SampledSoundRecord(resourceData);
                        Debug.WriteLine($"    Sampled Sound Record: SampleRate={sampledSoundRecord.SampleRate} Hz, Stereo={sampledSoundRecord.Stereo}, WaveSize={sampledSoundRecord.WaveSize} pages, SoundDataLength={sampledSoundRecord.SoundData.Length} bytes");
                        break;
                    }

                    case GsOsResourceForkType.TextEditRuler:
                    {
                        var textEditRulerRecord = new TextEditRulerRecord(resourceData);
                        Debug.WriteLine($"    Text Edit Ruler Record: Ruler LeftMargin={textEditRulerRecord.Ruler.LeftMargin}, LeftIndent={textEditRulerRecord.Ruler.LeftIndent}, RightMargin={textEditRulerRecord.Ruler.RightMargin}, Justification={textEditRulerRecord.Ruler.TabType}, TabStopCount={textEditRulerRecord.Ruler.TabStops?.Count ?? 0}, TabTerminator={textEditRulerRecord.Ruler.TabTerminator}");
                        foreach (var tabStop in textEditRulerRecord.Ruler.TabStops ?? Enumerable.Empty<ushort>())
                        {
                            Debug.WriteLine($"      Tab Stop: {tabStop}");
                        }

                        break;
                    }

                    case GsOsResourceForkType.Cursor:
                    {
                        var cursorRecord = new CursorRecord(resourceData);
                        Debug.WriteLine($"    Cursor Record: Width={cursorRecord.Width}, Height={cursorRecord.Height}, HotSpot=({cursorRecord.HotSpotX},{cursorRecord.HotSpotY}), Flags=0x{(ushort)cursorRecord.Flags:X4}, ImageData Length={cursorRecord.ImageData.Length} bytes, MaskData Length={cursorRecord.MaskData.Length} bytes");
                        break;
                    }
                    
                    case GsOsResourceForkType.ItemStruct:
                    {
                        var itemStructRecord = new ItemStructRecord(resourceData);
                        Debug.WriteLine($"    Item Struct Record: Flags=0x{itemStructRecord.Flags.RawValue:X4}, TitleReference={itemStructRecord.TitleReference}, IconReference={itemStructRecord.IconReference}");
                        break;
                    }

                    case GsOsResourceForkType.Version:
                    {
                        var versionRecord = new VersionRecord(resourceData);
                        Debug.WriteLine($"    Version Record: Version={versionRecord.Version}, Region={versionRecord.Region}, Name='{versionRecord.Name}', MoreInfo='{versionRecord.MoreInfo}'");
                        break;
                    }

                    case GsOsResourceForkType.Comment:
                    {
                        var commentRecord = new CommentRecord(resourceData);
                        Debug.WriteLine($"    Comment Record: '{commentRecord.Comment}'");
                        break;
                    }

                    case GsOsResourceForkType.Bundle:
                    {
                        var bundleRecord = new BundleRecord(resourceData);
                        Debug.WriteLine($"    Bundle Record: Version={bundleRecord.Version}, DocumentsOffset={bundleRecord.DocumentsOffset}, IconResourceID=0x{bundleRecord.IconResourceID:X8}, BundleResourceID=0x{bundleRecord.BundleResourceID:X8}, BundleResourceHandle=0x{bundleRecord.BundleResourceHandle:X8}, NumberOfDocuments={bundleRecord.NumberOfDocuments}");
                        break;
                    }

                    case GsOsResourceForkType.FinderPath:
                    {
                        var finderPathRecord = new FinderPathRecord(resourceData);
                        Debug.WriteLine($"    Finder Path Record: Version={finderPathRecord.Version}, PathNameOffset={finderPathRecord.PathNameOffset}, NumberOfEntries={finderPathRecord.NumberOfEntries}");
                        foreach (var finderPathEntry in finderPathRecord.Entries)
                        {
                            Debug.WriteLine($"      Finder Path Entry: VersionResourceID=0x{finderPathEntry.VersionResourceID:X8}, VersionResourceHandle=0x{finderPathEntry.VersionResourceHandle:X8}");
                        }

                        break;
                    }

                    case GsOsResourceForkType.PaletteWindow:
                    {
                        var paletteWindowRecord = new PaletteWindowRecord(resourceData);
                        Debug.WriteLine($"    Palette Window Record: Data Length={paletteWindowRecord.Data.Length} bytes");
                        break;
                    }

                    case GsOsResourceForkType.TaggedStrings:
                    {
                        var taggedStringsRecord = new TaggedStringsRecord(resourceData);
                        Debug.WriteLine($"    Tagged Strings Record: Count={taggedStringsRecord.Count}");
                        foreach (var pair in taggedStringsRecord.Pairs)
                        {
                            Debug.WriteLine($"      Tagged String: Key=0x{pair.Key:X4}, Value='{pair.Value}'");
                        }
                        break;
                    }

                    case GsOsResourceForkType.PatternList:
                    {
                        var patternList = new PatternListRecord(resourceData);
                        Debug.WriteLine($"    Pattern List Record: Count={patternList.Patterns.Count}");
                        foreach (var pattern in patternList.Patterns)
                        {
                            Debug.WriteLine($"      Pattern: Length={pattern.Length} bytes");
                        }
                        break;   
                    }

                    case GsOsResourceForkType.RectangleList:
                    {
                        var recangleList = new RectangleListRecord(resourceData);
                        Debug.WriteLine($"    Rectangle List Record: Count={recangleList.Count}");
                        for (int i = 0; i < recangleList.Rectangles.Count; i++)
                        {
                            var rect = recangleList.Rectangles[i];
                            Debug.WriteLine($"      Rectangle {i}: ({rect.Left}, {rect.Top}, {rect.Right}, {rect.Bottom})");
                        }

                        break;
                    }

                    case GsOsResourceForkType.PrintRecord:
                    {
                        var printRecord = new PrintRecord(resourceData);
                        Debug.WriteLine($"    Print Record: Data Length={printRecord.Data.Length} bytes");
                        break;
                    }

                    case GsOsResourceForkType.FontRecord:
                    {
                        var fontRecord = new FontRecord(resourceData);
                        Debug.WriteLine($"    Font Record: Data Length={fontRecord.Data.Length} bytes");
                        break;
                    }

                    case (GsOsResourceForkType)0x0001:
                    case (GsOsResourceForkType)0x0002:
                    case (GsOsResourceForkType)0x0042:
                    case (GsOsResourceForkType)0x4D61:
                    case (GsOsResourceForkType)0x5E72:
                    case (GsOsResourceForkType)0x7001:
                    {
                        // Unknown.
                        break;
                    }

                    default:
                    {
                        if (Enum.IsDefined(record.Type))
                        {
                            throw new NotImplementedException($"Resource Type {record.Type} not supported.");
                        }
                        else
                        {
                            throw new NotImplementedException($"Resource Type 0x{(ushort)record.Type:X4} not supported.");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.WriteLine($"Extracting file: {entry.FileName} ({entry.StorageType} and {entry.Eof} bytes) to {outputFilePath}");
            using var outputStream = File.Create(outputFilePath);
            disk.GetFileData(entry, outputStream);
        }
    }

    [Fact]
    public void Ctor_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("stream", () => new ProDiskVolume(null!));
    }
}
