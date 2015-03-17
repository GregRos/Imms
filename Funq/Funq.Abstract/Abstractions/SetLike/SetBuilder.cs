
namespace Funq.Abstract
{
	public abstract class SetBuilder<TElem> : IterableBuilder<TElem>
	{
		public abstract bool Contains(TElem item);

		public abstract void Remove(TElem item);
	}
}