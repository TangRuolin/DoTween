using DG.Tweening.Core.Easing;
using UnityEngine;

namespace DG.Tweening
{
	public class TweenParams
	{
		public static readonly TweenParams Params = new TweenParams();

		internal object id;

		internal object target;

		internal UpdateType updateType;

		internal bool isIndependentUpdate;

		internal TweenCallback onStart;

		internal TweenCallback onPlay;

		internal TweenCallback onRewind;

		internal TweenCallback onUpdate;

		internal TweenCallback onStepComplete;

		internal TweenCallback onComplete;

		internal TweenCallback onKill;

		internal TweenCallback<int> onWaypointChange;

		internal bool isRecyclable;

		internal bool isSpeedBased;

		internal bool autoKill;

		internal int loops;

		internal LoopType loopType;

		internal float delay;

		internal bool isRelative;

		internal Ease easeType;

		internal EaseFunction customEase;

		internal float easeOvershootOrAmplitude;

		internal float easePeriod;

		public TweenParams()
		{
			this.Clear();
		}

		public TweenParams Clear()
		{
			this.id = (this.target = null);
			this.updateType = DOTween.defaultUpdateType;
			this.isIndependentUpdate = DOTween.defaultTimeScaleIndependent;
			this.onStart = (this.onPlay = (this.onRewind = (this.onUpdate = (this.onStepComplete = (this.onComplete = (this.onKill = null))))));
			this.onWaypointChange = null;
			this.isRecyclable = DOTween.defaultRecyclable;
			this.isSpeedBased = false;
			this.autoKill = DOTween.defaultAutoKill;
			this.loops = 1;
			this.loopType = DOTween.defaultLoopType;
			this.delay = 0f;
			this.isRelative = false;
			this.easeType = Ease.Unset;
			this.customEase = null;
			this.easeOvershootOrAmplitude = DOTween.defaultEaseOvershootOrAmplitude;
			this.easePeriod = DOTween.defaultEasePeriod;
			return this;
		}

		public TweenParams SetAutoKill(bool autoKillOnCompletion = true)
		{
			this.autoKill = autoKillOnCompletion;
			return this;
		}

		public TweenParams SetId(object id)
		{
			this.id = id;
			return this;
		}

		public TweenParams SetTarget(object target)
		{
			this.target = target;
			return this;
		}

		public TweenParams SetLoops(int loops, LoopType? loopType = default(LoopType?))
		{
			if (loops < -1)
			{
				loops = -1;
			}
			else if (loops == 0)
			{
				loops = 1;
			}
			this.loops = loops;
			if (loopType.HasValue)
			{
				this.loopType = loopType.Value;
			}
			return this;
		}

		public TweenParams SetEase(Ease ease, float? overshootOrAmplitude = default(float?), float? period = default(float?))
		{
			this.easeType = ease;
			this.easeOvershootOrAmplitude = (overshootOrAmplitude.HasValue ? overshootOrAmplitude.Value : DOTween.defaultEaseOvershootOrAmplitude);
			this.easePeriod = (period.HasValue ? period.Value : DOTween.defaultEasePeriod);
			this.customEase = null;
			return this;
		}

		public TweenParams SetEase(AnimationCurve animCurve)
		{
			this.easeType = Ease.INTERNAL_Custom;
			this.customEase = new EaseCurve(animCurve).Evaluate;
			return this;
		}

		public TweenParams SetEase(EaseFunction customEase)
		{
			this.easeType = Ease.INTERNAL_Custom;
			this.customEase = customEase;
			return this;
		}

		public TweenParams SetRecyclable(bool recyclable = true)
		{
			this.isRecyclable = recyclable;
			return this;
		}

		public TweenParams SetUpdate(bool isIndependentUpdate)
		{
			this.updateType = DOTween.defaultUpdateType;
			this.isIndependentUpdate = isIndependentUpdate;
			return this;
		}

		public TweenParams SetUpdate(UpdateType updateType, bool isIndependentUpdate = false)
		{
			this.updateType = updateType;
			this.isIndependentUpdate = isIndependentUpdate;
			return this;
		}

		public TweenParams OnStart(TweenCallback action)
		{
			this.onStart = action;
			return this;
		}

		public TweenParams OnPlay(TweenCallback action)
		{
			this.onPlay = action;
			return this;
		}

		public TweenParams OnRewind(TweenCallback action)
		{
			this.onRewind = action;
			return this;
		}

		public TweenParams OnUpdate(TweenCallback action)
		{
			this.onUpdate = action;
			return this;
		}

		public TweenParams OnStepComplete(TweenCallback action)
		{
			this.onStepComplete = action;
			return this;
		}

		public TweenParams OnComplete(TweenCallback action)
		{
			this.onComplete = action;
			return this;
		}

		public TweenParams OnKill(TweenCallback action)
		{
			this.onKill = action;
			return this;
		}

		public TweenParams OnWaypointChange(TweenCallback<int> action)
		{
			this.onWaypointChange = action;
			return this;
		}

		public TweenParams SetDelay(float delay)
		{
			this.delay = delay;
			return this;
		}

		public TweenParams SetRelative(bool isRelative = true)
		{
			this.isRelative = isRelative;
			return this;
		}

		public TweenParams SetSpeedBased(bool isSpeedBased = true)
		{
			this.isSpeedBased = isSpeedBased;
			return this;
		}
	}
}
