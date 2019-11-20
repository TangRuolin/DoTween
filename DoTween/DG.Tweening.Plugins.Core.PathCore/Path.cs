using System;
using UnityEngine;

namespace DG.Tweening.Plugins.Core.PathCore
{
	[Serializable]
	public class Path
	{
		private static CatmullRomDecoder _catmullRomDecoder;

		private static LinearDecoder _linearDecoder;

		public float[] wpLengths;

		[SerializeField]
		internal PathType type;

		[SerializeField]
		internal int subdivisionsXSegment;

		[SerializeField]
		internal int subdivisions;

		[SerializeField]
		internal Vector3[] wps;

		[SerializeField]
		internal ControlPoint[] controlPoints;

		[SerializeField]
		internal float length;

		[SerializeField]
		internal bool isFinalized;

		[SerializeField]
		internal float[] timesTable;

		[SerializeField]
		internal float[] lengthsTable;

		internal int linearWPIndex = -1;

		private Path _incrementalClone;

		private int _incrementalIndex;

		private ABSPathDecoder _decoder;

		private bool _changed;

		internal Vector3[] nonLinearDrawWps;

		internal Vector3 targetPosition;

		internal Vector3? lookAtPosition;

		internal Color gizmoColor = new Color(1f, 1f, 1f, 0.7f);

		public Path(PathType type, Vector3[] waypoints, int subdivisionsXSegment, Color? gizmoColor = default(Color?))
		{
			this.type = type;
			this.subdivisionsXSegment = subdivisionsXSegment;
			if (gizmoColor.HasValue)
			{
				this.gizmoColor = gizmoColor.Value;
			}
			this.AssignWaypoints(waypoints, true);
			this.AssignDecoder(type);
			if (DOTween.isUnityEditor)
			{
				DOTween.GizmosDelegates.Add(this.Draw);
			}
		}

		internal Path()
		{
		}

		internal void FinalizePath(bool isClosedPath, AxisConstraint lockPositionAxes, Vector3 currTargetVal)
		{
			if (lockPositionAxes != 0)
			{
				bool flag = (lockPositionAxes & AxisConstraint.X) == AxisConstraint.X;
				bool flag2 = (lockPositionAxes & AxisConstraint.Y) == AxisConstraint.Y;
				bool flag3 = (lockPositionAxes & AxisConstraint.Z) == AxisConstraint.Z;
				for (int i = 0; i < this.wps.Length; i++)
				{
					Vector3 vector = this.wps[i];
					this.wps[i] = new Vector3(flag ? currTargetVal.x : vector.x, flag2 ? currTargetVal.y : vector.y, flag3 ? currTargetVal.z : vector.z);
				}
			}
			this._decoder.FinalizePath(this, this.wps, isClosedPath);
			this.isFinalized = true;
		}

		internal Vector3 GetPoint(float perc, bool convertToConstantPerc = false)
		{
			if (convertToConstantPerc)
			{
				perc = this.ConvertToConstantPathPerc(perc);
			}
			return this._decoder.GetPoint(perc, this.wps, this, this.controlPoints);
		}

		internal float ConvertToConstantPathPerc(float perc)
		{
			if (this.type == PathType.Linear)
			{
				return perc;
			}
			if (perc > 0f && perc < 1f)
			{
				float num = this.length * perc;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				int num6 = this.lengthsTable.Length;
				int num7 = 0;
				while (num7 < num6)
				{
					if (!(this.lengthsTable[num7] > num))
					{
						num2 = this.timesTable[num7];
						num7++;
						continue;
					}
					num4 = this.timesTable[num7];
					num5 = this.lengthsTable[num7];
					if (num7 <= 0)
					{
						break;
					}
					num3 = this.lengthsTable[num7 - 1];
					break;
				}
				perc = num2 + (num - num3) / (num5 - num3) * (num4 - num2);
			}
			if (perc > 1f)
			{
				perc = 1f;
			}
			else if (perc < 0f)
			{
				perc = 0f;
			}
			return perc;
		}

		internal int GetWaypointIndexFromPerc(float perc, bool isMovingForward)
		{
			if (perc >= 1f)
			{
				return this.wps.Length - 1;
			}
			if (perc <= 0f)
			{
				return 0;
			}
			float num = this.length * perc;
			float num2 = 0f;
			int i = 0;
			for (int num3 = this.wpLengths.Length; i < num3; i++)
			{
				num2 += this.wpLengths[i];
				if (!(num2 < num))
				{
					if (num2 > num)
					{
						if (!isMovingForward)
						{
							return i;
						}
						return i - 1;
					}
					return i;
				}
			}
			return 0;
		}

		internal static Vector3[] GetDrawPoints(Path p, int drawSubdivisionsXSegment)
		{
			int num = p.wps.Length;
			if (p.type == PathType.Linear)
			{
				return p.wps;
			}
			int num2 = num * drawSubdivisionsXSegment;
			Vector3[] array = new Vector3[num2 + 1];
			for (int i = 0; i <= num2; i++)
			{
				float perc = (float)i / (float)num2;
				Vector3 vector = array[i] = p.GetPoint(perc, false);
			}
			return array;
		}

		internal static void RefreshNonLinearDrawWps(Path p)
		{
			int num = p.wps.Length * 10;
			if (p.nonLinearDrawWps == null || p.nonLinearDrawWps.Length != num + 1)
			{
				p.nonLinearDrawWps = new Vector3[num + 1];
			}
			for (int i = 0; i <= num; i++)
			{
				float perc = (float)i / (float)num;
				Vector3 point = p.GetPoint(perc, false);
				p.nonLinearDrawWps[i] = point;
			}
		}

		internal void Destroy()
		{
			if (DOTween.isUnityEditor)
			{
				DOTween.GizmosDelegates.Remove(this.Draw);
			}
			this.wps = null;
			this.wpLengths = (this.timesTable = (this.lengthsTable = null));
			this.nonLinearDrawWps = null;
			this.isFinalized = false;
		}

		internal Path CloneIncremental(int loopIncrement)
		{
			if (this._incrementalClone != null)
			{
				if (this._incrementalIndex == loopIncrement)
				{
					return this._incrementalClone;
				}
				this._incrementalClone.Destroy();
			}
			int num = this.wps.Length;
			Vector3 a = this.wps[num - 1] - this.wps[0];
			Vector3[] array = new Vector3[this.wps.Length];
			for (int i = 0; i < num; i++)
			{
				array[i] = this.wps[i] + a * (float)loopIncrement;
			}
			int num2 = this.controlPoints.Length;
			ControlPoint[] array2 = new ControlPoint[num2];
			for (int j = 0; j < num2; j++)
			{
				array2[j] = this.controlPoints[j] + a * (float)loopIncrement;
			}
			Vector3[] array3 = null;
			if (this.nonLinearDrawWps != null)
			{
				int num3 = this.nonLinearDrawWps.Length;
				array3 = new Vector3[num3];
				for (int k = 0; k < num3; k++)
				{
					array3[k] = this.nonLinearDrawWps[k] + a * (float)loopIncrement;
				}
			}
			this._incrementalClone = new Path();
			this._incrementalIndex = loopIncrement;
			this._incrementalClone.type = this.type;
			this._incrementalClone.subdivisionsXSegment = this.subdivisionsXSegment;
			this._incrementalClone.subdivisions = this.subdivisions;
			this._incrementalClone.wps = array;
			this._incrementalClone.controlPoints = array2;
			if (DOTween.isUnityEditor)
			{
				DOTween.GizmosDelegates.Add(this._incrementalClone.Draw);
			}
			this._incrementalClone.length = this.length;
			this._incrementalClone.wpLengths = this.wpLengths;
			this._incrementalClone.timesTable = this.timesTable;
			this._incrementalClone.lengthsTable = this.lengthsTable;
			this._incrementalClone._decoder = this._decoder;
			this._incrementalClone.nonLinearDrawWps = array3;
			this._incrementalClone.targetPosition = this.targetPosition;
			this._incrementalClone.lookAtPosition = this.lookAtPosition;
			this._incrementalClone.isFinalized = true;
			return this._incrementalClone;
		}

		internal void AssignWaypoints(Vector3[] newWps, bool cloneWps = false)
		{
			if (cloneWps)
			{
				int num = newWps.Length;
				this.wps = new Vector3[num];
				for (int i = 0; i < num; i++)
				{
					this.wps[i] = newWps[i];
				}
			}
			else
			{
				this.wps = newWps;
			}
		}

		internal void AssignDecoder(PathType pathType)
		{
			this.type = pathType;
			if (pathType == PathType.Linear)
			{
				if (Path._linearDecoder == null)
				{
					Path._linearDecoder = new LinearDecoder();
				}
				this._decoder = Path._linearDecoder;
			}
			else
			{
				if (Path._catmullRomDecoder == null)
				{
					Path._catmullRomDecoder = new CatmullRomDecoder();
				}
				this._decoder = Path._catmullRomDecoder;
			}
		}

		internal void Draw()
		{
			Path.Draw(this);
		}

		private static void Draw(Path p)
		{
			if (p.timesTable != null)
			{
				Color color = p.gizmoColor;
				color.a *= 0.5f;
				Gizmos.color = p.gizmoColor;
				int num = p.wps.Length;
				if (p._changed || (p.type != 0 && p.nonLinearDrawWps == null))
				{
					p._changed = false;
					if (p.type != 0)
					{
						Path.RefreshNonLinearDrawWps(p);
					}
				}
				if (p.type == PathType.Linear)
				{
					Vector3 to = p.wps[0];
					for (int i = 0; i < num; i++)
					{
						Vector3 vector = p.wps[i];
						Gizmos.DrawLine(vector, to);
						to = vector;
					}
				}
				else
				{
					Vector3 to = p.nonLinearDrawWps[0];
					int num2 = p.nonLinearDrawWps.Length;
					for (int j = 1; j < num2; j++)
					{
						Vector3 vector2 = p.nonLinearDrawWps[j];
						Gizmos.DrawLine(vector2, to);
						to = vector2;
					}
				}
				Gizmos.color = color;
				for (int k = 0; k < num; k++)
				{
					Gizmos.DrawSphere(p.wps[k], 0.075f);
				}
				if (p.lookAtPosition.HasValue)
				{
					Vector3 value = p.lookAtPosition.Value;
					Gizmos.DrawLine(p.targetPosition, value);
					Gizmos.DrawWireSphere(value, 0.075f);
				}
			}
		}
	}
}
