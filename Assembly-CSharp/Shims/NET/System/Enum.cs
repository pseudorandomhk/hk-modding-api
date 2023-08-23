using System;

using _Enum = System.Enum;

namespace Shims.NET.System;

/// <summary>
/// Provides <see cref="Enum"/> methods introduced in .NET 4.0.
/// </summary>
public static class Enum
{
    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants
    /// to an equivalent enumerated object. The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="value">The case-sensitive string representation of the enumeration name or underlying value to convert.</param>
    /// <param name="result">When this method returns, contains an object of type <c>TEnum</c> whose value is represented by
    ///                      <c>value</c> if the parse operation succeeds. If the parse operation fails, contains the default
    ///                      value of the underlying type of <c>TEnum</c>. This parameter is passed uninitialized.</param>
    /// <typeparam name="TEnum">The enumeration type to which to convert <c>value</c>.</typeparam>
    /// <returns><c>true</c> if the <c>value</c> parameter was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse<TEnum>(string value, out TEnum result)
    {
        try
        {
            result = (TEnum)_Enum.Parse(typeof(TEnum), value);
            return true;
        }
        catch (Exception)
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Determines whether one or more bit fields are set in the current instance.
    /// </summary>
    /// <param name="instance">An <see cref="Enum"/> instance</param>
    /// <param name="flag">An enumeration value.</param>
    /// <typeparam name="TEnum">The type of <c>instance</c></typeparam>
    /// <returns><c>true</c> if the bit field or bit fields that are set in <c>flag</c> are also set in the current instance; otherwise, <c>false</c>.</returns>
    public static bool HasFlag<TEnum>(this TEnum instance, TEnum flag) where TEnum : _Enum
    {
        var flagMask = Convert.ToUInt64(flag);
        return (Convert.ToUInt64(instance) & flagMask) == flagMask;
    }
}