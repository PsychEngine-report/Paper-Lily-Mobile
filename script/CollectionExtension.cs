using System;
using System.Collections.Generic;
using System.Linq;

namespace LacieEngine.Core
{
	public static class CollectionExtension
	{
		public static bool IsEmpty<T>(this ICollection<T> collection)
		{
			return collection.Count == 0;
		}

		public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
		{
			return collection?.IsEmpty() ?? true;
		}

		public static string ToUsefulString<T>(this ICollection<T> collection)
		{
			return string.Join(",", collection);
		}

		public static bool IsEmpty<T>(this Queue<T> collection)
		{
			return collection.Count == 0;
		}

		public static bool IsNullOrEmpty<T>(this Queue<T> collection)
		{
			return collection?.IsEmpty() ?? true;
		}

		public static bool Contains<T>(this IList<T> array, T o)
		{
			foreach (T item in array)
			{
				if (item.Equals(o))
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveAll<K, V>(this IDictionary<K, V> dictionary, Func<K, V, bool> predicate)
		{
			foreach (K key in from val in dictionary.Keys.ToArray()
				where predicate(val, dictionary[val])
				select val)
			{
				dictionary.Remove(key);
			}
		}

		public static void AddRange<T>(this LinkedList<T> linkedList, IEnumerable<T> collection)
		{
			foreach (T element in collection)
			{
				linkedList.AddLast(element);
			}
		}

		public static void RemoveAll<T>(this LinkedList<T> linkedList, Func<T, bool> predicate)
		{
			LinkedListNode<T> node = linkedList.First;
			while (node != null)
			{
				LinkedListNode<T> next = node.Next;
				if (predicate(node.Value))
				{
					linkedList.Remove(node);
				}
				node = next;
			}
		}
	}
}
