using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Spect.Net.SpectrumEmu.Utility
{
    /// <summary>
    /// This class implements an LRU list with a fixed capacity that supports only
    /// Add operations.
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <remarks>
    /// Whenever the capacity is exceeded, a newly added item erase the oldest one.
    /// </remarks>
    public class LruList<T>: IList<T>
    {
        // --- Index that shows the write position
        private int _writeIndex;

        // --- Index that shows the read position
        private int _readIndex;

        // --- Number of items
        private int _count;

        // --- The list that holds the items
        private readonly List<T> _items;

        public int Capacity { get; }


        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public LruList(int capacity = 10)
        {
            Capacity = capacity;
            _items = new List<T>(capacity);
            Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can 
        /// be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerates the items in the list
        /// </summary>
        private IEnumerable<T> Enumerate()
        {
            var readIndex = _readIndex;
            for (var i = 0; i < _count; i++)
            {
                yield return _items[readIndex];
                readIndex = (readIndex + 1) % Capacity;
            }
        }

        /// <summary>
        /// Enumerates the items in the list in reverse order
        /// </summary>
        public IEnumerable<T> Reverse()
        {
            var readIndex = (_readIndex + _count - 1) % Capacity;
            for (var i = 0; i < _count; i++)
            {
                yield return _items[readIndex];
                readIndex = (readIndex - 1 + Capacity) % Capacity;
            }
        }
        
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </param>
        /// <exception cref="T:System.NotSupportedException">The 
        /// <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Add(T item)
        {
            if (_count < Capacity)
            {
                _count++;
            }
            else
            {
                _readIndex = (_readIndex + 1) % Capacity;
            }
            _items[_writeIndex] = item;
            _writeIndex = (_writeIndex + 1) % Capacity;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The 
        /// <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. 
        /// </exception>
        public void Clear()
        {
            _readIndex = 0;
            _writeIndex = 0;
            _count = 0;
            _items.Clear();
            for (var i = 0; i < Capacity; i++)
            {
                _items.Add(default(T));
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> 
        /// contains a specific value.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the 
        /// <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, 
        /// <see langword="false" />.
        /// </returns>
        public bool Contains(T item) => Enumerate().Any(t => t.Equals(item));

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> 
        /// to an <see cref="T:System.Array" />, starting at a particular 
        /// <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array" /> that is the destination 
        /// of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. 
        /// The <see cref="T:System.Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> 
        /// is greater than the available space from <paramref name="arrayIndex" /> to the 
        /// end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            var idx = arrayIndex;
            foreach (var item in Enumerate())
            {
                array[idx++] = item;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the 
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> was successfully 
        /// removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; 
        /// otherwise, <see langword="false" />. This method also returns 
        /// <see langword="false" /> if <paramref name="item" /> is not found in the original 
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public bool Remove(T item) => throw new NotSupportedException();

        /// <summary>
        /// Gets the number of elements contained in the 
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public int Count => _count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> 
        /// is read-only.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> 
        /// is read-only; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsReadOnly => false;

        /// <summary>
        /// Determines the index of a specific item in the 
        /// <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            var idx = 0;
            foreach (var t in Enumerate())
            {
                if (t.Equals(item))
                {
                    return idx;
                }
                idx++;
            }
            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> 
        /// at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which <paramref name="item" /> should be inserted.
        /// </param>
        /// <param name="item">
        /// The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the 
        /// <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
        /// </exception>
        public void Insert(int index, T item) => throw new NotSupportedException();

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> 
        /// item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the 
        /// <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
        /// </exception>
        public void RemoveAt(int index) => throw new NotSupportedException();

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the 
        /// <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> 
        /// is read-only.
        /// </exception>
        public T this[int index]
        {
            get => _items[(_readIndex + index) % Capacity];
            set => throw new NotSupportedException();
        }
    }
}