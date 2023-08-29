namespace Shims.NET.System;

/// <summary>
/// Provides static <c>System::Array</c> methods introduced in .NET 4.0
/// </summary>
public static class Array
{
    private static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
    
    /// <summary>
    /// Returns an empty array.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <returns>An empty array.</returns>
    public static T[] Empty<T>()
    {
        return EmptyArray<T>.Value;
    }
}