namespace Imms.Examples {
	internal class Optional {
		private static void Main(string[] args) {
		}

		private static void OptionalExamples() {
			{
				#region Optional_Map
				Optional<int> a = 5;
				Optional<string> str = a.Map(x => x.ToString());
				#endregion
			}

			{
				#region Optional_Map_Optional
				Optional<int> a = 5;
				Optional<string> str = a.Map(x => x % 2 == 0 ? Imms.Optional.Some(x.ToString()) : Imms.Optional.None);
				#endregion
			}
			{
				#region Coalescing
				Optional<int> a = 5;
				int b = a.Or(6);
				#endregion
			}
			{
				#region Coalescing_Optional
				var a = Imms.Optional.Some(5);
				var b = Imms.Optional.Some(6);
				Optional<int> c = a.Or(b);
				#endregion
			}
		}
	}
}