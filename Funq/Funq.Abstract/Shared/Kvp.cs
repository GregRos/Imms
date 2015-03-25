using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using Funq;

namespace Funq
{
	[DebuggerDisplay("{DebuggerDisplayString,nq}")]
	[DebuggerTypeProxy(typeof(Kvp<,>.KvpDebugView))]
	public struct Kvp<TKey, TValue> : IEquatable<Kvp<TKey, TValue>>
	{
		private readonly TKey key;
		private readonly TValue value;

		private class KvpDebugView
		{
			private Kvp<TKey, TValue> _inner;
			public KvpDebugView(Kvp<TKey, TValue> o)
			{
				_inner = o;
			}

			public TKey Key
			{
				get
				{
					return _inner.Key;
				}
			}

			public TValue Value
			{
				get
				{
					return _inner.Value;
				}
			}
		}

		public Kvp(TKey key, TValue value) : this()
		{
			this.key = key;
			this.value = value;
		}


		public override string ToString()
		{
			return string.Format("{{{0} :: {1}}}", Key, Value);
			
		}

		private string DebuggerDisplayString
		{
			get
			{
				return ToString();
			}
		}

		public bool Equals(Kvp<TKey, TValue> other)
		{
			return Equals(Key, other.Key) && Equals(Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (obj is Kvp<TKey, TValue>)
				return Equals((Kvp<TKey, TValue>) obj);
			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (13 * Key.GetHashCode()) ^ Value.GetHashCode();
			}
		}

		public static implicit operator KeyValuePair<TKey,TValue>(Kvp<TKey, TValue> self)
		{
			return new KeyValuePair<TKey, TValue>(self.Key, self.Value);
		}

		public static implicit operator Kvp<TKey, TValue>(KeyValuePair<TKey, TValue> bclKvp)
		{
			return Kvp.Of(bclKvp.Key, bclKvp.Value);
		}

		public static implicit operator Kvp<TKey, TValue>(Tuple<TKey, TValue> bclTuple)
		{
			return Kvp.Of(bclTuple.Item1, bclTuple.Item2);
		}

		public static implicit operator Tuple<TKey, TValue>(Kvp<TKey, TValue> funqTuple)
		{
			return Tuple.Create(funqTuple.Key, funqTuple.Value);
		}

		public TKey Key
		{
			get
			{
				return key;
			}
		}

		public TValue Value
		{
			get
			{
				return value;
			}
		}

		public Kvp<TRKey, TValue> OnKey<TRKey>(Func<TKey, TRKey> transform)
		{
			return Kvp.Of(transform(key), value);
		}

		public Kvp<TKey, TRValue> OnValue<TRValue>(Func<TValue, TRValue> transform)
		{
			return Kvp.Of(key, transform(value));
		}
	}
}

namespace Funq
{
	public static class Kvp
	{
		public static Kvp<TKey, TValue> Of<TKey, TValue>(TKey k, TValue v)
		{
			return new Kvp<TKey, TValue>(k, v);
		}
	}
}