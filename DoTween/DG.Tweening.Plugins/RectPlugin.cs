using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;

namespace DG.Tweening.Plugins
{
	public class RectPlugin : ABSTweenPlugin<Rect, Rect, RectOptions>
	{
		public override void Reset(TweenerCore<Rect, Rect, RectOptions> t)
		{
		}

		public override void SetFrom(TweenerCore<Rect, Rect, RectOptions> t, bool isRelative)
		{
			Rect endValue = t.endValue;
			t.endValue = t.getter();
			t.startValue = endValue;
			if (isRelative)
			{
				t.startValue.x += t.endValue.x;
				t.startValue.y += t.endValue.y;
				t.startValue.width += t.endValue.width;
				t.startValue.height += t.endValue.height;
			}
			Rect startValue = t.startValue;
			if (t.plugOptions.snapping)
			{
				startValue.x = (float)Math.Round((double)startValue.x);
				startValue.y = (float)Math.Round((double)startValue.y);
				startValue.width = (float)Math.Round((double)startValue.width);
				startValue.height = (float)Math.Round((double)startValue.height);
			}
			t.setter(startValue);
		}

		public override Rect ConvertToStartValue(TweenerCore<Rect, Rect, RectOptions> t, Rect value)
		{
			return value;
		}

		public override void SetRelativeEndValue(TweenerCore<Rect, Rect, RectOptions> t)
		{
			t.endValue.x += t.startValue.x;
			t.endValue.y += t.startValue.y;
			t.endValue.width += t.startValue.width;
			t.endValue.height += t.startValue.height;
		}

		public override void SetChangeValue(TweenerCore<Rect, Rect, RectOptions> t)
		{
			t.changeValue = new Rect(t.endValue.x - t.startValue.x, t.endValue.y - t.startValue.y, t.endValue.width - t.startValue.width, t.endValue.height - t.startValue.height);
		}

		public override float GetSpeedBasedDuration(RectOptions options, float unitsXSecond, Rect changeValue)
		{
			float width = changeValue.width;
			float height = changeValue.height;
			return (float)Math.Sqrt((double)(width * width + height * height)) / unitsXSecond;
		}

		public override void EvaluateAndApply(RectOptions options, Tween t, bool isRelative, DOGetter<Rect> getter, DOSetter<Rect> setter, float elapsed, Rect startValue, Rect changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice)
		{
			if (t.loopType == LoopType.Incremental)
			{
				int num = t.isComplete ? (t.completedLoops - 1) : t.completedLoops;
				startValue.x += changeValue.x * (float)num;
				startValue.y += changeValue.y * (float)num;
				startValue.width += changeValue.width * (float)num;
				startValue.height += changeValue.height * (float)num;
			}
			if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
			{
				int num2 = ((t.loopType != LoopType.Incremental) ? 1 : t.loops) * (t.sequenceParent.isComplete ? (t.sequenceParent.completedLoops - 1) : t.sequenceParent.completedLoops);
				startValue.x += changeValue.x * (float)num2;
				startValue.y += changeValue.y * (float)num2;
				startValue.width += changeValue.width * (float)num2;
				startValue.height += changeValue.height * (float)num2;
			}
			float num3 = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
			startValue.x += changeValue.x * num3;
			startValue.y += changeValue.y * num3;
			startValue.width += changeValue.width * num3;
			startValue.height += changeValue.height * num3;
			if (options.snapping)
			{
				startValue.x = (float)Math.Round((double)startValue.x);
				startValue.y = (float)Math.Round((double)startValue.y);
				startValue.width = (float)Math.Round((double)startValue.width);
				startValue.height = (float)Math.Round((double)startValue.height);
			}
			setter(startValue);
		}
	}
}
