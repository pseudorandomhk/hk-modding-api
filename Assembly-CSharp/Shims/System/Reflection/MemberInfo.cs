using System;
using System.Collections.Generic;
using System.Linq;
using _MemberInfo = System.Reflection.MemberInfo;

namespace Shims.NET.System.Reflection;

/// <summary>
/// Provides methods for <see cref="System.Reflection.MemberInfo"/> introduced in .NET 4.0
/// </summary>
public static class MemberInfo
{
    /// <summary>
    /// Retrieves a custom attribute of a specified type that is applied to a specified member, and optionally inspects the ancestors of that member.
    /// </summary>
    /// <param name="instance">The member to inspect.</param>
    /// <param name="inherit"><c>true</c> to inspect the ancestors of <c>element</c>; otherwise, <c>false</c>.</param>
    /// <typeparam name="T">The type of attribute to search for.</typeparam>
    /// <returns>A custom attribute that matches <c>attributeType</c>, or <c>null</c> if no such attribute is found.</returns>
    public static T GetCustomAttribute<T>(this _MemberInfo instance, bool inherit = false) where T : Attribute
    {
        return (T)Attribute.GetCustomAttribute(instance, typeof(T), inherit);
    }

    /// <summary>
    /// Retrieves the custom attributes of a specified type that is applied to a specified member, and optionally inspects the ancestors of that member.
    /// </summary>
    /// <param name="instance">The member to inspect.</param>
    /// <param name="inherit"><c>true</c> to inspect the ancestors of <c>element</c>; otherwise, <c>false</c>.</param>
    /// <typeparam name="T">The type of attribute to search for.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> of custom attributes that matches <c>attributeType</c>, or <c>null</c> if no such attribute is found.</returns>
    public static IEnumerable<T> GetCustomAttributes<T>(this _MemberInfo instance, bool inherit = false) where T : Attribute
    {
        return (IEnumerable<T>)Attribute.GetCustomAttributes(instance, typeof(T), inherit).AsEnumerable();
    }
}