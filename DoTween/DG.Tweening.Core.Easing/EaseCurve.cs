using UnityEngine;

namespace DG.Tweening.Core.Easing
{
	public class EaseCurve
	{
		private readonly AnimationCurve _animCurve;

		public EaseCurve(AnimationCurve animCurve)
		{
			this._animCurve = animCurve;
		}

		public float Evaluate(float time, float duration, float unusedOvershoot, float unusedPeriod)
		{
			float time2 = this._animCurve[this._animCurve.length - 1].time;
			float num = time / duration;
			return this._animCurve.Evaluate(num * time2);
		}
	}
}
