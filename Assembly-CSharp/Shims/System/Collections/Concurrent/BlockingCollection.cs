using System.Collections.Generic;
using static System.Collections.Concurrent.LockingHelper;

namespace System.Collections.Concurrent;

/// <summary>
/// Port of .NET 4.0 <c>BlockingCollection&lt;T&gt;</c>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BlockingCollection<T> : IDisposable, IReadOnlyCollection<T>, ICollection, IEnumerable<T>
{
    private IProducerConsumerCollection<T> _backingCollection;
    private bool _isAddingComplete;

    /// <summary>
    /// Creates an instance of <see cref="BlockingCollection{T}"/>.
    /// </summary>
    public BlockingCollection()
    {
        _backingCollection = new ConcurrentQueue<T>();
        _isAddingComplete = false;
    }

    /// <summary>
    /// Returns the number of elements in the current <see cref="BlockingCollection{T}"/>.
    /// </summary>
    public int Count { get =>  _backingCollection.Count; }

    /// <summary>
    /// Adds <paramref name="item"/> to the current <see cref="BlockingCollection{T}"/>, blocking if it is being modified.
    /// </summary>
    /// <param name="item">The item to add to the current <see cref="BlockingCollection{T}"/>.</param>
    public void Add(T item)
    {
        if (_isAddingComplete) throw new InvalidOperationException("This collection is not accepting additions.");
        _backingCollection.TryAdd(item);
    }

    /// <summary>
    /// Provides a consuming <see cref="IEnumerable{T}"/> for items in the collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> that removes and returns items from the collection.</returns>
    public IEnumerable<T> GetConsumingEnumerable()
    {
        if (_isAddingComplete) yield break;

        while (!_isAddingComplete)
            if (_backingCollection.TryTake(out T item))
                yield return item;
    }

    /// <summary>
    /// Marks the <see cref="BlockingCollection{T}"/> instance as not accepting any more additions.
    /// </summary>
    public void CompleteAdding() => _isAddingComplete = true;

    /// <inheritdoc />
    public IEnumerator GetEnumerator() => _backingCollection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _backingCollection.GetEnumerator();

    /// <inheritdoc />
    public void CopyTo(Array array, int index)
    {
        T[] genericArray = _backingCollection.ToArray();
        Array.Copy(genericArray, 0, array, index, genericArray.Length);
    }

    /// <inheritdoc />
    public object SyncRoot => throw new NotSupportedException();

    /// <inheritdoc />
    public bool IsSynchronized => false;

    /// <inheritdoc />
    public void Dispose() { }
}
