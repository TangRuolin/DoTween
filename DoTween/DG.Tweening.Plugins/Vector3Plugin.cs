using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;

namespace DG.Tweening.Plugins
{
	public class Vector3Plugin : ABSTweenPlugin<Vector3, Vector3, VectorOptions>
	{
		public override void Reset(TweenerCore<Vector3, Vector3, VectorOptions> t)
		{
		}

		public override void SetFrom(TweenerCore<Vector3, Vector3, VectorOptions> t, bool isRelative)
		{
			Vector3 endValue = t.endValue;
			t.endValue = t.getter();
			t.startValue = (isRelative ? (t.endValue + endValue) : endValue);
			Vector3 vector = t.endValue;
			switch (t.plugOptions.axisConstraint)
			{
			case AxisConstraint.X:
				vector.x = t.startValue.x;
				break;
			case AxisConstraint.Y:
				vector.y = t.startValue.y;
				break;
			case AxisConstraint.Z:
				vector.z = t.startValue.z;
				break;
			default:
				vector = t.startValue;
				break;
			}
			if (t.plugOptions.snapping)
			{
				vector.x = (float)Math.Round((double)vector.x);
				vector.y = (float)Math.Round((double)vector.y);
				vector.z = (float)Math.Round((double)vector.z);
			}
			t.setter(vector);
		}

		public override Vector3 ConvertToStartValue(TweenerCore<Vector3, Vector3, VectorOptions> t, Vector3 value)
		{
			return value;
		}

		public override void SetRelativeEndValue(TweenerCore<Vector3, Vector3, VectorOptions> t)
		{
			t.endValue += t.startValue;
		}

		public override void SetChangeValue(TweenerCore<Vector3, Vector3, VectorOptions> t)
		{
			switch (t.plugOptions.axisConstraint)
			{
			case AxisConstraint.X:
				t.changeValue = new Vector3(t.endValue.x - t.startValue.x, 0f, 0f);
				break;
			case AxisConstraint.Y:
				t.changeValue = new Vector3(0f, t.endValue.y - t.startValue.y, 0f);
				break;
			case AxisConstraint.Z:
				t.changeValue = new Vector3(0f, 0f, t.endValue.z - t.startValue.z);
				break;
			default:
				t.changeValue = t.endValue - t.startValue;
				break;
			}
		}

		public override float GetSpeedBasedDuration(VectorOptions options, float unitsXSecond, Vector3 changeValue)
		{
			return changeValue.magnitude / unitsXSecond;
		}

		public override void EvaluateAndApply(VectorOptions options, Tween t, bool isRelative, DOGetter<Vector3> getter, DOSetter<Vector3> setter, float elapsed, Vector3 startValue, Vector3 changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice)
		{
			if (t.loopType == LoopType.Incremental)
			{
				startValue += changeValue * (float)(t.isComplete ? (t.completedLoops - 1) : t.completedLoops);
			}
			if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
			{
				startValue += changeValue * (float)((t.loopType != LoopType.Incremental) ? 1 : t.loops) * (float)(t.sequenceParent.isComplete ? (t.sequenceParent.completedLoops - 1) : t.sequenceParent.completedLoops);
			}
			float num = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
			switch (options.axisConstraint)
			{
			case AxisConstraint.X:
			{
				Vector3 vector2 = getter();
				vector2.x = startValue.x + changeValue.x * num;
				if (options.snapping)
				{
					vector2.x = (float)Math.Round((double)vector2.x);
				}
				setter(vector2);
				break;
			}
			case AxisConstraint.Y:
			{
				Vector3 vector = getter();
				vector.y = startValue.y + changeValue.y * num;
				if (options.snapping)
				{
					vector.y = (float)Math.Round((double)vector.y);
				}
				setter(vector);
				break;
			}
			case AxisConstraint.Z:
			{
				Vector3 vector3 = getter();
				vector3.z = startValue.z + changeValue.z * num;
				if (options.snapping)
				{
					vector3.z = (float)Math.Round((double)vector3.z);
				}
				setter(vector3);
				break;
			}
			default:
				startValue.x += changeValue.x * num;
				startValue.y += changeValue.y * num;
				startValue.z += changeValue.z * num;
				if (options.snapping)
				{
					startValue.x = (float)Math.Round((double)startValue.x);
					startValue.y = (float)Math.Round((double)startValue.y);
					startValue.z = (float)Math.Round((double)startValue.z);
				}
				setter(startValue);
				break;
			}
		}
	}
}
