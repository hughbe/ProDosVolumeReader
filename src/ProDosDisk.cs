using ApplePartitionMapReader;

namespace ProDosVolumeReader;

/// <summary>
/// Represents a disk containing one or more ProDOS volumes.
/// </summary>
public class ProDosDisk
{
    /// <summary>
    /// Gets the list of ProDOS volumes found on the disk.
    /// </summary>
    public List<ProDiskVolume> Volumes { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ProDosDisk"/> class and scans for ProDOS volumes.
    /// </summary>
    /// <param name="stream">The stream containing the disk image data.</param>
    public ProDosDisk(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanSeek || !stream.CanRead)
        {
            throw new ArgumentException("Stream must be seekable and readable.", nameof(stream));
        }

        // Try to read Apple Partition Map entries first.
        if (ApplePartitionMap.IsApplePartitionMap(stream, 0))
        {
            var partitionMap = new ApplePartitionMap(stream, 0);
            foreach (var partitionEntry in partitionMap)
            {
                if (partitionEntry.Type.Equals("Apple_PRODOS"u8))
                {
                    // Found the ProDOS partition - add a volume for it.
                    var proDosStartOffset = (long)partitionEntry.PartitionStartBlock * 512;
                    stream.Seek(proDosStartOffset, SeekOrigin.Begin);
                    Volumes.Add(new ProDiskVolume(stream));
                }
            }
        }

        // If no ProDOS volumes found, assume the entire image is a single ProDOS volume.
        if (Volumes.Count == 0)
        {
            stream.Seek(0, SeekOrigin.Begin);
            Volumes.Add(new ProDiskVolume(stream));
        }
    }
}
