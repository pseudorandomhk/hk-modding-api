using _StringBuilder = System.Text.StringBuilder;

namespace Shims.NET.System.Text;

/// <summary>
/// Port of <see cref="StringBuilder"/> methods from later .NET versions
/// </summary>
public static class StringBuilder
{
    /// <summary>
    /// Removes all characters from the current <see cref="StringBuilder"/> instance.
    /// </summary>
    /// <param name="self">A <see cref="StringBuilder"/> instance</param>
    /// <returns><paramref name="self"/></returns>
    public static _StringBuilder Clear(this _StringBuilder self)
    {
        self.Length = 0;
        return self;
    }
}
