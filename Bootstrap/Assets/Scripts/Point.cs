using Unity.Entities;

public struct Point : IComponentData
{
	public float x;
	public float y;
	public float z;

	public float oldX;
	public float oldY;
	public float oldZ;

	public byte anchor;

	public int neighborCount;

	public void CopyFrom(Point other) {
		x = other.x;
		y = other.y;
		z = other.z;
		oldX = other.oldX;
		oldY = other.oldY;
		oldZ = other.oldZ;

		anchor = other.anchor;
		neighborCount = other.neighborCount;
	}
}
