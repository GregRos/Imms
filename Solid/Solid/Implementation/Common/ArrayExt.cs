using System;
using System.Runtime.CompilerServices;

namespace Solid.Common
{
	internal static class ArrayExt
	{
		//Most of the usages of these methods have been inlined in the hope of squeezing a little more performance.
		//It didn't seem to help (most likely the JITter inlined them anyway).

		public static T[] Set<T>(this T[] self, int index, T value)
		{
			var myCopy = new T[self.Length];
			self.CopyTo(myCopy, 0);
			myCopy[index] = value;
			return myCopy;
		}
	
		public static T[] Add<T>(this T[] self, T value)
		{
			var myCopy = new T[self.Length + 1];
			self.CopyTo(myCopy, 0);
			myCopy[myCopy.Length - 1] = value;
			return myCopy;
		}

		public static T[] Insert<T>(this T[] self, int index, T value)
		{
			var myCopy = new T[self.Length + 1];
			Array.Copy(self,0,myCopy,0,index);
			myCopy[index] = value;
			Array.Copy(self,index,myCopy,index+1,self.Length - index);
			return myCopy;
		}


		public static T[] RemoveAt<T>(this T[] self, int index)
		{
			var myCopy = new T[self.Length];
			int i = 0;
			if (index >= self.Length) throw new Exception();
			for (; i < index; i++)
			{
				myCopy[i] = self[i];
			}
			i++;
			for (; i < self.Length; i++)
			{
				myCopy[i] = self[i];
			}
			return myCopy;
		}

		public static T[] Remove<T>(this T[] self)
		{
			var myCopy = new T[self.Length - 1];
			for (int i = 0; i < self.Length - 1; i++)
			{
				myCopy[i] = self[i];
			}
			return myCopy;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] TakeFirst<T>(this T[] self, int count)
		{
			var myCopy = new T[count];
			Array.Copy(self, 0, myCopy, 0, count);
			return myCopy;
		}
	}
}