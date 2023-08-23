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
}