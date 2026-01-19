using System.Buffers.Binary;
using System.Diagnostics;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// Represents a rectangle structure in a resource fork.
/// </summary>
public struct RECT
{
    /// <summary>
    /// The size of a RECT structure in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// Gets the top coordinate.
    /// </summary>
    public short Top { get; }

    /// <summary>
    /// Gets the bottom coordinate.
    /// </summary>
    public short Left { get; }

    /// <summary>
    /// Gets the bottom coordinate.
    /// </summary>
    public short Bottom { get; }

    /// <summary>
    /// Gets the right coordinate.
    /// </summary>
    public short Right { get; }

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public readonly int Width => Math.Abs(Right - Left);

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public readonly int Height => Math.Abs(Bottom - Top);

    /// <summary>
    /// Initializes a new instance of the <see cref="RECT"/> struct by parsing binary data.
    /// </summary>
    /// <param name="data">A span containing exactly 8 bytes of RECT data.</param>
    /// <exception cref="ArgumentException">Thrown when data is not exactly 8 bytes long.</exception>
    public RECT(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data must be exactly {Size} bytes long.", nameof(data));
        }

        int offset = 0;

        Top = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Left = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Bottom = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Right = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        offset += 2;

        Debug.Assert(offset == data.Length, "Did not parse the entire RECT data.");
    }
}
