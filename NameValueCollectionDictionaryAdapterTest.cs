using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace Velir
{
	[TestFixture]
	public class NameValueCollectionDictionaryAdapterTest
	{
		private NameValueCollection _nvc;
		private IDictionary<string, string> _adapted;

		[SetUp]
		public void Setup()
		{
			_nvc = new NameValueCollection();
			_nvc.Add("foo", "1");
			_nvc.Add("foo", "2");
			_nvc["bar"] = "abc";
			_nvc["baz"] = null;
			_adapted = new NameValueCollectionDictionaryAdapter(_nvc);
		}

		[Test]
		public void Test_Constructor_Null_NVC()
		{
			Assert.Throws<ArgumentNullException>(() => _adapted = new NameValueCollectionDictionaryAdapter(null));
		}

		[Test]
		public void Test_Initial_Properties()
		{
			Assert.IsFalse(_adapted.IsReadOnly);
			Assert.AreEqual(3, _adapted.Count);
		}

		[Test]
		public void Test_Indexer()
		{
			Assert.AreEqual("1,2", _adapted["foo"]);
			_adapted["foo"] = "567";			
			Assert.AreEqual("567", _nvc["foo"]);
			Assert.AreEqual("567", _adapted["foo"]);
		}

		[Test]
		public void Test_Indexer_Null_Key()
		{
			string v;
			Assert.Throws<ArgumentNullException>(() => v = _adapted[null]);
			Assert.Throws<ArgumentNullException>(() => _adapted[null] = "foo");
		}

		[Test]
		public void Test_Indexer_Key_Does_Not_Exist()
		{
			string v;
			Assert.Throws<KeyNotFoundException>(() => v = _adapted["testing"]);
		}
		
		[Test]
		public void Test_Keys()
		{
			Assert.AreEqual(new[] { "foo", "bar", "baz" }, _adapted.Keys);
			_nvc["abc"] = "123";
			Assert.AreEqual(new[] { "foo", "bar", "baz", "abc" }, _adapted.Keys);
		}

		[Test]
		public void Test_Values()
		{
			Assert.AreEqual(new[] { "1,2", "abc", null }, _adapted.Values);
			_nvc["abc"] = "123";
			Assert.AreEqual(new[] { "1,2", "abc", null, "123" }, _adapted.Values);
		}

		[Test]
		public void Test_ContainsKey()
		{
			Assert.IsTrue(_adapted.ContainsKey("foo"));
			Assert.IsFalse(_adapted.ContainsKey("h"));
		}

		[Test]
		public void Test_ContainsKey_Null_Key()
		{
			Assert.Throws<ArgumentNullException>(() => _adapted.ContainsKey(null));
		}

		[Test]
		public void Test_Contains_Via_KeyValuePair()
		{
			KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("foo", "1,2");
			Assert.IsTrue(_adapted.Contains(kvp));
			kvp = new KeyValuePair<string, string>("foo", "bar");
			Assert.IsFalse(_adapted.Contains(kvp));
		}

		[Test]
		public void Test_TryGetValue()
		{
			string val;
			Assert.IsTrue(_adapted.TryGetValue("bar", out val));
			Assert.AreEqual("abc", val);
			Assert.IsFalse(_adapted.TryGetValue("hello", out val));
			Assert.IsNull(val);
		}

		[Test]
		public void Test_TryGetValue_Null_Key()
		{
			string val;
			Assert.Throws<ArgumentNullException>(() => _adapted.TryGetValue(null, out val));
		}

		[Test]
		public void Test_Add()
		{
			_adapted.Add("hello", "world");
			Assert.AreEqual("world", _nvc["hello"]);
		}

		[Test]
		public void Test_Add_With_Null_Key()
		{
			Assert.Throws<ArgumentNullException>(() => _adapted.Add(null, "test"));
		}

		[Test]
		public void Test_Add_With_Existing_Key()
		{
			Assert.Throws<ArgumentNullException>(() => _adapted.Add(null, "test"));
		}

		[Test]
		public void Test_Add_Via_KeyValuePair()
		{
			KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("k", "v");
			_adapted.Add(kvp);
			Assert.AreEqual("v", _nvc["k"]);
		}
		
		[Test]
		public void Test_Remove()
		{
			Assert.IsTrue(_adapted.Remove("foo"));	
			Assert.AreEqual(2, _nvc.Count);
			Assert.IsNull(_nvc["foo"]);
			Assert.IsFalse(_adapted.Remove("akey"));
		}

		[Test]
		public void Test_Remove_Via_KeyValuePair()
		{
			KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("foo", "1,2");
			Assert.IsTrue(_adapted.Remove(kvp));
			Assert.AreEqual(2, _nvc.Count);
			Assert.IsNull(_nvc["foo"]);
			kvp = new KeyValuePair<string, string>("bar", "test");
			Assert.IsFalse(_adapted.Remove(kvp));
		}

		[Test]
		public void Test_Remove_Null_Key()
		{
			Assert.Throws<ArgumentNullException>(() => _adapted.Remove(null));			
		}

		[Test]
		public void Test_Clear()
		{
			_adapted.Clear();
			Assert.AreEqual(0, _nvc.Count);
		}

		[Test]
		public void Test_GetEnumerator()
		{
			IEnumerator<KeyValuePair<string, string>> enumerator = _adapted.GetEnumerator();
			
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual("foo", enumerator.Current.Key);
			Assert.AreEqual("1,2", enumerator.Current.Value);

			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual("bar", enumerator.Current.Key);
			Assert.AreEqual("abc", enumerator.Current.Value);

			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual("baz", enumerator.Current.Key);
			Assert.AreEqual(null, enumerator.Current.Value);

			Assert.IsFalse(enumerator.MoveNext());			
		}

		[Test]
		public void Test_CopyTo()
		{
			KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[6];
			_adapted.CopyTo(array, 2);
			Assert.AreEqual("foo", array[2].Key);
			Assert.AreEqual("1,2", array[2].Value);
			Assert.AreEqual("bar", array[3].Key);
			Assert.AreEqual("abc", array[3].Value);
			Assert.AreEqual("baz", array[4].Key);
			Assert.AreEqual(null, array[4].Value);
		}

		[Test]
		public void Test_CopyTo_Null_Array()
		{
			Assert.Throws<ArgumentNullException>(() => _adapted.CopyTo(null, 0));
		}

		[Test]
		public void Test_CopyTo_Invalid_Index()
		{
			KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[6];
			Assert.Throws<ArgumentOutOfRangeException>(() => _adapted.CopyTo(array, -2));
		}

		[Test]
		public void Test_CopyTo_Not_Enough_Space_In_Array()
		{
			KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[6];
			Assert.Throws<ArgumentException>(() => _adapted.CopyTo(array, 4));
		}
	}
}
