using _Vector2 = UnityEngine.Vector2;

namespace Shims.UnityEngine;

/// <summary>
/// Provides <see cref="Vector2"/> methods introduced in Unity 2018.1
/// </summary>
public static class Vector2
{
    /// <summary>
    /// Multiplies a vector by another vector.
    /// </summary>
    /// <param name="a">The first vector to multiply</param>
    /// <param name="b">The second vector to multiply</param>
    /// <returns>The result of multiplying each component of <c>a</c> by its matching component from <c>b</c></returns>
    public static _Vector2 multiply(_Vector2 a, _Vector2 b)
    {
        return new _Vector2(a.x * b.x, a.y * b.y);
    }
}