# ProDosVolumeReader

A lightweight .NET library for reading Apple ProDOS disk images and extracting their contents. Supports both raw ProDOS volumes and partitioned disk images (via Apple Partition Map), including Apple IIgs GS/OS features like forked files and resource forks.

## Features

- Read ProDOS disk images (.po, .dsk, .img, .iso)
- Automatic Apple Partition Map detection for partitioned disks (`Apple_PRODOS`)
- Enumerate files and subdirectories
- Extract file data (seedling, sapling, and tree storage types)
- GS/OS forked file support (data and resource forks)
- GS/OS resource fork parsing (icons, menus, controls, windows, fonts, sounds, and more)
- Finder metadata (MacFinderInfo, MacExtendedFinderInfo)
- Support for .NET 9.0

## Installation

Add the project reference to your .NET application:

```sh
dotnet add reference path/to/ProDosVolumeReader.csproj
```

Or, if published on NuGet:

```sh
dotnet add package ProDosVolumeReader
```

## Usage

### Reading a ProDOS Disk Image

```csharp
using ProDosVolumeReader;

// Open a disk image file
using var stream = File.OpenRead("System.Disk.po");

// Create a disk reader (handles partition detection automatically)
var disk = new ProDosDisk(stream);

foreach (var volume in disk.Volumes)
{
    Console.WriteLine($"Volume: {volume.VolumeDirectoryHeader.FileName}");
    Console.WriteLine($"Total Blocks: {volume.VolumeDirectoryHeader.TotalBlocks}");
    Console.WriteLine($"Files: {volume.VolumeDirectoryHeader.FileCount}");
}
```

### Enumerating Files

```csharp
var volume = disk.Volumes[0];

foreach (var entry in volume.EnumerateRootContents())
{
    Console.WriteLine($"File: {entry.FileName} ({entry.FileType}, {entry.Eof} bytes)");

    if (entry.StorageType == StorageType.Subdirectory)
    {
        foreach (var subEntry in volume.EnumerateSubdirectory(entry))
        {
            Console.WriteLine($"  {subEntry.FileName} ({subEntry.FileType}, {subEntry.Eof} bytes)");
        }
    }
}
```

### Extracting File Data

```csharp
foreach (var entry in volume.EnumerateRootContents())
{
    if (entry.StorageType == StorageType.Subdirectory)
        continue;

    // Get file data as byte array
    byte[] data = volume.GetFileData(entry);
    File.WriteAllBytes($"{entry.FileName}.bin", data);

    // Or stream to an output stream
    using var outputStream = File.Create($"{entry.FileName}.dat");
    volume.GetFileData(entry, outputStream);
}
```

### Working with GS/OS Forked Files

```csharp
foreach (var entry in volume.EnumerateRootContents())
{
    if (entry.StorageType != StorageType.GSOSForkedFile)
        continue;

    // Extract data and resource forks separately
    byte[] dataFork = volume.GetDataFork(entry);
    byte[] resourceFork = volume.GetResourceFork(entry);

    File.WriteAllBytes($"{entry.FileName}.data", dataFork);
    File.WriteAllBytes($"{entry.FileName}.res", resourceFork);
}
```

## API Overview

### ProDosDisk

Entry point for reading disk images. Automatically detects Apple Partition Maps and discovers ProDOS volumes.

- `ProDosDisk(Stream stream)` - Opens a disk image, scanning for ProDOS partitions
- `Volumes` - List of `ProDiskVolume` instances found on the disk

### ProDiskVolume

Represents a single ProDOS volume and provides access to its contents.

- `VolumeDirectoryHeader` - Volume metadata (name, creation date, total blocks, file count)
- `EnumerateRootContents()` - Enumerates files and directories in the volume root
- `EnumerateSubdirectory(FileEntry)` - Enumerates contents of a subdirectory
- `GetVolumeBlockAllocationBitmap()` - Returns the volume block allocation bitmap
- `GetFileData(FileEntry)` / `GetFileData(FileEntry, Stream)` - Reads file data
- `GetDataFork(FileEntry)` / `GetDataFork(FileEntry, Stream)` - Reads the data fork of a GS/OS forked file
- `GetResourceFork(FileEntry)` / `GetResourceFork(FileEntry, Stream)` - Reads the resource fork of a GS/OS forked file

### Key Types

- `VolumeDirectoryHeader` - Volume-level metadata (name, dates, total blocks, file count, bitmap pointer)
- `FileEntry` - File or directory entry (name, type, storage type, key pointer, EOF, dates, access flags)
- `StorageType` - Seedling, Sapling, Tree, GSOSForkedFile, Subdirectory, etc.
- `FileType` - Text, Binary, ApplesoftProgram, IntegerBasicProgram, GraphicsScreen, System, etc.
- `ExtendedKeyBlock` - GS/OS forked file metadata containing data and resource fork entries
- `MacFinderInfo` / `MacExtendedFinderInfo` - Finder metadata for GS/OS files

## Building

Build the project using the .NET SDK:

```sh
dotnet build
```

Run tests:

```sh
dotnet test
```

## ProDosDumper CLI

Extract a ProDOS disk image to a directory using the dumper tool.

### Install/Build

```sh
dotnet build dumper/ProDosVolumeDumper.csproj -c Release
```

### Usage

```sh
prodos-dumper <input> [-o|--output <directory>]
```

- `<input>` - Path to the disk image file
- `-o|--output` - Destination directory for extracted files (defaults to the input filename without extension)

Files are extracted with extensions based on their ProDOS file type (.txt, .bin, .bas, .sys, etc.).

## Requirements

- .NET 9.0 or later

## License

MIT License. See [LICENSE](LICENSE) for details.

Copyright (c) 2026 Hugh Bellamy

## Related Projects

- [AppleDiskImageReader](https://github.com/hughbe/AppleDiskImageReader) - Reader for Apple II universal disk (.2mg) images
- [AppleIIDiskReader](https://github.com/hughbe/AppleIIDiskReader) - Reader for Apple II DOS 3.3 disk (.dsk) images
- [ApplePascalVolumeReader](https://github.com/hughbe/ApplePascalVolumeReader) - Reader for Apple Pascal (.dsk, .po) volumes
- [ProDosVolumeReader](https://github.com/hughbe/ProDosVolumeReader) - Reader for ProDOS (.po) volumes
- [WozDiskImageReader](https://github.com/hughbe/WozDiskImageReader) - Reader for WOZ (.woz) disk images
- [DiskCopyReader](https://github.com/hughbe/DiskCopyReader) - Reader for Disk Copy 4.2 (.dc42) images
- [MfsReader](https://github.com/hughbe/MfsReader) - Reader for MFS (Macintosh File System) volumes
- [HfsReader](https://github.com/hughbe/HfsReader) - Reader for HFS (Hierarchical File System) volumes
- [HfsPlusReader](https://github.com/hughbe/HfsPlusReader) - Reader for HFS+ (Hierarchical File System+) volumes
- [ApplePartitionMapReader](https://github.com/hughbe/ApplePartitionMapReader) - Reader for Apple Partition Map (APM) images
- [ResourceForkReader](https://github.com/hughbe/ResourceForkReader) - Reader for Macintosh resource forks
- [BinaryIIReader](https://github.com/hughbe/BinaryIIReader) - Reader for Binary II (.bny, .bxy) archives
- [StuffItReader](https://github.com/hughbe/StuffItReader) - Reader for StuffIt (.sit) archives
- [ShrinkItReader](https://github.com/hughbe/ShrinkItReader) - Reader for ShrinkIt (.shk, .sdk) archives

## Documentation
### ProDOS File System
- ProDOS 8 Technical Note #25 https://prodos8.com/docs/technote/25/
### Apple IIgs Resource Forks
- https://ciderpress2.com/formatdoc/ResourceFork-notes.html
- Apple IIGS Toolbox Reference, Volume 3: https://dn790003.ca.archive.org/0/items/Apple_IIGS_Toolbox_Reference_vol_3/Apple_IIGS_Toolbox_Reference_vol_3.pdf
- Apple IIGS Technical Note #76: https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.076.html
- Apple IIGS Technical Note #100: https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.100.html
- Apple IIGS Toolbox Changes
for System Software 6.0: https://apple2.gs/downloads/library/Apple%20IIgs%20Toolbox%20Changes%20for%20System%20Software%206.0.pdf
- Apple IIGS Technical Note #24: https://mirrors.apple2.org.za/Apple%20II%20Documentation%20Project/Computers/Apple%20II/Apple%20IIGS/Documentation/Apple%20IIGS%20Technical%20Notes%2011-25.pdf
- Apple IIGS Technical Note #66: https://web.archive.org/web/20051108132137/https://web.pdx.edu/~heiss/technotes/ftyp/ftn.42.xxxx.html
- Prez: https://github.com/ksherlock/prez
