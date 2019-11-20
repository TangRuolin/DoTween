using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using System;

namespace DG.Tweening
{
	public abstract class Tween : ABSSequentiable
	{
		public float timeScale;

		public bool isBackwards;

		public object id;

		public object target;

		internal UpdateType updateType;

		internal bool isIndependentUpdate;

		internal TweenCallback onPlay;

		internal TweenCallback onPause;

		internal TweenCallback onRewind;

		internal TweenCallback onUpdate;

		internal TweenCallback onStepComplete;

		internal TweenCallback onComplete;

		internal TweenCallback onKill;

		internal TweenCallback<int> onWaypointChange;

		internal bool isFrom;

		internal bool isBlendable;

		internal bool isRecyclable;

		internal bool isSpeedBased;

		internal bool autoKill;

		internal float duration;

		internal int loops;

		internal LoopType loopType;

		internal float delay;

		internal bool isRelative;

		internal Ease easeType;

		internal EaseFunction customEase;

		public float easeOvershootOrAmplitude;

		public float easePeriod;

		internal Type typeofT1;

		internal Type typeofT2;

		internal Type typeofTPlugOptions;

		internal bool active;

		internal bool isSequenced;

		internal Sequence sequenceParent;

		internal int activeId = -1;

		internal SpecialStartupMode specialStartupMode;

		internal bool creationLocked;

		internal bool startupDone;

		internal bool playedOnce;

		internal float position;

		internal float fullDuration;

		internal int completedLoops;

		internal bool isPlaying;

		internal bool isComplete;

		internal float elapsedDelay;

		internal bool delayComplete = true;

		internal int miscInt = -1;

		public float fullPosition
		{
			get
			{
				return this.Elapsed(true);
			}
			set
			{
				this.Goto(value, this.isPlaying);
			}
		}

		internal virtual void Reset()
		{
			this.timeScale = 1f;
			this.isBackwards = false;
			this.id = null;
			this.isIndependentUpdate = false;
			base.onStart = (this.onPlay = (this.onRewind = (this.onUpdate = (this.onComplete = (this.onStepComplete = (this.onKill = null))))));
			this.onWaypointChange = null;
			this.target = null;
			this.isFrom = false;
			this.isBlendable = false;
			this.isSpeedBased = false;
			this.duration = 0f;
			this.loops = 1;
			this.delay = 0f;
			this.isRelative = false;
			this.customEase = null;
			this.isSequenced = false;
			this.sequenceParent = null;
			this.specialStartupMode = SpecialStartupMode.None;
			this.creationLocked = (this.startupDone = (this.playedOnce = false));
			this.position = (this.fullDuration = (float)(this.completedLoops = 0));
			this.isPlaying = (this.isComplete = false);
			this.elapsedDelay = 0f;
			this.delayComplete = true;
			this.miscInt = -1;
		}

		internal abstract bool Validate();

		internal virtual float UpdateDelay(float elapsed)
		{
			return 0f;
		}

		internal abstract bool Startup();

		internal abstract bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode, UpdateNotice updateNotice);

		internal static bool DoGoto(Tween t, float toPosition, int toCompletedLoops, UpdateMode updateMode)
		{
			if (!t.startupDone && !t.Startup())
			{
				return true;
			}
			if (!t.playedOnce && updateMode == UpdateMode.Update)
			{
				t.playedOnce = true;
				if (t.onStart != null)
				{
					Tween.OnTweenCallback(t.onStart);
					if (!t.active)
					{
						return true;
					}
				}
				if (t.onPlay != null)
				{
					Tween.OnTweenCallback(t.onPlay);
					if (!t.active)
					{
						return true;
					}
				}
			}
			float prevPosition = t.position;
			int num = t.completedLoops;
			t.completedLoops = toCompletedLoops;
			bool flag = t.position <= 0f && num <= 0;
			bool flag2 = t.isComplete;
			if (t.loops != -1)
			{
				t.isComplete = (t.completedLoops == t.loops);
			}
			int num2 = 0;
			if (updateMode == UpdateMode.Update)
			{
				if (t.isBackwards)
				{
					num2 = ((t.completedLoops < num) ? (num - t.completedLoops) : ((toPosition <= 0f && !flag) ? 1 : 0));
					if (flag2)
					{
						num2--;
					}
				}
				else
				{
					num2 = ((t.completedLoops > num) ? (t.completedLoops - num) : 0);
				}
			}
			else if (t.tweenType == TweenType.Sequence)
			{
				num2 = num - toCompletedLoops;
				if (num2 < 0)
				{
					num2 = -num2;
				}
			}
			t.position = toPosition;
			if (t.position > t.duration)
			{
				t.position = t.duration;
			}
			else if (t.position <= 0f)
			{
				if (t.completedLoops > 0 || t.isComplete)
				{
					t.position = t.duration;
				}
				else
				{
					t.position = 0f;
				}
			}
			bool flag3 = t.isPlaying;
			if (t.isPlaying)
			{
				if (!t.isBackwards)
				{
					t.isPlaying = !t.isComplete;
				}
				else
				{
					t.isPlaying = (t.completedLoops != 0 || !(t.position <= 0f));
				}
			}
			bool useInversePosition = t.loopType == LoopType.Yoyo && ((t.position < t.duration) ? (t.completedLoops % 2 != 0) : (t.completedLoops % 2 == 0));
			if (!flag)
			{
				if (t.loopType == LoopType.Restart && t.completedLoops != num)
				{
					goto IL_0220;
				}
				if (t.position <= 0f && t.completedLoops <= 0)
				{
					goto IL_0220;
				}
			}
			int num3 = 0;
			goto IL_0221;
			IL_0220:
			num3 = 1;
			goto IL_0221;
			IL_0221:
			UpdateNotice updateNotice = (UpdateNotice)num3;
			if (t.ApplyTween(prevPosition, num, num2, useInversePosition, updateMode, updateNotice))
			{
				return true;
			}
			if (t.onUpdate != null && updateMode != UpdateMode.IgnoreOnUpdate)
			{
				Tween.OnTweenCallback(t.onUpdate);
			}
			if (t.position <= 0f && t.completedLoops <= 0 && !flag && t.onRewind != null)
			{
				Tween.OnTweenCallback(t.onRewind);
			}
			if (num2 > 0 && updateMode == UpdateMode.Update && t.onStepComplete != null)
			{
				for (int i = 0; i < num2; i++)
				{
					Tween.OnTweenCallback(t.onStepComplete);
				}
			}
			if (t.isComplete && !flag2 && t.onComplete != null)
			{
				Tween.OnTweenCallback(t.onComplete);
			}
			if ((!t.isPlaying & flag3) && (!t.isComplete || !t.autoKill) && t.onPause != null)
			{
				Tween.OnTweenCallback(t.onPause);
			}
			if (t.autoKill)
			{
				return t.isComplete;
			}
			return false;
		}

		internal static bool OnTweenCallback(TweenCallback callback)
		{
			if (DOTween.useSafeMode)
			{
				try
				{
					callback();
				}
				catch (Exception ex)
				{
					Debugger.LogWarning("An error inside a tween callback was silently taken care of > " + ex.Message + "\n\n" + ex.StackTrace + "\n\n");
					return false;
				}
			}
			else
			{
				callback();
			}
			return true;
		}

		internal static bool OnTweenCallback<T>(TweenCallback<T> callback, T param)
		{
			if (DOTween.useSafeMode)
			{
				try
				{
					callback(param);
				}
				catch (Exception ex)
				{
					Debugger.LogWarning("An error inside a tween callback was silently taken care of > " + ex.Message);
					return false;
				}
			}
			else
			{
				callback(param);
			}
			return true;
		}
	}
}
