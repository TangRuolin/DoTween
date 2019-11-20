using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening.Plugins
{
	public class QuaternionPlugin : ABSTweenPlugin<Quaternion, Vector3, QuaternionOptions>
	{
		public override void Reset(TweenerCore<Quaternion, Vector3, QuaternionOptions> t)
		{
		}

		public override void SetFrom(TweenerCore<Quaternion, Vector3, QuaternionOptions> t, bool isRelative)
		{
			Vector3 endValue = t.endValue;
			Quaternion quaternion = t.getter();
			t.endValue = quaternion.eulerAngles;
			if (t.plugOptions.rotateMode == RotateMode.Fast && !t.isRelative)
			{
				t.startValue = endValue;
			}
			else if (t.plugOptions.rotateMode == RotateMode.FastBeyond360)
			{
				t.startValue = t.endValue + endValue;
			}
			else
			{
				Quaternion quaternion2 = t.getter();
				if (t.plugOptions.rotateMode == RotateMode.WorldAxisAdd)
				{
					quaternion = quaternion2 * Quaternion.Inverse(quaternion2) * Quaternion.Euler(endValue) * quaternion2;
					t.startValue = quaternion.eulerAngles;
				}
				else
				{
					quaternion = quaternion2 * Quaternion.Euler(endValue);
					t.startValue = quaternion.eulerAngles;
				}
				t.endValue = -endValue;
			}
			t.setter(Quaternion.Euler(t.startValue));
		}

		public override Vector3 ConvertToStartValue(TweenerCore<Quaternion, Vector3, QuaternionOptions> t, Quaternion value)
		{
			return value.eulerAngles;
		}

		public override void SetRelativeEndValue(TweenerCore<Quaternion, Vector3, QuaternionOptions> t)
		{
			t.endValue += t.startValue;
		}

		public override void SetChangeValue(TweenerCore<Quaternion, Vector3, QuaternionOptions> t)
		{
			if (t.plugOptions.rotateMode == RotateMode.Fast && !t.isRelative)
			{
				Vector3 endValue = t.endValue;
				if (endValue.x > 360f)
				{
					endValue.x %= 360f;
				}
				if (endValue.y > 360f)
				{
					endValue.y %= 360f;
				}
				if (endValue.z > 360f)
				{
					endValue.z %= 360f;
				}
				Vector3 vector = endValue - t.startValue;
				float num = (vector.x > 0f) ? vector.x : (0f - vector.x);
				if (num > 180f)
				{
					vector.x = ((vector.x > 0f) ? (0f - (360f - num)) : (360f - num));
				}
				num = ((vector.y > 0f) ? vector.y : (0f - vector.y));
				if (num > 180f)
				{
					vector.y = ((vector.y > 0f) ? (0f - (360f - num)) : (360f - num));
				}
				num = ((vector.z > 0f) ? vector.z : (0f - vector.z));
				if (num > 180f)
				{
					vector.z = ((vector.z > 0f) ? (0f - (360f - num)) : (360f - num));
				}
				t.changeValue = vector;
			}
			else if (t.plugOptions.rotateMode == RotateMode.FastBeyond360 || t.isRelative)
			{
				t.changeValue = t.endValue - t.startValue;
			}
			else
			{
				t.changeValue = t.endValue;
			}
		}

		public override float GetSpeedBasedDuration(QuaternionOptions options, float unitsXSecond, Vector3 changeValue)
		{
			return changeValue.magnitude / unitsXSecond;
		}

		public override void EvaluateAndApply(QuaternionOptions options, Tween t, bool isRelative, DOGetter<Quaternion> getter, DOSetter<Quaternion> setter, float elapsed, Vector3 startValue, Vector3 changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice)
		{
			Vector3 vector = startValue;
			if (t.loopType == LoopType.Incremental)
			{
				vector += changeValue * (float)(t.isComplete ? (t.completedLoops - 1) : t.completedLoops);
			}
			if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
			{
				vector += changeValue * (float)((t.loopType != LoopType.Incremental) ? 1 : t.loops) * (float)(t.sequenceParent.isComplete ? (t.sequenceParent.completedLoops - 1) : t.sequenceParent.completedLoops);
			}
			float num = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
			RotateMode rotateMode = options.rotateMode;
			if (rotateMode == RotateMode.WorldAxisAdd || rotateMode == RotateMode.LocalAxisAdd)
			{
				Quaternion quaternion = Quaternion.Euler(startValue);
				vector.x = changeValue.x * num;
				vector.y = changeValue.y * num;
				vector.z = changeValue.z * num;
				if (options.rotateMode == RotateMode.WorldAxisAdd)
				{
					setter(quaternion * Quaternion.Inverse(quaternion) * Quaternion.Euler(vector) * quaternion);
				}
				else
				{
					setter(quaternion * Quaternion.Euler(vector));
				}
			}
			else
			{
				vector.x += changeValue.x * num;
				vector.y += changeValue.y * num;
				vector.z += changeValue.z * num;
				setter(Quaternion.Euler(vector));
			}
		}
	}
}
