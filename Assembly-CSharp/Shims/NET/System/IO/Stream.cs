using _Stream = System.IO.Stream;

namespace Shims.NET.System.IO;

/// <summary>
/// Provides <see cref="System.IO.Stream"/> methods introduced in .NET 4.0
/// </summary>
public static class Stream
{
    //We pick a value that is the largest multiple of 4096 that is still smaller than the large object heap threshold (85K).
    // The CopyTo/CopyToAsync buffer is short-lived and is likely to be collected at Gen0, and it offers a significant
    // improvement in Copy performance.
    private const int _DefaultCopyBufferSize = 81920;
    
    /// <summary>
    /// Reads the bytes from the current stream and writes them to another stream. Both streams positions are advanced by the number of bytes copied.
    /// </summary>
    /// <param name="source">The source stream</param>
    /// <param name="destination">The stream to which the contents of <c>source</c> will be copied.</param>
    public static void CopyTo(this _Stream source, _Stream destination)
    {
        byte[] buffer = new byte[_DefaultCopyBufferSize];
        int read;
        while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
        {
            destination.Write(buffer, 0, read);
        }
    }
}