using UnityEngine;

namespace DG.Tweening.Plugins
{
	public struct SpiralOptions
	{
		public float depth;

		public float frequency;

		public float speed;

		public SpiralMode mode;

		public bool snapping;

		internal float unit;

		internal Quaternion axisQ;
	}
}
