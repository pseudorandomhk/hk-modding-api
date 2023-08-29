using System.Collections.Generic;
using System.Linq;
using static System.Collections.Concurrent.LockingHelper;

namespace System.Collections.Concurrent;

/// <summary>
/// Scuffed port of <c>System.Collections.Concurrent.ConcurrentQueue</c> from later .NET versions
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentQueue<T> : Queue<T>, IProducerConsumerCollection<T>
{
    private object _lock = new();

    /// <summary>
    /// Gets a value that indicates whether the <see cref="ConcurrentQueue{T}"/> is empty.
    /// </summary>
    public bool IsEmpty => this.Count == 0;

    /// <inheritdoc />
    public new void Clear() => LockAndForwardCall(_lock, base.Clear);

    /// <inheritdoc />
    public new void Enqueue(T item) => LockAndForwardCall(_lock, base.Enqueue, item);

    private IEnumerator<T> GetEnumeratorUnsafe() => this.ToList().GetEnumerator();
    /// <inheritdoc />
    public new IEnumerator<T> GetEnumerator() => LockAndForwardCall(_lock, GetEnumeratorUnsafe);

    /// <summary>
    /// INVALID OPERATION <br />
    /// Use <see cref="TryDequeue(out T)"/> instead
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown</exception>
    public new T Dequeue() => throw new InvalidOperationException();

    /// <summary>
    /// Tries to remove and return the object at the beginning of the concurrent queue.
    /// </summary>
    /// <param name="result">When this method returns, if the operation was successful, <paramref name="result"/> contains the object removed. If no object was available to be removed, the value is unspecified.</param>
    /// <returns><c>true</c> if an element was removed and returned from the beginning of the <see cref="ConcurrentQueue{T}"/> successfully; otherwise, <c>false</c>.</returns>
    public bool TryDequeue(out T result)
    {
        if (IsEmpty)
        {
            result = default;
            return false;
        }

        lock (_lock)
        {
            result = base.Dequeue();
            return true;
        }
    }

    /// <summary>
    /// INVALID OPERATION <br />
    /// Use <see cref="TryPeek(out T)"/> instead.
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown</exception>
    public new T Peek() => throw new InvalidOperationException();

    /// <summary>
    /// Tries to return an object from the beginning of the <see cref="ConcurrentQueue{T}"/> without removing it.
    /// </summary>
    /// <param name="result">When this method returns, <paramref name="result"/> contains an object from the beginning of the <see cref="ConcurrentQueue{T}"/> or an unspecified value if the operation failed.</param>
    /// <returns><c>true</c> if an object was returned successfully; otherwise, <c>false</c>.</returns>
    public bool TryPeek(out T result)
    {
        if (IsEmpty)
        {
            result = default;
            return false;
        }

        lock (_lock)
        {
            result = base.Peek();
            return true;
        }
    }

    /// <inheritdoc />
    public bool TryAdd(T item)
    {
        Enqueue(item);
        return true;
    }

    /// <inheritdoc />
    public bool TryTake(out T item) => TryDequeue(out item);
}
