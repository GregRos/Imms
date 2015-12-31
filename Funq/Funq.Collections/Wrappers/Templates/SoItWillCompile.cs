using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq;
using Funq.Abstract;

public interface __HandlerObject__<TKey>
{

}

public interface __Parent__
{
	
}

partial class __ListLikeClass__<T>
{
	protected override ISequentialBuilder<T, __ListLikeClass__<T>> EmptyBuilder {
		get;
	}


	protected override ISequentialBuilder<T, __ListLikeClass__<T>> BuilderFrom(__ListLikeClass__<T> collection) {
		throw new NotImplementedException();
	}

	public override IEnumerator<T> GetEnumerator() {
		throw new NotImplementedException();
	}
}


partial class __OrderedSetLikeClass__<T>
{
		private static __OrderedSetLikeClass__<T> Empty(__HandlerObject__<T> handler) {
		return null;
	}

	private __HandlerObject__<T> __CurrentHandler__ {
		get;
	} 

	protected override ISetBuilder<T, __OrderedSetLikeClass__<T>> EmptyBuilder {
		get;
	}

	protected override ISetBuilder<T, __OrderedSetLikeClass__<T>> BuilderFrom(__OrderedSetLikeClass__<T> collection) {
		throw new NotImplementedException();
	}

	public override IEnumerator<T> GetEnumerator() {
		throw new NotImplementedException();
	}

	public override __OrderedSetLikeClass__<T> Add(T item) {
		throw new NotImplementedException();
	}

	public override __OrderedSetLikeClass__<T> Remove(T item) {
		throw new NotImplementedException();
	}

	protected override Optional<T> TryGet(T item) {
		throw new NotImplementedException();
	}

	protected override bool IsCompatibleWith(__OrderedSetLikeClass__<T> other) {
		throw new NotImplementedException();
	}
}


partial class __SetLikeClass__<T>
{
	private __HandlerObject__<T> __CurrentHandler__ {
		get;
	} 

	private static __SetLikeClass__<T> Empty(__HandlerObject__<T> handler) {
		return null;
	}
	protected override ISetBuilder<T, __SetLikeClass__<T>> EmptyBuilder {
		get;
	}

	protected override ISetBuilder<T, __SetLikeClass__<T>> BuilderFrom(__SetLikeClass__<T> collection) {
		throw new NotImplementedException();
	}

	public override IEnumerator<T> GetEnumerator() {
		throw new NotImplementedException();
	}

	public override __SetLikeClass__<T> Add(T item) {
		throw new NotImplementedException();
	}

	public override __SetLikeClass__<T> Remove(T item) {
		throw new NotImplementedException();
	}

	protected override Optional<T> TryGet(T item) {
		throw new NotImplementedException();
	}

	protected override bool IsCompatibleWith(__SetLikeClass__<T> other) {
		throw new NotImplementedException();
	}
}

partial class __MapLikeClass__<TKey, TValue>
{
	protected override IMapBuilder<TKey, TValue, __MapLikeClass__<TKey, TValue>> EmptyBuilder {
		get;
	}

	private __HandlerObject__<TKey> __CurrentHandler__ {
		get;
	}

	private static __MapLikeClass__<TKey, TValue> Empty(__HandlerObject__<TKey> handler) {
		return null;
	}

	protected override IMapBuilder<TKey, TValue, __MapLikeClass__<TKey, TValue>> BuilderFrom(__MapLikeClass__<TKey, TValue> collection) {
		throw new NotImplementedException();
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		throw new NotImplementedException();
	}

	protected override Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
		throw new NotImplementedException();
	}

	protected override bool IsCompatibleWith(__MapLikeClass__<TKey, TValue> other) {
		throw new NotImplementedException();
	}

	protected override __MapLikeClass__<TKey, TValue> Set(TKey key, TValue value, OverwriteBehavior behavior) {
		throw new NotImplementedException();
	}

	public override __MapLikeClass__<TKey, TValue> Remove(TKey key) {
		throw new NotImplementedException();
	}
}


partial class __OrderedMapLikeClass__<TKey, TValue>
{
	protected override IMapBuilder<TKey, TValue, __OrderedMapLikeClass__<TKey, TValue>> EmptyBuilder {
		get;
	}

	private __HandlerObject__<TKey> __CurrentHandler__ {
		get;
	} 

	private static __OrderedMapLikeClass__<TKey, TValue> Empty(__HandlerObject__<TKey> handler) {
		return null;
	}


	protected override IMapBuilder<TKey, TValue, __OrderedMapLikeClass__<TKey, TValue>> BuilderFrom(__OrderedMapLikeClass__<TKey, TValue> collection) {
		throw new NotImplementedException();
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		throw new NotImplementedException();
	}

	protected override Optional<KeyValuePair<TKey, TValue>> TryGetKvp(TKey key) {
		throw new NotImplementedException();
	}

	protected override bool IsCompatibleWith(__OrderedMapLikeClass__<TKey, TValue> other) {
		throw new NotImplementedException();
	}

	protected override __OrderedMapLikeClass__<TKey, TValue> Set(TKey key, TValue value, OverwriteBehavior behavior) {
		throw new NotImplementedException();
	}

	public override __OrderedMapLikeClass__<TKey, TValue> Remove(TKey key) {
		throw new NotImplementedException();
	}
}

