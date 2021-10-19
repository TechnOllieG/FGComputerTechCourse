using Unity.Mathematics;

namespace TechnOllieG
{
	public static class UtilityLibrary
	{
		public static int2 With(this int2 a, int? x = null, int? y = null)
		{
			return new int2(x ?? a.x, y ?? a.y);
		}
		
		public static int2 WithAdd(this int2 a, int x = 0, int y = 0)
		{
			return new int2(a.x + x, a.y + y);
		}
	}
}