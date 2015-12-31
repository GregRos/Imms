using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Imm;

namespace Imm.Collections.Mutable
{
	public class MutableMap<TKey,TValue> : MapLike<TKey, TValue, MutableMap<TKey, TValue>>
	{

		private class Builder : MapBuilder<TKey, TValue>
		{
			private readonly Dictionary<TKey, TValue> _inner;

			public Builder()
			{
				_inner = new Dictionary<TKey, TValue>();
			}

			public Builder(Dictionary<TKey, TValue> inner)
			{
				_inner = inner;
			}

			public override object Result
			{
				get
				{
					return _inner;
				}
			}

			public override void EnsureCapacity(int n)
			{
				
			}

			protected override void add(Kvp<TKey, TValue> item)
			{
				_inner[item.Key] = item.Value;
			}

			protected override Option<TValue> lookup(TKey k)
			{
				return _inner.ContainsKey(k) ? Option.Some(_inner[k]) : Option.None;
			}
		}

		private readonly Dictionary<TKey, TValue> _inner;

		private MutableMap(Dictionary<TKey, TValue> map)
		{
			_inner = new Dictionary<TKey, TValue>(map);
		}

		public static implicit operator MutableMap<TKey, TValue> (Dictionary<TKey, TValue> map)
		{
			return new MutableMap<TKey, TValue>(map);
		}

		protected override MapBuilder<TKey, TValue> EmptyBuilder
		{
			get
			{
				return new Builder();
			}
		}

		protected override MapBuilder<TKey, TValue> BuilderFrom(MutableMap<TKey, TValue> provider)
		{
			return new Builder(provider._inner);
		}

		protected override IEnumerator<Kvp<TKey, TValue>> GetEnumerator()
		{
			foreach (var kvp in _inner) yield return kvp;
		}

		protected override MutableMap<TKey, TValue> ProviderFrom(MapBuilder<TKey, TValue> builder)
		{
			return new MutableMap<TKey, TValue>((Dictionary<TKey, TValue>)builder.Result);
		}

		public override Option<TValue> TryGet(TKey k)
		{
			return _inner.ContainsKey(k) ? Option.Some(_inner[k]) : Option.None;
		}

		public TValue this[TKey k]
		{
			get
			{
				return base.Get(k);
			}
			set
			{
				_inner[k] = value;
			}
		}

		public void Add(TKey k, TValue v)
		{
			_inner.Add(k, v);
		}

		public bool Remove(TKey k)
		{
			return _inner.Remove(k);
		}

		public override int Length
		{
			get
			{
				return _inner.Count;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return this.Length == 0;
			}
		}

	}
}
