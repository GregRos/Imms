
namespace Funq.Abstract
{
	public abstract class SetBuilder<TElem> : IterableBuilder<TElem>
	{
		/// <summary>
		/// Checks if an element is contained in this builder.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract bool Contains(TElem item);

		/// <summary>
		/// Removes an element from this builder.
		/// </summary>
		/// <param name="item"></param>
		public abstract void Remove(TElem item);
	}
}