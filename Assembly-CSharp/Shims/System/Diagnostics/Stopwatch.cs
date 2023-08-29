using _Stopwatch = System.Diagnostics.Stopwatch;

namespace Shims.NET.System.Diagnostics;

/// <summary>
/// Provides extensions for <see cref="_Stopwatch"/> methods introduced in .NET 4.0
/// </summary>
public static class Stopwatch
{
    /// <summary>
    /// Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
    /// </summary>
    /// <param name="self">The <see cref="_Stopwatch"/> instance to reset.</param>
    public static void Restart(this _Stopwatch self)
    {
        self.Stop();
        self.Reset();
        self.Start();
    }
}
