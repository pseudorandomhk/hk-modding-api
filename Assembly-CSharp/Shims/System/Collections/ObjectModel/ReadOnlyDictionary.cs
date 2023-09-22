using System.Collections.Generic;

namespace System.Collections.ObjectModel;

#pragma warning disable CS1591

/// <summary>
/// Low effort port of .NET 4.0 <c>ReadOnlyDictionary&lt;TKey, TValue&gt;</c>.
/// </summary>
/// <typeparam name="TKey">The type of the keys</typeparam>
/// <typeparam name="TValue">The type of the values</typeparam>
public class ReadOnlyDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>,
    IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IDictionary
{
    private IDictionary<TKey, TValue> _dictionary;

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
    {
        this._dictionary = dictionary;
    }

    public int Count => _dictionary.Count;
    public IDictionary<TKey, TValue> Dictionary => _dictionary;
    public TValue this[TKey key] => _dictionary[key];
    public ICollection<TKey> Keys => _dictionary.Keys;
    public ICollection<TValue> Values => _dictionary.Values;

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

    void ICollection.CopyTo(Array array, int index) => ((ICollection)_dictionary).CopyTo(array, index);
    bool ICollection.IsSynchronized => ((ICollection)_dictionary).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();
    void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw new NotSupportedException();
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();
    void IDictionary.Add(object key, object value) => throw new NotSupportedException();
    void IDictionary.Clear() => throw new NotSupportedException();
    bool IDictionary.Contains(object key) => key is TKey k ? ContainsKey(k) : false;
    IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)_dictionary).GetEnumerator();
    bool IDictionary.IsFixedSize => true;
    bool IDictionary.IsReadOnly => true;
    object IDictionary.this[object key]
    {
        get => key is TKey k ? this[k] : null;
        set => throw new NotSupportedException();
    }
    ICollection IDictionary.Keys => (ICollection)Keys;
    void IDictionary.Remove(object key) => throw new NotSupportedException();
    ICollection IDictionary.Values => (ICollection)Values;
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw new NotSupportedException();
    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => this[key];
        set => throw new NotSupportedException();
    }
    bool IDictionary<TKey, TValue>.Remove(TKey key) => throw new NotSupportedException();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();
}
