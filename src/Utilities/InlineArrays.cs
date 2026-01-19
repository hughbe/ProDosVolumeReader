using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ProDosVolumeReader.Utilities;

/// <summary>
/// An inline array of 8 bytes.
/// </summary>
[InlineArray(Size)]
public struct ByteArray8
{
    /// <summary>
    /// The size of the array in bytes.
    /// </summary>
    public const int Size = 8;

    private byte _element0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArray8"/> struct.
    /// </summary>
    public ByteArray8(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be exactly {Size} bytes long.", nameof(data));
        }
        
        data.CopyTo(AsSpan());
    }

    /// <summary>
    /// Gets a span over the elements of the array.
    /// </summary>   
    public Span<byte> AsSpan() =>
        MemoryMarshal.CreateSpan(ref _element0, Size);
}

/// <summary>
/// An inline array of 15 bytes.
/// </summary>
[InlineArray(Size)]
public struct ByteArray15
{
    /// <summary>
    /// The size of the array in bytes.
    /// </summary>
    public const int Size = 15;

    private byte _element0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArray15"/> struct.
    /// </summary>
    public ByteArray15(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be exactly {Size} bytes long.", nameof(data));
        }

        data.CopyTo(AsSpan());
    }

    /// <summary>
    /// Gets a span over the elements of the array.
    /// </summary>   
    public Span<byte> AsSpan() =>
        MemoryMarshal.CreateSpan(ref _element0, Size);
}
