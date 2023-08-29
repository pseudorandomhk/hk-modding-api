namespace System.Collections.Generic;

/// <summary>
/// Represents a strongly-typed, read-only collection of elements.
/// </summary>
/// <typeparam name="T">The type of the elements.</typeparam>
public interface IReadOnlyList<T> : IEnumerable<T>
{
    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element at the specified index in the read-only list.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    T this[int index] { get; }
}
