﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Velir
{
	/// <summary>
	/// Wraps a NameValueCollection so it can be used an IDictionary.
	/// </summary>
	/// <remarks>
	/// Tries to stay as close to the official IDictionary spec as possible, throwing
	/// exceptions when error conditions on IDictionary would cause them, even if it
	/// would be OK on a NameValueCollection.
	/// </remarks>
	public class NameValueCollectionDictionaryAdapter : IDictionary<string, string>
	{
		private readonly NameValueCollection _nvc;

		/// <summary>
		/// Adapt a NameValueCollection into an IDictionary
		/// </summary>
		/// <param name="nvc">A NameValueCollection</param>
		public NameValueCollectionDictionaryAdapter(NameValueCollection nvc)
		{
			if (nvc == null)
			{
				throw new ArgumentNullException("nvc");
			}
			_nvc = nvc;
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		///   </returns>
		public int Count
		{
			get { return _nvc.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		///   </returns>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary>
		/// <returns>
		/// The element with the specified key.
		///   </returns>
		///   
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key"/> is null.
		/// </exception>
		///   
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
		/// The property is retrieved and <paramref name="key"/> is not found.
		/// </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
		/// </exception>
		public string this[string key]
		{
			get { return DoGet(key); }
			set { DoAdd(key, value); }
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the
		/// keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys 
		/// of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </returns>
		public ICollection<string> Keys
		{
			get { return _nvc.AllKeys; }
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the
		/// values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values 
		/// in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		///   </returns>
		public ICollection<string> Values
		{
			get { return Keys.Select(key => _nvc[key]).ToList(); }
		}

		/// <summary>
		/// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key"/> is null.
		/// </exception> 
		/// <exception cref="T:System.ArgumentException">
		/// An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </exception>		  
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
		/// </exception>
		public void Add(string key, string value)
		{				
			if (ContainsKey(key))
			{
				throw new NotSupportedException("Key already exists in dictionary.");
			}
			DoAdd(key, value);			
		}

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </exception>
		public void Add(KeyValuePair<string, string> item)
		{			
			Add(item.Key, item.Value);
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </exception>
		public void Clear()
		{
			_nvc.Clear();
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/>
		/// contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key"/> is null.
		/// </exception>
		public bool ContainsKey(string key)
		{			
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return Keys.Contains(key);
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		public bool Contains(KeyValuePair<string, string> item)
		{			
			return  item.Key != null && _nvc[item.Key] == item.Value;
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an
		/// <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements
		/// copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/>
		///  must have zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="array"/> is null.
		/// </exception>		  
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> is less than 0.
		/// </exception>		  
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="array"/> is multidimensional.
		/// -or-
		/// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
		/// -or-
		/// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is 
		/// greater than the available space from <paramref name="arrayIndex"/> to the end of the destination
		/// <paramref name="array"/>.		
		/// </exception>
		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (arrayIndex + Count > array.Length)
			{
				throw new ArgumentException("Not enough space in destination array for copying elements");
			}
			int i = arrayIndex;
			foreach (KeyValuePair<string, string> kvp in this)
			{
				array[i] = kvp;
				i += 1;
			}
		}

		/// <summary>
		/// Removes the element with the specified key from the<see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		/// true if the element is successfully removed; otherwise, false. 
		/// This method also returns false if <paramref name="key"/> was not found in the 
		/// original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key"/> is null.
		/// </exception>		  
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
		/// </exception>
		public bool Remove(string key)
		{			
			if (!ContainsKey(key))
			{
				return false;
			}
			_nvc.Remove(key);
			return true;
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the
		/// <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method 
		/// also returns false if <paramref name="item"/> is not found in the original 
		/// <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </exception>
		public bool Remove(KeyValuePair<string, string> item)
		{
			return Contains(item) && Remove(item.Key);
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">
		/// When this method returns, the value associated with the specified key, 
		/// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> 
		/// parameter. This parameter is passed uninitialized.</param>
		/// <returns>
		/// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> 
		/// contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key"/> is null.
		/// </exception>
		public bool TryGetValue(string key, out string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			value = _nvc[key];
			return ContainsKey(key);			
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used 
		/// to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return Keys.Select(key => new KeyValuePair<string, string>(key, _nvc[key])).GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used
		/// to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}		

		/// <summary>
		/// Takes care of the Get operation's error checking, etc.
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The corresponding value</returns>
		private string DoGet(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!ContainsKey(key))
			{
				throw new KeyNotFoundException("Key not found: " + key);
			}
			return _nvc[key];
		}

		/// <summary>
		/// Takes care of the Add operation's error checking
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">the value</param>
		private void DoAdd(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			_nvc[key] = value;
		}
	}
}
