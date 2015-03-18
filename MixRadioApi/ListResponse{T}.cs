// -----------------------------------------------------------------------
// <copyright file="ListResponse{T}.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Nokia.Music
{
    /// <summary>
    /// Contains the result or the error if an error occurred.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class ListResponse<T> : Response<List<T>>, IEnumerable<T>, IList<T>, IList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListResponse{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The result.</param>
        /// <param name="startIndex">The start index asked for.</param>
        /// <param name="itemsPerPage">The items per page asked for.</param>
        /// <param name="totalResults">The total results available.</param>
        /// <param name="requestId">The request id.</param>
        /// <param name="foundMixRadioHeader">The X-Mix-Radio header state.</param>
        internal ListResponse(HttpStatusCode? statusCode, List<T> result, int? startIndex, int? itemsPerPage, int? totalResults, Guid requestId, bool? foundMixRadioHeader = null)
            : base(statusCode, result, requestId, foundMixRadioHeader)
        {
            this.StartIndex = startIndex;
            this.ItemsPerPage = itemsPerPage;
            this.TotalResults = totalResults;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResponse{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The HTTP Status code</param>
        /// <param name="error">The error.</param>
        /// <param name="errorResponseBody">The response body</param>
        /// <param name="requestId">The request id.</param>
        /// <param name="foundMixRadioHeader">The X-Mix-Radio header state.</param>
        internal ListResponse(HttpStatusCode? statusCode, Exception error, string errorResponseBody, Guid requestId, bool? foundMixRadioHeader = null)
            : base(statusCode, error, errorResponseBody, requestId, foundMixRadioHeader)
        {
        }

        /// <summary>
        /// Gets the items per page the API call was asked for.
        /// </summary>
        /// <value>
        /// The items per page.
        /// </value>
        public int? ItemsPerPage { get; private set; }

        /// <summary>
        /// Gets the start index the API call was asked for.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int? StartIndex { get; private set; }

        /// <summary>
        /// Gets the total results available.
        /// </summary>
        /// <value>
        /// The total results.
        /// </value>
        public int? TotalResults { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            get
            {
                if (this.Result != null)
                {
                    return this.Result.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, false.</returns>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
        public object SyncRoot
        {
            get
            {
                return this.Result;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The item at the index</returns>
        public T this[int index]
        {
            get
            {
                if (this.Result != null)
                {
                    return this.Result[index];
                }
                else
                {
                    return default(T);
                }
            }

            set
            {
                if (this.Result != null)
                {
                    this.Result[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The object at the specified index</returns>
        object IList.this[int index]
        {
            get
            {
                return (this as IList<T>)[index];
            }

            set
            {
                (this as IList<T>)[index] = (T)value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this.Result != null)
            {
                return this.Result.GetEnumerator();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.Result != null)
            {
                return this.Result.GetEnumerator();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            if (this.Result != null)
            {
                return this.Result.IndexOf(item);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>
        /// The index of <paramref name="value" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(object value)
        {
            return (this as IList<T>).IndexOf((T)value);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(T item)
        {
            if (this.Result != null)
            {
                this.Result.Add(item);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to add to the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        public int Add(object value)
        {
            (this as IList<T>).Add((T)value);
            return this.Count;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, T item)
        {
            if (this.Result != null)
            {
                this.Result.Insert(index, item);
            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to insert into the <see cref="T:System.Collections.IList" />.</param>
        public void Insert(int index, object value)
        {
            (this as IList<T>).Insert(index, (T)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(T item)
        {
            if (this.Result != null)
            {
                return this.Result.Remove(item);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to remove from the <see cref="T:System.Collections.IList" />.</param>
        public void Remove(object value)
        {
            (this as IList<T>).Remove((T)value);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if (this.Result != null)
            {
                this.Result.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            if (this.Result != null)
            {
                this.Result.Clear();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            if (this.Result != null)
            {
                return this.Result.Contains(item);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, false.
        /// </returns>
        public bool Contains(object value)
        {
            return (this as IList<T>).Contains((T)value);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (this.Result != null)
            {
                this.Result.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            if (this.Result != null)
            {
                T[] tmpArray = new T[this.Count];
                this.Result.CopyTo(tmpArray, index);
                array = tmpArray;
            }
        }
    }
}
