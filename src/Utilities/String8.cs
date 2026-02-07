using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ProDosVolumeReader.Utilities;

/// <summary>
/// Represents a fixed-size string of up to 8 bytes (e.g., file type, creator codes).
/// </summary>
[InlineArray(Size)]
public struct String8 : ISpanFormattable, IEquatable<String8>
{
    /// <summary>
    /// Gets the size of the string in bytes.
    /// </summary>
    public const int Size = 8;

    /// <summary>
    /// The first element of the array.
    /// </summary>
    private byte _element0;

    /// <summary>
    /// Initializes a new instance of the <see cref="String8"/> struct.
    /// </summary>
    /// <param name="data">The span containing the string bytes.</param>
    /// <exception cref="ArgumentException">Thrown when the data span length is not equal to <see cref="Size"/>.</exception>
    public String8(ReadOnlySpan<byte> data)
    {
        if (data.Length != Size)
        {
            throw new ArgumentException($"Data span must be exactly {Size} bytes long.", nameof(data));
        }

        data.CopyTo(AsSpan());
    }

    /// <summary>
    /// Gets the length of the string (excluding null terminator).
    /// </summary>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> span = AsReadOnlySpan();
            int length = span.IndexOf((byte)0);
            return length < 0 ? span.Length : length;
        }
    }

    /// <summary>
    /// Gets a span over the elements of the array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> AsSpan() =>
        MemoryMarshal.CreateSpan(ref _element0, Size);

    /// <summary>
    /// Gets a read-only span over the elements of the array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<byte> AsReadOnlySpan() =>
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _element0), Size);

    /// <summary>
    /// Attempts to format the string into the provided span without allocating.
    /// </summary>
    /// <param name="destination">The span to write the string to.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters written.</param>
    /// <returns>true if the formatting was successful; otherwise, false.</returns>
    public readonly bool TryFormat(Span<char> destination, out int charsWritten)
    {
        ReadOnlySpan<byte> span = AsReadOnlySpan();
        int length = Length;

        if (destination.Length < length)
        {
            charsWritten = 0;
            return false;
        }

        for (int i = 0; i < length; i++)
        {
            destination[i] = (char)span[i];
        }

        charsWritten = length;
        return true;
    }

    /// <inheritdoc/>
    readonly bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
        TryFormat(destination, out charsWritten);

    /// <inheritdoc/>
    readonly string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString();

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        ReadOnlySpan<byte> span = AsReadOnlySpan();
        int length = Length;

        return Encoding.ASCII.GetString(span[..length]);
    }

    /// <summary>
    /// Determines whether this string equals the specified character span without allocating.
    /// </summary>
    /// <param name="other">The character span to compare with.</param>
    /// <returns>true if the strings are equal; otherwise, false.</returns>
    public readonly bool Equals(ReadOnlySpan<char> other)
    {
        int length = Length;
        if (other.Length != length)
        {
            return false;
        }

        ReadOnlySpan<byte> span = AsReadOnlySpan();
        for (int i = 0; i < length; i++)
        {
            if ((char)span[i] != other[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether this string equals the specified byte span (ASCII) without allocating.
    /// </summary>
    /// <param name="other">The byte span to compare with.</param>
    /// <returns>true if the strings are equal; otherwise, false.</returns>
    public readonly bool Equals(ReadOnlySpan<byte> other)
    {
        int length = Length;
        if (other.Length != length)
        {
            return false;
        }

        return AsReadOnlySpan()[..length].SequenceEqual(other);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(String8 other) =>
        AsReadOnlySpan().SequenceEqual(other.AsReadOnlySpan());

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) =>
        obj is String8 other && Equals(other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        ReadOnlySpan<byte> span = AsReadOnlySpan();
        return HashCode.Combine(span[0], span[1], span[2], span[3]);
    }

    /// <summary>
    /// Determines whether two <see cref="String8"/> instances are equal.
    /// </summary>
    public static bool operator ==(String8 left, String8 right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="String8"/> instances are not equal.
    /// </summary>
    public static bool operator !=(String8 left, String8 right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts the <see cref="String8"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="str">The <see cref="String8"/> instance.</param>
    /// <returns>The converted string.</returns>
    public static implicit operator string(String8 str) => str.ToString();
}
