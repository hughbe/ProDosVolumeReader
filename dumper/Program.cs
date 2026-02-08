using Spectre.Console;
using Spectre.Console.Cli;
using ProDosVolumeReader;

public sealed class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp<ExtractCommand>();
        app.Configure(config =>
        {
            config.SetApplicationName("prodos-dumper");
            config.ValidateExamples();
        });

        return app.Run(args);
    }
}

sealed class ExtractSettings : CommandSettings
{
    [CommandArgument(0, "<input>")]
    public required string Input { get; init; }

    [CommandOption("-o|--output")]
    public string? Output { get; init; }
}

sealed class ExtractCommand : AsyncCommand<ExtractSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ExtractSettings settings, CancellationToken cancellationToken)
    {
        var input = new FileInfo(settings.Input);
        if (!input.Exists)
        {
            AnsiConsole.MarkupLine($"[red]Input file not found[/]: {input.FullName}");
            return -1;
        }

        var outputPath = settings.Output ?? Path.GetFileNameWithoutExtension(input.Name);
        var outputDir = new DirectoryInfo(outputPath);
        if (!outputDir.Exists)
        {
            outputDir.Create();
        }

        await using var stream = input.OpenRead();
        var disk = new ProDosDisk(stream);

        foreach (var volume in disk.Volumes)
        {
            AnsiConsole.MarkupLine($"[green]Volume[/]: {volume.VolumeDirectoryHeader.FileName}");
            AnsiConsole.MarkupLine($"[green]Total Blocks[/]: {volume.VolumeDirectoryHeader.TotalBlocks}");
            AnsiConsole.MarkupLine($"[green]Files[/]: {volume.VolumeDirectoryHeader.FileCount}");

            var volumeOutputDir = disk.Volumes.Count == 1
                ? outputDir.FullName
                : Path.Combine(outputDir.FullName, volume.VolumeDirectoryHeader.FileName.ToString());

            await ExtractDirectory(volume, volumeOutputDir, cancellationToken);

            AnsiConsole.MarkupLine($"[green]Extraction complete[/]: {volumeOutputDir}");
        }
        return 0;
    }

    private static async Task ExtractDirectory(ProDiskVolume disk, string outputDirectory, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputDirectory);

        foreach (var entry in disk.EnumerateRootContents())
        {
            await ExtractEntry(disk, entry, outputDirectory, cancellationToken);
        }
    }

    private static async Task ExtractEntry(ProDiskVolume disk, FileEntry entry, string outputDirectory, CancellationToken cancellationToken)
    {
        var safeName = SanitizeName(entry.FileName);

        if (entry.StorageType == StorageType.Subdirectory)
        {
            var subDirPath = Path.Combine(outputDirectory, safeName);
            Directory.CreateDirectory(subDirPath);

            foreach (var subEntry in disk.EnumerateSubdirectory(entry))
            {
                await ExtractEntry(disk, subEntry, subDirPath, cancellationToken);
            }
            return;
        }

        // Add extension based on file type
        string extension = entry.FileType switch
        {
            FileType.Text => ".txt",
            FileType.Binary => ".bin",
            FileType.Directory => "",
            FileType.ApplesoftProgram => ".bas",
            FileType.IntegerBasicProgram => ".int",
            FileType.PascalCode => ".pcd",
            FileType.PascalText => ".ptx",
            FileType.PascalData => ".pda",
            FileType.GraphicsScreen => ".pic",
            FileType.ProDos8System or FileType.ProDos16System => ".sys",
            _ => ".dat"
        };

        var filePath = Path.Combine(outputDirectory, safeName + extension);

        try
        {
            var data = disk.GetFileData(entry);
            await File.WriteAllBytesAsync(filePath, data, cancellationToken);
            AnsiConsole.MarkupLine($"Wrote: [blue]{Path.GetFileName(filePath)}[/] ({data.Length} bytes) [[{entry.FileType}]]");
        }
        catch (NotSupportedException ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Skipped[/]: {entry.FileName} - {ex.Message}");
        }
    }

    private static string SanitizeName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            name = name.Replace(invalidChar, '_');
        }

        return name;
    }
}
