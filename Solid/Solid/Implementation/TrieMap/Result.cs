namespace Solid.TrieMap
{
	internal enum Result
	{
		Unassigned = 0,
		Success = 1,
		KeyNotFound = 2,
		KeyExists = 3,
		HashCollision = 4,
		TurnedEmpty = 5
	}
}