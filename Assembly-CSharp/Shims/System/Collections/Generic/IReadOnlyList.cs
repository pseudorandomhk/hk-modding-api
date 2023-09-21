using System.Collections.Generic._Impl.IReadOnlyList;
using System.Collections.ObjectModel;

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly-typed, read-only collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IReadOnlyList<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
    }

    namespace _Impl.IReadOnlyList {
        internal sealed class IListImpl<T> : IReadOnlyList<T>
        {
            private IList<T> _backingList;

            public IListImpl(IList<T> original) { this._backingList = original; }

            public int Count => _backingList.Count;

            public T this[int index] => _backingList[index];

            public IEnumerator<T> GetEnumerator() => _backingList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    /// <summary>
    /// Extension class for using various containers as <see cref="IReadOnlyList{T}"/>
    /// </summary>
    public static class IReadOnlyListConversionsExtension
    {
        /// <summary>
        /// Converts <paramref name="list"/> to an <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns><paramref name="list"/> as an <see cref="IReadOnlyList{T}"/>.</returns>
        public static IReadOnlyList<T> AsIReadOnlyList<T>(this IList<T> list) => new IListImpl<T>(list);
    }
}
