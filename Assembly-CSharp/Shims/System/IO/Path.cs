using System;

using _Path = System.IO.Path;

namespace Shims.NET.System.IO;

/// <summary>
/// Provides static <see cref="System.IO.Path"/> methods introduced in .NET 4.0
/// </summary>
public static class Path
{
    /// <summary>
    /// Combines an array of strings into a path.
    /// </summary>
    /// <param name="paths">An array of parts of the path.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException">One of the strings in the array is <c>null</c>.</exception>
    public static string Combine(params string[] paths)
    {
        if (paths.Length == 0)
        {
            return "";
        }

        if (paths[0] == null)
        {
            throw new ArgumentNullException("Paths cannot be null");
        }

        string result = paths[0];
        for (int i = 1; i < paths.Length; i++)
        {
            if (paths[i] == null)
            {
                throw new ArgumentNullException("Paths cannot be null");
            }

            result = _Path.Combine(result, paths[i]);
        }

        return result;
    }
}