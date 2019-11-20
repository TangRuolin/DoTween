using DG.Tweening.Core.Easing;
using UnityEngine;

namespace DG.Tweening
{
	public class EaseFactory
	{
		public static EaseFunction StopMotion(int motionFps, Ease? ease = default(Ease?))
		{
			EaseFunction customEase = EaseManager.ToEaseFunction((!ease.HasValue) ? DOTween.defaultEaseType : ease.Value);
			return EaseFactory.StopMotion(motionFps, customEase);
		}

		public static EaseFunction StopMotion(int motionFps, AnimationCurve animCurve)
		{
			return EaseFactory.StopMotion(motionFps, new EaseCurve(animCurve).Evaluate);
		}

		public static EaseFunction StopMotion(int motionFps, EaseFunction customEase)
		{
			float motionDelay = 1f / (float)motionFps;
			return delegate(float time, float duration, float overshootOrAmplitude, float period)
			{
				float time2 = (time < duration) ? (time - time % motionDelay) : time;
				return customEase(time2, duration, overshootOrAmplitude, period);
			};
		}
	}
}
