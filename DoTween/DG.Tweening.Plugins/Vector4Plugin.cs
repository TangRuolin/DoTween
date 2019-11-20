using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;

namespace DG.Tweening.Plugins
{
	public class Vector4Plugin : ABSTweenPlugin<Vector4, Vector4, VectorOptions>
	{
		public override void Reset(TweenerCore<Vector4, Vector4, VectorOptions> t)
		{
		}

		public override void SetFrom(TweenerCore<Vector4, Vector4, VectorOptions> t, bool isRelative)
		{
			Vector4 endValue = t.endValue;
			t.endValue = t.getter();
			t.startValue = (isRelative ? (t.endValue + endValue) : endValue);
			Vector4 vector = t.endValue;
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
			case AxisConstraint.W:
				vector.w = t.startValue.w;
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
				vector.w = (float)Math.Round((double)vector.w);
			}
			t.setter(vector);
		}

		public override Vector4 ConvertToStartValue(TweenerCore<Vector4, Vector4, VectorOptions> t, Vector4 value)
		{
			return value;
		}

		public override void SetRelativeEndValue(TweenerCore<Vector4, Vector4, VectorOptions> t)
		{
			t.endValue += t.startValue;
		}

		public override void SetChangeValue(TweenerCore<Vector4, Vector4, VectorOptions> t)
		{
			switch (t.plugOptions.axisConstraint)
			{
			case AxisConstraint.X:
				t.changeValue = new Vector4(t.endValue.x - t.startValue.x, 0f, 0f, 0f);
				break;
			case AxisConstraint.Y:
				t.changeValue = new Vector4(0f, t.endValue.y - t.startValue.y, 0f, 0f);
				break;
			case AxisConstraint.Z:
				t.changeValue = new Vector4(0f, 0f, t.endValue.z - t.startValue.z, 0f);
				break;
			case AxisConstraint.W:
				t.changeValue = new Vector4(0f, 0f, 0f, t.endValue.w - t.startValue.w);
				break;
			default:
				t.changeValue = t.endValue - t.startValue;
				break;
			}
		}

		public override float GetSpeedBasedDuration(VectorOptions options, float unitsXSecond, Vector4 changeValue)
		{
			return changeValue.magnitude / unitsXSecond;
		}

		public override void EvaluateAndApply(VectorOptions options, Tween t, bool isRelative, DOGetter<Vector4> getter, DOSetter<Vector4> setter, float elapsed, Vector4 startValue, Vector4 changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice)
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
				Vector4 vector2 = getter();
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
				Vector4 vector4 = getter();
				vector4.y = startValue.y + changeValue.y * num;
				if (options.snapping)
				{
					vector4.y = (float)Math.Round((double)vector4.y);
				}
				setter(vector4);
				break;
			}
			case AxisConstraint.Z:
			{
				Vector4 vector = getter();
				vector.z = startValue.z + changeValue.z * num;
				if (options.snapping)
				{
					vector.z = (float)Math.Round((double)vector.z);
				}
				setter(vector);
				break;
			}
			case AxisConstraint.W:
			{
				Vector4 vector3 = getter();
				vector3.w = startValue.w + changeValue.w * num;
				if (options.snapping)
				{
					vector3.w = (float)Math.Round((double)vector3.w);
				}
				setter(vector3);
				break;
			}
			default:
				startValue.x += changeValue.x * num;
				startValue.y += changeValue.y * num;
				startValue.z += changeValue.z * num;
				startValue.w += changeValue.w * num;
				if (options.snapping)
				{
					startValue.x = (float)Math.Round((double)startValue.x);
					startValue.y = (float)Math.Round((double)startValue.y);
					startValue.z = (float)Math.Round((double)startValue.z);
					startValue.w = (float)Math.Round((double)startValue.w);
				}
				setter(startValue);
				break;
			}
		}
	}
}
