namespace Funq.Abstract
{
	/// <summary>
	/// Used for generalizing over list-like collections when the concrete collection type is unknown.
	/// </summary>
	/// <typeparam name="TElem"></typeparam>
	public interface ITrait_Sequential<TElem> : ITrait_CollectionBuilderFactory<TElem, IterableBuilder<TElem>>
	{
		TElem this[int index]
		{
			get;
		}

		TElem First
		{
			get;
		}

		TElem Last
		{
			get;
		}

		Option<TElem> TryFirst
		{
			get;
		}

		Option<TElem> TryLast
		{
			get;
		}

		void CopyTo(TElem[] arr, int start, int count);
	}
}