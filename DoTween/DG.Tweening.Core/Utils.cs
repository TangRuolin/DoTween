using UnityEngine;

namespace DG.Tweening.Core
{
	internal static class Utils
	{
		internal static Vector3 Vector3FromAngle(float degrees, float magnitude)
		{
			float f = degrees * 0.0174532924f;
			return new Vector3(magnitude * Mathf.Cos(f), magnitude * Mathf.Sin(f), 0f);
		}

		internal static float Angle2D(Vector3 from, Vector3 to)
		{
			Vector2 right = Vector2.right;
			to -= from;
			float num = Vector2.Angle(right, to);
			if (Vector3.Cross(right, to).z > 0f)
			{
				num = 360f - num;
			}
			return num * -1f;
		}
	}
}
