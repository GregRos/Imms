namespace Funq
{
	internal class Box<TValue>
	{
		private readonly TValue _value;

		public Box(TValue value)
		{
			_value = value;
		}

		public TValue Value
		{
			get
			{
				return _value;
			}
		}
	}
}