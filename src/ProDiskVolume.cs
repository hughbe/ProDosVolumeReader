using System.Buffers.Binary;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a Apple ProDOS disk image.
/// </summary>
public class ProDiskVolume
{
    /// <summary>
    /// The starting block number of the volume directory.
    /// </summary>
    private const int VolumeDirectoryStartBlock = 2;

    /// <summary>
    /// The size of a block in bytes.
    /// </summary>
    private const int BlockSize = 512;

    /// <summary>
    /// The minimum size of a ProDOS volume in bytes.
    /// 2 boot blocks, 1 master directory block, 1 volume bitmap block, and
    /// at least 1 data block.
    /// </summary>
    private const int MinimumVolumeSize = BlockSize * 5;

    /// <summary>
    /// The maximum size of a ProDOS volume in bytes.
    /// </summary>
    private const int MaximumVolumeSize = BlockSize * 65535;

    private readonly Stream _stream;
    private readonly long _streamOffset;

    /// <summary>
    /// Gets the volume directory header.
    /// </summary>
    public VolumeDirectoryHeader VolumeDirectoryHeader { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProDiskVolume"/> class.
    /// </summary>
    /// <param name="stream">The stream containing the ProDos disk image.</param>
    /// <exception cref="ArgumentException">>Thrown if the stream is not seekable or readable.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is <see langword="null"/>.</exception>
    public ProDiskVolume(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanSeek || !stream.CanRead)
        {
            throw new ArgumentException("Stream must be seekable and readable.", nameof(stream));
        }

        _stream = stream;
        _streamOffset = stream.Position;

        var streamLength = stream.Length - _streamOffset;
        if (streamLength < MinimumVolumeSize || streamLength > MaximumVolumeSize)
        {
            throw new ArgumentException("Stream size is not valid for a ProDos volume.", nameof(stream));
        }

        // Skip the loader in blocks 0 and 1.
        // Skip the first four bytes of block 2 (the block pointers).
        // Volume Directory Headers Block 2 of a volume is the key block of that
        // volume's directory file. The volume directory header is at byte position
        // $0004 of the key block, immediately following the block's two pointers.
        // Thirteen fields are currently defined to be in a volume directory header:
        // they contain all the vital information about that volume. Figure B-3
        // illustrates the structure of a volume directory header.
        Span<byte> buffer = stackalloc byte[4 + VolumeDirectoryHeader.Size];
        ReadBlock(VolumeDirectoryStartBlock, buffer);

        VolumeDirectoryHeader = new VolumeDirectoryHeader(buffer[4..]);
        if (VolumeDirectoryHeader.EntryLength != FileEntry.Size)
        {
            throw new ArgumentException($"Unsupported volume directory entry length: {VolumeDirectoryHeader.EntryLength}.", nameof(stream));
        }
    }

    /// <summary>
    /// Reads the volume block allocation bitmap from the disk.
    /// </summary>
    /// <returns>The volume block allocation bitmap.</returns>
    /// <exception cref="IOException">Thrown if there is an error reading from the stream.</exception>
    public VolumeBlockAllocationBitmap GetVolumeBlockAllocationBitmap()
    {
        ushort totalBlocks = VolumeDirectoryHeader.TotalBlocks;
        int bitmapBlockCount = VolumeBlockAllocationBitmap.CalculateBitmapBlockCount(totalBlocks);

        // Read all bitmap blocks into a single buffer
        byte[] bitmapData = new byte[bitmapBlockCount * 512];
        ushort bitmapStartBlock = VolumeDirectoryHeader.BitMapPointer;

        for (int i = 0; i < bitmapBlockCount; i++)
        {
            ReadBlock((ushort)(bitmapStartBlock + i), bitmapData.AsSpan(i * 512, 512));
        }

        return new VolumeBlockAllocationBitmap(bitmapData, totalBlocks);
    }

    /// <summary>
    /// Enumerates the file entries in the volume directory.
    /// </summary>
    /// <returns>The file entries.</returns>
    /// <exception cref="IOException">Thrown if there is an error reading from the stream.</exception>
    public IEnumerable<FileEntry> EnumerateRootContents()
    {
        // Start at the key block of the volume directory
        return EnumerateEntries(2, VolumeDirectoryHeader.FileCount);
    }

    /// <summary>
    /// Enumerates the file entries in a subdirectory.
    /// </summary>
    /// <param name="fileEntry">The file entry representing the subdirectory.</param>
    /// <returns>The file entries.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="fileEntry"/> is not a subdirectory.</exception>
    /// <exception cref="IOException">Thrown if there is an error reading from the stream.</exception>
    public IEnumerable<FileEntry> EnumerateSubdirectory(FileEntry fileEntry)
    {
        if (fileEntry.StorageType != StorageType.Subdirectory)
        {
            throw new ArgumentException("File entry is not a subdirectory.", nameof(fileEntry));
        }

        // Read the subdirectory's Volume Directory Header.
        Span<byte> buffer = stackalloc byte[SubdirectoryHeader.Size];
        long blockOffsetWithinStream = _streamOffset + (fileEntry.KeyPointer * 512);
        if (blockOffsetWithinStream > _stream.Length - 512)
        {
            throw new IOException("Subdirectory block is out of bounds.");
        }

        // Skip the first four bytes (the block pointers).
        _stream.Seek(blockOffsetWithinStream + 4, SeekOrigin.Begin);
        if (_stream.Read(buffer) != buffer.Length)
        {
            throw new IOException("Failed to read subdirectory block from stream.");
        }

        var header = new SubdirectoryHeader(buffer);
        return EnumerateEntries(fileEntry.KeyPointer, header.FileCount);
    }

    /// <summary>
    /// Enumerates the file entries in a directory starting from the specified block.
    /// </summary>
    /// <param name="startBlock">The starting block number of the directory.</param>
    /// <param name="count">The number of file entries to read.</param>
    /// <returns>The file entries.</returns>
    /// <exception cref="IOException">Thrown if there is an error reading from the stream.</exception>
    private IEnumerable<FileEntry> EnumerateEntries(ushort startBlock, int count)
    {
        var i = 0;
        var currentBlockNumber = startBlock;
        byte[] blockBuffer = new byte[512];

        while (i < count)
        {
            ReadBlock(currentBlockNumber, blockBuffer);

            int offsetWithinBlock = 0;

            // Read the next block number from the block header
            var _ = BinaryPrimitives.ReadUInt16LittleEndian(blockBuffer.AsSpan(offsetWithinBlock, 2));
            offsetWithinBlock += 2;

            var nextBlockNumber = BinaryPrimitives.ReadUInt16LittleEndian(blockBuffer.AsSpan(offsetWithinBlock, 2));
            offsetWithinBlock += 2;

            if (currentBlockNumber == startBlock)
            {
                // Skip the volume directory header in the key block.
                offsetWithinBlock += VolumeDirectoryHeader.Size;
            }

            // Read file entries from the rest of the block.
            while (offsetWithinBlock < blockBuffer.Length && i < count)
            {
                byte entryStorageTypeAndNameLength = blockBuffer[offsetWithinBlock];
                if (entryStorageTypeAndNameLength == 0)
                {
                    // Entry is unused, skip it.
                    offsetWithinBlock += VolumeDirectoryHeader.EntryLength;
                    continue;
                }

                var entryData = blockBuffer.AsSpan(offsetWithinBlock, VolumeDirectoryHeader.EntryLength);
                var fileEntry = new FileEntry(entryData);
                yield return fileEntry;
                offsetWithinBlock += VolumeDirectoryHeader.EntryLength;
                i++;
            }

            // This will continue to the next block in the directory.
            currentBlockNumber = nextBlockNumber;

            if (currentBlockNumber == 0 && i < count)
            {
                throw new IOException("Directory ended before all file entries were read.");
            }
        }
    }

    /// <summary>
    /// Gets the file data for the specified file entry.
    /// </summary>
    /// <param name="fileEntry">The file entry.</param>
    /// <returns>The file data.</returns>
    public byte[] GetFileData(FileEntry fileEntry)
    {
        using var memoryStream = new MemoryStream();
        GetFileData(fileEntry, memoryStream);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Writes data from a data block to the output stream.
    /// </summary>
    /// <param name="dataBlockNumber">The block number.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <returns>The number of bytes written.</returns>
    private int WriteDataBlock(ushort dataBlockNumber, Stream outputStream, int count)
    {
        Span<byte> dataBuffer = stackalloc byte[512];
        ReadBlock(dataBlockNumber, dataBuffer);
        
        int bytesToWrite = Math.Min(count, dataBuffer.Length);
        outputStream.Write(dataBuffer[..bytesToWrite]);
        return bytesToWrite;
    }

    /// <summary>
    /// Writes data from an index block to the output stream.
    /// </summary>
    /// <param name="indexBlockNumber">The index block number.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <returns>The number of bytes written.</returns>
    private int WriteIndexBlock(ushort indexBlockNumber, Stream outputStream, int count)
    {
        Span<byte> indexBuffer = stackalloc byte[512];
        ReadBlock(indexBlockNumber, indexBuffer);
        
        // Iterate through up to 256 pointers and copy data blocks until the
        // requested count is reached.
        // 
        // ProDOS index blocks store block pointers in a split format:
        // - Low bytes of all 256 pointers are in bytes 0-255
        // - High bytes of all 256 pointers are in bytes 256-511
        var remaining = count;
        var totalWritten = 0;

        for (int i = 0; i < 256 && remaining > 0; i++)
        {
            var lowByte = indexBuffer[i];
            var highByte = indexBuffer[256 + i];
            var dataBlockNumber = (ushort)(lowByte | (highByte << 8));

            var bytesToWrite = Math.Min(remaining, 512);
            
            // A block pointer of 0 indicates a sparse (unallocated) block.
            // In this case, we write zeros instead of reading from disk.
            if (dataBlockNumber == 0)
            {
                // Skip reading and write zeros instead.
                outputStream.Write(new byte[bytesToWrite]);
                totalWritten += bytesToWrite;
                remaining -= bytesToWrite;
            }
            else
            {
                var written = WriteDataBlock(dataBlockNumber, outputStream, bytesToWrite);
                totalWritten += written;
                remaining -= written;
            }
        }

        return totalWritten;
    }

    /// <summary>
    /// Writes data from a master index block to the output stream.
    /// </summary>
    /// <param name="masterIndexBlockNumber">The master index block number.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <returns>The number of bytes written.</returns>
    private int WriteMasterIndexBlock(ushort masterIndexBlockNumber, Stream outputStream, int count)
    {
        Span<byte> masterIndexBuffer = stackalloc byte[512];
        ReadBlock(masterIndexBlockNumber, masterIndexBuffer);

        // Iterate through up to 256 pointers to index blocks and copy data blocks until the
        // requested count is reached.
        //
        // ProDOS master index blocks store block pointers in a split format:
        // - Low bytes of all 256 pointers are in bytes 0-255
        // - High bytes of all 256 pointers are in bytes 256-511
        var remaining = count;
        var totalWritten = 0;

        for (int i = 0; i < 256 && remaining > 0; i++)
        {
            byte lowByte = masterIndexBuffer[i];
            byte highByte = masterIndexBuffer[256 + i];
            ushort indexBlockNumber = (ushort)(lowByte | (highByte << 8));

            int bytesToWrite = Math.Min(remaining, 256 * 512);
            int written = WriteIndexBlock(indexBlockNumber, outputStream, bytesToWrite);
            totalWritten += written;
            remaining -= written;
        }

        return totalWritten;
    }

    /// <summary>
    /// Gets the file data for the specified file entry.
    /// </summary>
    /// <param name="fileEntry">The file entry.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <returns>The number of bytes written to the output stream.</returns>
    /// <exception cref="NotSupportedException">Thrown if the file's storage type is not supported.</exception>
    public int GetFileData(FileEntry fileEntry, Stream outputStream)
    {
        switch (fileEntry.StorageType)
        {
            case StorageType.Seedling:
            {
                // A seedling file is a standard file that contains no more than 512 data bytes ($0 <= EOF <= $200).
                // This file is stored as one block on the volume, and this data block is the file's key block.
                int bytesToWrite = (int)Math.Min(fileEntry.Eof, 512);
                return WriteDataBlock(fileEntry.KeyPointer, outputStream, bytesToWrite);
            }
            case StorageType.Sapling:
            {
                // A sapling file contains an index block with up to 256 pointers
                // to data blocks. Each pointer is a 2-byte little-endian block number.
                int bytesToWrite = (int)Math.Min(fileEntry.Eof, 256 * 512);
                return WriteIndexBlock(fileEntry.KeyPointer, outputStream, bytesToWrite);
            }
            case StorageType.Tree:
            {
                // Ignore these for now.
                int bytesToWrite = (int)Math.Min(fileEntry.Eof, 65536 * 512);
                return WriteMasterIndexBlock(fileEntry.KeyPointer, outputStream, bytesToWrite);
            }
            case StorageType.GSOSForkedFile:
            {
                // GS/OS forked files have an extended key block that contains
                // mini-entries for data and resource forks.
                var extendedKeyBlock = GetExtendedKeyBlock(fileEntry.KeyPointer);
                return WriteForkData(extendedKeyBlock.DataFork, outputStream);
            }
            default:
                throw new NotSupportedException($"Storage type {fileEntry.StorageType} is not supported.");
        }
    }

    /// <summary>
    /// Reads the extended key block for a GS/OS forked file.
    /// </summary>
    /// <param name="keyBlockNumber">The key block number from the file entry.</param>
    /// <returns>The extended key block containing fork information.</returns>
    /// <exception cref="IOException">Thrown if there is an error reading from the stream.</exception>
    public ExtendedKeyBlock GetExtendedKeyBlock(ushort keyBlockNumber)
    {
        Span<byte> blockBuffer = stackalloc byte[512];
        ReadBlock(keyBlockNumber, blockBuffer);
        return new ExtendedKeyBlock(blockBuffer);
    }

    /// <summary>
    /// Gets the data fork for a GS/OS forked file.
    /// </summary>
    /// <param name="fileEntry">The file entry representing the forked file.</param>
    /// <returns>The data fork contents.</returns>
    /// <exception cref="ArgumentException">Thrown if the file entry is not a GS/OS forked file.</exception>
    public byte[] GetDataFork(FileEntry fileEntry)
    {
        if (fileEntry.StorageType != StorageType.GSOSForkedFile)
        {
            throw new ArgumentException("File entry is not a GS/OS forked file.", nameof(fileEntry));
        }

        var extendedKeyBlock = GetExtendedKeyBlock(fileEntry.KeyPointer);
        using var memoryStream = new MemoryStream();
        WriteForkData(extendedKeyBlock.DataFork, memoryStream);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Gets the data fork for a GS/OS forked file and writes it to the output stream.
    /// </summary>
    /// <param name="fileEntry">The file entry representing the forked file.</param>
    /// <param name="outputStream">The output stream to write to.</param>
    /// <returns>The number of bytes written to the output stream.</returns>
    /// <exception cref="ArgumentException">Thrown if the file entry is not a GS/OS forked file.</exception>
    public int GetDataFork(FileEntry fileEntry, Stream outputStream)
    {
        if (fileEntry.StorageType != StorageType.GSOSForkedFile)
        {
            throw new ArgumentException("File entry is not a GS/OS forked file.", nameof(fileEntry));
        }

        var extendedKeyBlock = GetExtendedKeyBlock(fileEntry.KeyPointer);
        return WriteForkData(extendedKeyBlock.DataFork, outputStream);
    }

    /// <summary>
    /// Gets the resource fork for a GS/OS forked file.
    /// </summary>
    /// <param name="fileEntry">The file entry representing the forked file.</param>
    /// <returns>The resource fork contents.</returns>
    /// <exception cref="ArgumentException">Thrown if the file entry is not a GS/OS forked file.</exception>
    public byte[] GetResourceFork(FileEntry fileEntry)
    {
        if (fileEntry.StorageType != StorageType.GSOSForkedFile)
        {
            throw new ArgumentException("File entry is not a GS/OS forked file.", nameof(fileEntry));
        }

        var extendedKeyBlock = GetExtendedKeyBlock(fileEntry.KeyPointer);
        using var memoryStream = new MemoryStream();
        WriteForkData(extendedKeyBlock.ResourceFork, memoryStream);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Gets the resource fork for a GS/OS forked file and writes it to the output stream.
    /// </summary>
    /// <param name="fileEntry">The file entry representing the forked file.</param>
    /// <param name="outputStream">The output stream to write to.</param>
    /// <returns>The number of bytes written to the output stream.</returns>
    /// <exception cref="ArgumentException">Thrown if the file entry is not a GS/OS forked file.</exception>
    public int GetResourceFork(FileEntry fileEntry, Stream outputStream)
    {
        if (fileEntry.StorageType != StorageType.GSOSForkedFile)
        {
            throw new ArgumentException("File entry is not a GS/OS forked file.", nameof(fileEntry));
        }

        var extendedKeyBlock = GetExtendedKeyBlock(fileEntry.KeyPointer);
        return WriteForkData(extendedKeyBlock.ResourceFork, outputStream);
    }

    /// <summary>
    /// Writes data from a fork entry to the output stream.
    /// </summary>
    /// <param name="forkEntry">The fork entry from the extended key block.</param>
    /// <param name="outputStream">The output stream.</param>
    /// <returns>The number of bytes written.</returns>
    private int WriteForkData(ExtendedKeyBlockEntry forkEntry, Stream outputStream)
    {
        if (forkEntry.IsEmpty || !forkEntry.IsValid)
        {
            return 0;
        }

        return forkEntry.StorageType switch
        {
            StorageType.Seedling => WriteDataBlock(forkEntry.KeyBlock, outputStream, (int)Math.Min(forkEntry.Eof, 512)),
            StorageType.Sapling => WriteIndexBlock(forkEntry.KeyBlock, outputStream, (int)Math.Min(forkEntry.Eof, 256 * 512)),
            StorageType.Tree => WriteMasterIndexBlock(forkEntry.KeyBlock, outputStream, (int)Math.Min(forkEntry.Eof, 65536 * 512)),
            _ => 0
        };
    }

    /// <summary>
    /// Reads a block from the stream into the provided buffer.
    /// </summary>
    /// <param name="blockNumber">The block number to read.</param>
    /// <param name="buffer">The buffer to read the block into.</param>
    /// <exception cref="IOException">Thrown if there is an error reading from the stream.</exception>
    private void ReadBlock(ushort blockNumber, Span<byte> buffer)
    {
        if (VolumeDirectoryHeader.TotalBlocks != 0 && blockNumber > VolumeDirectoryHeader.TotalBlocks)
        {
            throw new IOException($"Block number {blockNumber} is out of bounds for this volume.");
        }

        var blockOffsetWithinStream = _streamOffset + (blockNumber * (long)BlockSize);

        // If the block lies beyond the end of the provided stream but the
        // volume header indicates the disk is larger, treat the missing block
        // as a sparse (zero-filled) block.
        if (blockOffsetWithinStream < 0 || blockOffsetWithinStream + BlockSize > _stream.Length)
        {
            if (VolumeDirectoryHeader.TotalBlocks != 0 && blockNumber < VolumeDirectoryHeader.TotalBlocks)
            {
                buffer.Clear();
                return;
            }

            throw new IOException($"Block {blockNumber} is out of bounds (offset {blockOffsetWithinStream}, stream length {_stream.Length}).");
        }

        _stream.Seek(blockOffsetWithinStream, SeekOrigin.Begin);
        if (_stream.Read(buffer) != buffer.Length)
        {
            throw new IOException($"Failed to read block {blockNumber} from stream.");
        }
    }
}
