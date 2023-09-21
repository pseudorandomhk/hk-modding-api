using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Concurrent;

/// <summary>
/// Port of .NET 4.0 <c>IProducerConsumerCollection&lt;T&gt;</c>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IProducerConsumerCollection<T> : IEnumerable<T>, ICollection
{
    /// <summary>
    /// Copies the elements of the <see cref="IProducerConsumerCollection{T}"/> to an <see cref="Array"/>, starting at a specified index.
    /// </summary>
    /// <param name="array">The array to copy elements to.</param>
    /// <param name="arrayIndex">The index of <paramref name="array"/> to start copying elements to.</param>
    void CopyTo(T[] array, int arrayIndex);

    /// <summary>
    /// Copies the elements in the <see cref="IProducerConsumerCollection{T}"/> to a new array.
    /// </summary>
    /// <returns>A new array containing the elements of this <see cref="IProducerConsumerCollection{T}"/>.</returns>
    T[] ToArray();

    /// <summary>
    /// Attempts to add an object to the <see cref="IProducerConsumerCollection{T}"/>.
    /// </summary>
    /// <param name="item">The object to add.</param>
    /// <returns><c>true</c> if the object was successfully added; <c>false</c> otherwise.</returns>
    bool TryAdd(T item);

    /// <summary>
    /// Attempts to remove and return an object from the <see cref="IProducerConsumerCollection{T}"/>.
    /// </summary>
    /// <param name="item">When this method returns, if the object was removed and returned successfully, <paramref name="item"/> contains the removed object. If no object was available to be removed, the value is unspecified.</param>
    /// <returns><c>true</c> if the object was removed and returned successfully; otherwise, <c>false</c>.</returns>
    bool TryTake(out T item);
}
