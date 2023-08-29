using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent;

/// <summary>
/// Partial implementation of System::Collections::Concurrent::ConcurrentDictionary from .NET 4.0+.
/// No locking is performed on reads, and the only write operations used within mapi are
/// indexer and <c>TryAdd</c>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    private object _lock = new();

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set</param>
    public new TValue this[TKey key]
    {
        get => base[key];
        set
        {
            lock (_lock)
            {
                base[key] = value;
            }
        }
    }

    /// <summary>
    /// Attempts to add the specified key and value to the <see cref="ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
    /// <returns><c>true</c> if the key/value pair was added to the <see cref="ConcurrentDictionary{TKey,TValue}"/> successfully;
    ///          <c>false</c> if the key already exists.</returns>
    /// <exception cref="ArgumentNullException"><c>key</c> is <c>null</c>.</exception>
    public bool TryAdd(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (ContainsKey(key))
        {
            return false;
        }

        lock (_lock)
        {
            Add(key, value);
            return true;
        }
    }
}