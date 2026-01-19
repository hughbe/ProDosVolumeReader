using System.Diagnostics;
using System.Text;

namespace ProDosVolumeReader.Resources.Records;

/// <summary>
/// A comment record containing text.
/// </summary>
public readonly struct CommentRecord
{
    /// <summary>
    /// Gets the text content.
    /// </summary>
    public string Comment { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentRecord"/> struct.
    /// </summary>
    /// <param name="data">The raw data for the CommentRecord.</param>
    public CommentRecord(ReadOnlySpan<byte> data)
    {
        // Structure documented in https://web.archive.org/web/20050425130811/https://web.pdx.edu/~heiss/technotes/iigs/tn.iigs.076.html
        int offset = 0;

        // The comment. This is unformatted, 8-bit text suitable for displaying by a
        // desktop program. No length limit is imposed by this resource format,
        // although a practical limit of a few hundred characters is recommended.
        Comment = Encoding.ASCII.GetString(data.Slice(offset, data.Length));
        offset += data.Length;

        Debug.Assert(offset == data.Length, "Did not consume all data for CommentRecord.");
    }
}
