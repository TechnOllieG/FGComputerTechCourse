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
		
		public static float3 With(this float3 a, float? x = null, float? y = null, float? z = null)
		{
			return new float3(x ?? a.x, y ?? a.y, z ?? a.z);
		}
		
		public static int2 WithAdd(this int2 a, int x = 0, int y = 0)
		{
			return new int2(a.x + x, a.y + y);
		}
		
		public static float3 WithAdd(this float3 a, float x = 0, float y = 0, float z = 0)
		{
			return new float3(a.x + x, a.y + y, a.z + z);
		}

		public static bool IsNearlyZero(this Vector3 v, float preciseness = SmallNumber)
		{
			return v.x < preciseness && v.x > -preciseness && v.y < preciseness && v.y > -preciseness && v.z < preciseness && v.z > -preciseness;
		}
	}
}