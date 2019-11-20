using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using System;
using System.Collections.Generic;

namespace DG.Tweening
{
	public sealed class Sequence : Tween
	{
		internal readonly List<Tween> sequencedTweens = new List<Tween>();

		private readonly List<ABSSequentiable> _sequencedObjs = new List<ABSSequentiable>();

		internal float lastTweenInsertTime;

		internal Sequence()
		{
			base.tweenType = TweenType.Sequence;
			this.Reset();
		}

		internal static Sequence DoPrepend(Sequence inSequence, Tween t)
		{
			if (t.loops == -1)
			{
				t.loops = 1;
			}
			float num = t.delay + t.duration * (float)t.loops;
			inSequence.duration += num;
			int count = inSequence._sequencedObjs.Count;
			for (int i = 0; i < count; i++)
			{
				ABSSequentiable aBSSequentiable = inSequence._sequencedObjs[i];
				aBSSequentiable.sequencedPosition += num;
				aBSSequentiable.sequencedEndPosition += num;
			}
			return Sequence.DoInsert(inSequence, t, 0f);
		}

		internal static Sequence DoInsert(Sequence inSequence, Tween t, float atPosition)
		{
			TweenManager.AddActiveTweenToSequence(t);
			atPosition += t.delay;
			inSequence.lastTweenInsertTime = atPosition;
			t.isSequenced = (t.creationLocked = true);
			t.sequenceParent = inSequence;
			if (t.loops == -1)
			{
				t.loops = 1;
			}
			float num = t.duration * (float)t.loops;
			t.autoKill = false;
			t.delay = (t.elapsedDelay = 0f);
			t.delayComplete = true;
			t.isSpeedBased = false;
			t.sequencedPosition = atPosition;
			t.sequencedEndPosition = atPosition + num;
			if (t.sequencedEndPosition > inSequence.duration)
			{
				inSequence.duration = t.sequencedEndPosition;
			}
			inSequence._sequencedObjs.Add(t);
			inSequence.sequencedTweens.Add(t);
			return inSequence;
		}

		internal static Sequence DoAppendInterval(Sequence inSequence, float interval)
		{
			inSequence.lastTweenInsertTime = inSequence.duration;
			inSequence.duration += interval;
			return inSequence;
		}

		internal static Sequence DoPrependInterval(Sequence inSequence, float interval)
		{
			inSequence.lastTweenInsertTime = 0f;
			inSequence.duration += interval;
			int count = inSequence._sequencedObjs.Count;
			for (int i = 0; i < count; i++)
			{
				ABSSequentiable aBSSequentiable = inSequence._sequencedObjs[i];
				aBSSequentiable.sequencedPosition += interval;
				aBSSequentiable.sequencedEndPosition += interval;
			}
			return inSequence;
		}

		internal static Sequence DoInsertCallback(Sequence inSequence, TweenCallback callback, float atPosition)
		{
			inSequence.lastTweenInsertTime = atPosition;
			SequenceCallback sequenceCallback = new SequenceCallback(atPosition, callback);
			sequenceCallback.sequencedPosition = (sequenceCallback.sequencedEndPosition = atPosition);
			inSequence._sequencedObjs.Add(sequenceCallback);
			if (inSequence.duration < atPosition)
			{
				inSequence.duration = atPosition;
			}
			return inSequence;
		}

		internal override void Reset()
		{
			base.Reset();
			this.sequencedTweens.Clear();
			this._sequencedObjs.Clear();
			this.lastTweenInsertTime = 0f;
		}

		internal override bool Validate()
		{
			int count = this.sequencedTweens.Count;
			for (int i = 0; i < count; i++)
			{
				if (!this.sequencedTweens[i].Validate())
				{
					return false;
				}
			}
			return true;
		}

		internal override bool Startup()
		{
			return Sequence.DoStartup(this);
		}

		internal override bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode, UpdateNotice updateNotice)
		{
			return Sequence.DoApplyTween(this, prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition, updateMode);
		}

		internal static void Setup(Sequence s)
		{
			s.autoKill = DOTween.defaultAutoKill;
			s.isRecyclable = DOTween.defaultRecyclable;
			s.isPlaying = (DOTween.defaultAutoPlay == AutoPlay.All || DOTween.defaultAutoPlay == AutoPlay.AutoPlaySequences);
			s.loopType = DOTween.defaultLoopType;
			s.easeType = Ease.Linear;
			s.easeOvershootOrAmplitude = DOTween.defaultEaseOvershootOrAmplitude;
			s.easePeriod = DOTween.defaultEasePeriod;
		}

		internal static bool DoStartup(Sequence s)
		{
			if (s.sequencedTweens.Count == 0 && s._sequencedObjs.Count == 0 && s.onComplete == null && s.onKill == null && s.onPause == null && s.onPlay == null && s.onRewind == null && s.onStart == null && s.onStepComplete == null && s.onUpdate == null)
			{
				return false;
			}
			s.startupDone = true;
			s.fullDuration = ((s.loops > -1) ? (s.duration * (float)s.loops) : float.PositiveInfinity);
			s._sequencedObjs.Sort(Sequence.SortSequencedObjs);
			if (s.isRelative)
			{
				int count = s.sequencedTweens.Count;
				for (int i = 0; i < count; i++)
				{
					Tween tween = s.sequencedTweens[i];
					if (!s.isBlendable)
					{
						s.sequencedTweens[i].isRelative = true;
					}
				}
			}
			return true;
		}

		internal static bool DoApplyTween(Sequence s, float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode)
		{
			float num = prevPosition;
			float num2 = s.position;
			if (s.easeType != Ease.Linear)
			{
				num = s.duration * EaseManager.Evaluate(s.easeType, s.customEase, num, s.duration, s.easeOvershootOrAmplitude, s.easePeriod);
				num2 = s.duration * EaseManager.Evaluate(s.easeType, s.customEase, num2, s.duration, s.easeOvershootOrAmplitude, s.easePeriod);
			}
			float num3 = 0f;
			bool flag = s.loopType == LoopType.Yoyo && ((num < s.duration) ? (prevCompletedLoops % 2 != 0) : (prevCompletedLoops % 2 == 0));
			if (s.isBackwards)
			{
				flag = !flag;
			}
			float num6;
			if (newCompletedSteps > 0)
			{
				int completedLoops = s.completedLoops;
				float position = s.position;
				int num4 = newCompletedSteps;
				int num5 = 0;
				num6 = num;
				if (updateMode == UpdateMode.Update)
				{
					while (num5 < num4)
					{
						if (num5 > 0)
						{
							num6 = num3;
						}
						else if (flag && !s.isBackwards)
						{
							num6 = s.duration - num6;
						}
						num3 = (flag ? 0f : s.duration);
						if (Sequence.ApplyInternalCycle(s, num6, num3, updateMode, useInversePosition, flag, true))
						{
							return true;
						}
						num5++;
						if (s.loopType == LoopType.Yoyo)
						{
							flag = !flag;
						}
					}
					if (completedLoops == s.completedLoops && !(Math.Abs(position - s.position) > 1.401298E-45f))
					{
						goto IL_0175;
					}
					return !s.active;
				}
				if (s.loopType == LoopType.Yoyo && newCompletedSteps % 2 != 0)
				{
					flag = !flag;
					num = s.duration - num;
				}
				newCompletedSteps = 0;
			}
			goto IL_0175;
			IL_0175:
			if (newCompletedSteps == 1 && s.isComplete)
			{
				return false;
			}
			if (newCompletedSteps > 0 && !s.isComplete)
			{
				num6 = (useInversePosition ? s.duration : 0f);
				if (s.loopType == LoopType.Restart && num3 > 0f)
				{
					Sequence.ApplyInternalCycle(s, s.duration, 0f, UpdateMode.Goto, false, false, false);
				}
			}
			else
			{
				num6 = (useInversePosition ? (s.duration - num) : num);
			}
			return Sequence.ApplyInternalCycle(s, num6, useInversePosition ? (s.duration - num2) : num2, updateMode, useInversePosition, flag, false);
		}

		private static bool ApplyInternalCycle(Sequence s, float fromPos, float toPos, UpdateMode updateMode, bool useInverse, bool prevPosIsInverse, bool multiCycleStep = false)
		{
			if (toPos < fromPos)
			{
				for (int num = s._sequencedObjs.Count - 1; num > -1; num--)
				{
					if (!s.active)
					{
						return true;
					}
					ABSSequentiable aBSSequentiable = s._sequencedObjs[num];
					if (!(aBSSequentiable.sequencedEndPosition < toPos) && !(aBSSequentiable.sequencedPosition > fromPos))
					{
						if (aBSSequentiable.tweenType == TweenType.Callback)
						{
							if (updateMode == UpdateMode.Update & prevPosIsInverse)
							{
								Tween.OnTweenCallback(aBSSequentiable.onStart);
							}
						}
						else
						{
							float num2 = toPos - aBSSequentiable.sequencedPosition;
							if (num2 < 0f)
							{
								num2 = 0f;
							}
							Tween tween = (Tween)aBSSequentiable;
							if (tween.startupDone)
							{
								tween.isBackwards = true;
								if (TweenManager.Goto(tween, num2, false, updateMode))
								{
									return true;
								}
								if (multiCycleStep && tween.tweenType == TweenType.Sequence)
								{
									if (s.position <= 0f && s.completedLoops == 0)
									{
										tween.position = 0f;
									}
									else
									{
										bool flag = s.completedLoops == 0 || (s.isBackwards && (s.completedLoops < s.loops || s.loops == -1));
										if (tween.isBackwards)
										{
											flag = !flag;
										}
										if (useInverse)
										{
											flag = !flag;
										}
										if (s.isBackwards && !useInverse && !prevPosIsInverse)
										{
											flag = !flag;
										}
										tween.position = (flag ? 0f : tween.duration);
									}
								}
							}
						}
					}
				}
			}
			else
			{
				int count = s._sequencedObjs.Count;
				for (int i = 0; i < count; i++)
				{
					if (!s.active)
					{
						return true;
					}
					ABSSequentiable aBSSequentiable2 = s._sequencedObjs[i];
					if (!(aBSSequentiable2.sequencedPosition > toPos) && !(aBSSequentiable2.sequencedEndPosition < fromPos))
					{
						if (aBSSequentiable2.tweenType == TweenType.Callback)
						{
							if (updateMode == UpdateMode.Update && ((!s.isBackwards && !useInverse && !prevPosIsInverse) || ((s.isBackwards & useInverse) && !prevPosIsInverse)))
							{
								Tween.OnTweenCallback(aBSSequentiable2.onStart);
							}
						}
						else
						{
							float num3 = toPos - aBSSequentiable2.sequencedPosition;
							if (num3 < 0f)
							{
								num3 = 0f;
							}
							Tween tween2 = (Tween)aBSSequentiable2;
							if (toPos >= aBSSequentiable2.sequencedEndPosition)
							{
								if (!tween2.startupDone)
								{
									TweenManager.ForceInit(tween2, true);
								}
								if (num3 < tween2.fullDuration)
								{
									num3 = tween2.fullDuration;
								}
							}
							tween2.isBackwards = false;
							if (TweenManager.Goto(tween2, num3, false, updateMode))
							{
								return true;
							}
							if (multiCycleStep && tween2.tweenType == TweenType.Sequence)
							{
								if (s.position <= 0f && s.completedLoops == 0)
								{
									tween2.position = 0f;
								}
								else
								{
									bool flag2 = s.completedLoops == 0 || (!s.isBackwards && (s.completedLoops < s.loops || s.loops == -1));
									if (tween2.isBackwards)
									{
										flag2 = !flag2;
									}
									if (useInverse)
									{
										flag2 = !flag2;
									}
									if (s.isBackwards && !useInverse && !prevPosIsInverse)
									{
										flag2 = !flag2;
									}
									tween2.position = (flag2 ? 0f : tween2.duration);
								}
							}
						}
					}
				}
			}
			return false;
		}

		private static int SortSequencedObjs(ABSSequentiable a, ABSSequentiable b)
		{
			if (a.sequencedPosition > b.sequencedPosition)
			{
				return 1;
			}
			if (a.sequencedPosition < b.sequencedPosition)
			{
				return -1;
			}
			return 0;
		}
	}
}
