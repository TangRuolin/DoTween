using UnityEngine;

namespace DG.Tweening.Plugins.Options
{
	public struct PathOptions
	{
		public PathMode mode;

		public OrientType orientType;

		public AxisConstraint lockPositionAxis;

		public AxisConstraint lockRotationAxis;

		public bool isClosedPath;

		public Vector3 lookAtPosition;

		public Transform lookAtTransform;

		public float lookAhead;

		public bool hasCustomForwardDirection;

		public Quaternion forward;

		public bool useLocalPosition;

		public Transform parent;

		internal Quaternion startupRot;

		internal float startupZRot;
	}
}
