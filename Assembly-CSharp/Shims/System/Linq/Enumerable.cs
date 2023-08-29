using System;
using System.Collections.Generic;

namespace Shims.NET.System.Linq;

/// <summary>
/// Provides methods for <see cref="IEnumerable{T}"/> introduced in .NET 4.0
/// </summary>
public static class Enumerable
{
    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
    /// </summary>
    /// <param name="first">The first sequence to merge.</param>
    /// <param name="second">The second sequence to merge.</param>
    /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains merged elements of two input sequences.</returns>
    /// <exception cref="ArgumentNullException"><c>first</c> or <c>second</c> is <c>null</c>.</exception>
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
    {
        if (first == null)
        {
            throw new ArgumentNullException(nameof(first));
        }

        if (second == null)
        {
            throw new ArgumentNullException(nameof(second));
        }

        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();
        while (e1.MoveNext() && e2.MoveNext())
        {
            yield return resultSelector(e1.Current, e2.Current);
        }
    }

    /// <summary>
    /// Adds a value to the beginning of the sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="element">The value to prepend to <paramref name="source"/>.</param>
    /// <returns>A new sequence that begins with <paramref name="element"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        yield return element;
        foreach (TSource e1 in source) yield return e1;
    }

    /// <summary>
    /// Adds a value to the end of the sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="element">The value to append to <paramref name="source"/>.</param>
    /// <returns>A new sequence that ends with <paramref name="element"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        foreach (TSource e1 in source) yield return e1;
        yield return element;
    }
}