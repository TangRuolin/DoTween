using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using System;
using UnityEngine;

namespace DG.Tweening.Plugins
{
	public class SpiralPlugin : ABSTweenPlugin<Vector3, Vector3, SpiralOptions>
	{
		public static readonly Vector3 DefaultDirection = Vector3.forward;

		public override void Reset(TweenerCore<Vector3, Vector3, SpiralOptions> t)
		{
		}

		public override void SetFrom(TweenerCore<Vector3, Vector3, SpiralOptions> t, bool isRelative)
		{
		}

		public static ABSTweenPlugin<Vector3, Vector3, SpiralOptions> Get()
		{
			return PluginsManager.GetCustomPlugin<SpiralPlugin, Vector3, Vector3, SpiralOptions>();
		}

		public override Vector3 ConvertToStartValue(TweenerCore<Vector3, Vector3, SpiralOptions> t, Vector3 value)
		{
			return value;
		}

		public override void SetRelativeEndValue(TweenerCore<Vector3, Vector3, SpiralOptions> t)
		{
		}

		public override void SetChangeValue(TweenerCore<Vector3, Vector3, SpiralOptions> t)
		{
			t.plugOptions.speed *= 10f / t.plugOptions.frequency;
			t.plugOptions.axisQ = Quaternion.LookRotation(t.endValue, Vector3.up);
		}

		public override float GetSpeedBasedDuration(SpiralOptions options, float unitsXSecond, Vector3 changeValue)
		{
			return unitsXSecond;
		}

		public override void EvaluateAndApply(SpiralOptions options, Tween t, bool isRelative, DOGetter<Vector3> getter, DOSetter<Vector3> setter, float elapsed, Vector3 startValue, Vector3 changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice)
		{
			float num = EaseManager.Evaluate(t, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
			float num2 = (options.mode == SpiralMode.ExpandThenContract && num > 0.5f) ? (0.5f - (num - 0.5f)) : num;
			if (t.loopType == LoopType.Incremental)
			{
				num += (float)(t.isComplete ? (t.completedLoops - 1) : t.completedLoops);
			}
			float num3 = duration * options.speed * num;
			options.unit = duration * options.speed * num2;
			Vector3 vector = new Vector3(options.unit * Mathf.Cos(num3 * options.frequency), options.unit * Mathf.Sin(num3 * options.frequency), options.depth * num);
			vector = options.axisQ * vector + startValue;
			if (options.snapping)
			{
				vector.x = (float)Math.Round((double)vector.x);
				vector.y = (float)Math.Round((double)vector.y);
				vector.z = (float)Math.Round((double)vector.z);
			}
			setter(vector);
		}
	}
}
