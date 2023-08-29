using System.Collections.Generic;
using System.IO;
using _DirectoryInfo = System.IO.DirectoryInfo;

namespace Shims.NET.System.IO;

/// <summary>
/// Provides extensions for <see cref="_DirectoryInfo"/> methods introduced in .NET 4.0
/// </summary>
public static class DirectoryInfo
{
    /// <summary>
    /// Returns an enumerable collection of file information in the current directory.
    /// </summary>
    /// <param name="self">The current directory</param>
    /// <returns>An enumerable collection of file information in the current directory.</returns>
    public static IEnumerable<FileInfo> EnumerateFiles(this _DirectoryInfo self)
    {
        return self.GetFiles();
    }
}
