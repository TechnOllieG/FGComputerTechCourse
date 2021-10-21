using Unity.Mathematics;
using UnityEngine;

namespace TechnOllieG
{
	public static class UtilityLibrary
	{
		private const float SmallNumber = 1E-08f;
		
		public static int2 With(this int2 a, int? x = null, int? y = null)
		{
			return new int2(x ?? a.x, y ?? a.y);
		}
		
		public static int2 WithAdd(this int2 a, int x = 0, int y = 0)
		{
			return new int2(a.x + x, a.y + y);
		}

		public static bool IsNearlyZero(this Vector3 v, float preciseness = SmallNumber)
		{
			return v.x < preciseness && v.x > -preciseness && v.y < preciseness && v.y > -preciseness && v.z < preciseness && v.z > -preciseness;
		}
	}
}