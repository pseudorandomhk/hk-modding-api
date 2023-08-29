using _PropertyInfo = System.Reflection.PropertyInfo;

namespace Shims.NET.System.Reflection;

/// <summary>
/// Extension methods for <see cref="PropertyInfo"/>
/// </summary>
public static class PropertyInfo
{
    /// <summary>
    /// Returns the property value of a specified object.
    /// </summary>
    /// <param name="self">The <see cref="_PropertyInfo"/> describing the property to get</param>
    /// <param name="obj">The object whose property value will be returned.</param>
    /// <returns></returns>
    public static object GetValue(this _PropertyInfo self, object obj) => self.GetValue(obj, null);

    /// <summary>
    /// Sets the property value of a specified object.
    /// </summary>
    /// <param name="self">The <see cref="_PropertyInfo"/> describing the property to set</param>
    /// <param name="obj">The object whose property value will be set.</param>
    /// <param name="value">The new property value.</param>
    public static void SetValue(this _PropertyInfo self, object obj, object value) => self.SetValue(obj, value, null);
}
