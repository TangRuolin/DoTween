using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening
{
	public static class TweenSettingsExtensions
	{
		public static T SetAutoKill<T>(this T t) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				((Tween)(object)t).autoKill = true;
				return t;
			}
			return t;
		}

		public static T SetAutoKill<T>(this T t, bool autoKillOnCompletion) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				((Tween)(object)t).autoKill = autoKillOnCompletion;
				return t;
			}
			return t;
		}

		public static T SetId<T>(this T t, object id) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).id = id;
				return t;
			}
			return t;
		}

		public static T SetTarget<T>(this T t, object target) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).target = target;
				return t;
			}
			return t;
		}

		public static T SetLoops<T>(this T t, int loops) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				if (loops < -1)
				{
					loops = -1;
				}
				else if (loops == 0)
				{
					loops = 1;
				}
				((Tween)(object)t).loops = loops;
				if (((ABSSequentiable)(object)t).tweenType == TweenType.Tweener)
				{
					if (loops > -1)
					{
						((Tween)(object)t).fullDuration = ((Tween)(object)t).duration * (float)loops;
					}
					else
					{
						((Tween)(object)t).fullDuration = float.PositiveInfinity;
					}
				}
				return t;
			}
			return t;
		}

		public static T SetLoops<T>(this T t, int loops, LoopType loopType) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				if (loops < -1)
				{
					loops = -1;
				}
				else if (loops == 0)
				{
					loops = 1;
				}
				((Tween)(object)t).loops = loops;
				((Tween)(object)t).loopType = loopType;
				if (((ABSSequentiable)(object)t).tweenType == TweenType.Tweener)
				{
					if (loops > -1)
					{
						((Tween)(object)t).fullDuration = ((Tween)(object)t).duration * (float)loops;
					}
					else
					{
						((Tween)(object)t).fullDuration = float.PositiveInfinity;
					}
				}
				return t;
			}
			return t;
		}

		public static T SetEase<T>(this T t, Ease ease) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).easeType = ease;
				if (EaseManager.IsFlashEase(ease))
				{
					((Tween)(object)t).easeOvershootOrAmplitude = (float)(int)((Tween)(object)t).easeOvershootOrAmplitude;
				}
				((Tween)(object)t).customEase = null;
				return t;
			}
			return t;
		}

		public static T SetEase<T>(this T t, Ease ease, float overshoot) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).easeType = ease;
				if (EaseManager.IsFlashEase(ease))
				{
					overshoot = (float)(int)overshoot;
				}
				((Tween)(object)t).easeOvershootOrAmplitude = overshoot;
				((Tween)(object)t).customEase = null;
				return t;
			}
			return t;
		}

		public static T SetEase<T>(this T t, Ease ease, float amplitude, float period) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).easeType = ease;
				if (EaseManager.IsFlashEase(ease))
				{
					amplitude = (float)(int)amplitude;
				}
				((Tween)(object)t).easeOvershootOrAmplitude = amplitude;
				((Tween)(object)t).easePeriod = period;
				((Tween)(object)t).customEase = null;
				return t;
			}
			return t;
		}

		public static T SetEase<T>(this T t, AnimationCurve animCurve) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).easeType = Ease.INTERNAL_Custom;
				((Tween)(object)t).customEase = new EaseCurve(animCurve).Evaluate;
				return t;
			}
			return t;
		}

		public static T SetEase<T>(this T t, EaseFunction customEase) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).easeType = Ease.INTERNAL_Custom;
				((Tween)(object)t).customEase = customEase;
				return t;
			}
			return t;
		}

		public static T SetRecyclable<T>(this T t) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).isRecyclable = true;
				return t;
			}
			return t;
		}

		public static T SetRecyclable<T>(this T t, bool recyclable) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).isRecyclable = recyclable;
				return t;
			}
			return t;
		}

		public static T SetUpdate<T>(this T t, bool isIndependentUpdate) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				TweenManager.SetUpdateType((Tween)(object)t, DOTween.defaultUpdateType, isIndependentUpdate);
				return t;
			}
			return t;
		}

		public static T SetUpdate<T>(this T t, UpdateType updateType) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				TweenManager.SetUpdateType((Tween)(object)t, updateType, DOTween.defaultTimeScaleIndependent);
				return t;
			}
			return t;
		}

		public static T SetUpdate<T>(this T t, UpdateType updateType, bool isIndependentUpdate) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				TweenManager.SetUpdateType((Tween)(object)t, updateType, isIndependentUpdate);
				return t;
			}
			return t;
		}

		public static T OnStart<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((ABSSequentiable)(object)t).onStart = action;
				return t;
			}
			return t;
		}

		public static T OnPlay<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onPlay = action;
				return t;
			}
			return t;
		}

		public static T OnPause<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onPause = action;
				return t;
			}
			return t;
		}

		public static T OnRewind<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onRewind = action;
				return t;
			}
			return t;
		}

		public static T OnUpdate<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onUpdate = action;
				return t;
			}
			return t;
		}

		public static T OnStepComplete<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onStepComplete = action;
				return t;
			}
			return t;
		}

		public static T OnComplete<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onComplete = action;
				return t;
			}
			return t;
		}

		public static T OnKill<T>(this T t, TweenCallback action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onKill = action;
				return t;
			}
			return t;
		}

		public static T OnWaypointChange<T>(this T t, TweenCallback<int> action) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active)
			{
				((Tween)(object)t).onWaypointChange = action;
				return t;
			}
			return t;
		}

		public static T SetAs<T>(this T t, Tween asTween) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				((Tween)(object)t).timeScale = asTween.timeScale;
				((Tween)(object)t).isBackwards = asTween.isBackwards;
				TweenManager.SetUpdateType((Tween)(object)t, asTween.updateType, asTween.isIndependentUpdate);
				((Tween)(object)t).id = asTween.id;
				((ABSSequentiable)(object)t).onStart = asTween.onStart;
				((Tween)(object)t).onPlay = asTween.onPlay;
				((Tween)(object)t).onRewind = asTween.onRewind;
				((Tween)(object)t).onUpdate = asTween.onUpdate;
				((Tween)(object)t).onStepComplete = asTween.onStepComplete;
				((Tween)(object)t).onComplete = asTween.onComplete;
				((Tween)(object)t).onKill = asTween.onKill;
				((Tween)(object)t).onWaypointChange = asTween.onWaypointChange;
				((Tween)(object)t).isRecyclable = asTween.isRecyclable;
				((Tween)(object)t).isSpeedBased = asTween.isSpeedBased;
				((Tween)(object)t).autoKill = asTween.autoKill;
				((Tween)(object)t).loops = asTween.loops;
				((Tween)(object)t).loopType = asTween.loopType;
				if (((ABSSequentiable)(object)t).tweenType == TweenType.Tweener)
				{
					if (((Tween)(object)t).loops > -1)
					{
						((Tween)(object)t).fullDuration = ((Tween)(object)t).duration * (float)((Tween)(object)t).loops;
					}
					else
					{
						((Tween)(object)t).fullDuration = float.PositiveInfinity;
					}
				}
				((Tween)(object)t).delay = asTween.delay;
				((Tween)(object)t).delayComplete = (((Tween)(object)t).delay <= 0f);
				((Tween)(object)t).isRelative = asTween.isRelative;
				((Tween)(object)t).easeType = asTween.easeType;
				((Tween)(object)t).customEase = asTween.customEase;
				((Tween)(object)t).easeOvershootOrAmplitude = asTween.easeOvershootOrAmplitude;
				((Tween)(object)t).easePeriod = asTween.easePeriod;
				return t;
			}
			return t;
		}

		public static T SetAs<T>(this T t, TweenParams tweenParams) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				TweenManager.SetUpdateType((Tween)(object)t, tweenParams.updateType, tweenParams.isIndependentUpdate);
				((Tween)(object)t).id = tweenParams.id;
				((ABSSequentiable)(object)t).onStart = tweenParams.onStart;
				((Tween)(object)t).onPlay = tweenParams.onPlay;
				((Tween)(object)t).onRewind = tweenParams.onRewind;
				((Tween)(object)t).onUpdate = tweenParams.onUpdate;
				((Tween)(object)t).onStepComplete = tweenParams.onStepComplete;
				((Tween)(object)t).onComplete = tweenParams.onComplete;
				((Tween)(object)t).onKill = tweenParams.onKill;
				((Tween)(object)t).onWaypointChange = tweenParams.onWaypointChange;
				((Tween)(object)t).isRecyclable = tweenParams.isRecyclable;
				((Tween)(object)t).isSpeedBased = tweenParams.isSpeedBased;
				((Tween)(object)t).autoKill = tweenParams.autoKill;
				((Tween)(object)t).loops = tweenParams.loops;
				((Tween)(object)t).loopType = tweenParams.loopType;
				if (((ABSSequentiable)(object)t).tweenType == TweenType.Tweener)
				{
					if (((Tween)(object)t).loops > -1)
					{
						((Tween)(object)t).fullDuration = ((Tween)(object)t).duration * (float)((Tween)(object)t).loops;
					}
					else
					{
						((Tween)(object)t).fullDuration = float.PositiveInfinity;
					}
				}
				((Tween)(object)t).delay = tweenParams.delay;
				((Tween)(object)t).delayComplete = (((Tween)(object)t).delay <= 0f);
				((Tween)(object)t).isRelative = tweenParams.isRelative;
				if (tweenParams.easeType == Ease.Unset)
				{
					if (((ABSSequentiable)(object)t).tweenType == TweenType.Sequence)
					{
						((Tween)(object)t).easeType = Ease.Linear;
					}
					else
					{
						((Tween)(object)t).easeType = DOTween.defaultEaseType;
					}
				}
				else
				{
					((Tween)(object)t).easeType = tweenParams.easeType;
				}
				((Tween)(object)t).customEase = tweenParams.customEase;
				((Tween)(object)t).easeOvershootOrAmplitude = tweenParams.easeOvershootOrAmplitude;
				((Tween)(object)t).easePeriod = tweenParams.easePeriod;
				return t;
			}
			return t;
		}

		public static Sequence Append(this Sequence s, Tween t)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				if (t != null && t.active && !t.isSequenced)
				{
					Sequence.DoInsert(s, t, s.duration);
					return s;
				}
				return s;
			}
			return s;
		}

		public static Sequence Prepend(this Sequence s, Tween t)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				if (t != null && t.active && !t.isSequenced)
				{
					Sequence.DoPrepend(s, t);
					return s;
				}
				return s;
			}
			return s;
		}

		public static Sequence Join(this Sequence s, Tween t)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				if (t != null && t.active && !t.isSequenced)
				{
					Sequence.DoInsert(s, t, s.lastTweenInsertTime);
					return s;
				}
				return s;
			}
			return s;
		}

		public static Sequence Insert(this Sequence s, float atPosition, Tween t)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				if (t != null && t.active && !t.isSequenced)
				{
					Sequence.DoInsert(s, t, atPosition);
					return s;
				}
				return s;
			}
			return s;
		}

		public static Sequence AppendInterval(this Sequence s, float interval)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				Sequence.DoAppendInterval(s, interval);
				return s;
			}
			return s;
		}

		public static Sequence PrependInterval(this Sequence s, float interval)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				Sequence.DoPrependInterval(s, interval);
				return s;
			}
			return s;
		}

		public static Sequence AppendCallback(this Sequence s, TweenCallback callback)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				if (callback == null)
				{
					return s;
				}
				Sequence.DoInsertCallback(s, callback, s.duration);
				return s;
			}
			return s;
		}

		public static Sequence PrependCallback(this Sequence s, TweenCallback callback)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				if (callback == null)
				{
					return s;
				}
				Sequence.DoInsertCallback(s, callback, 0f);
				return s;
			}
			return s;
		}

		public static Sequence InsertCallback(this Sequence s, float atPosition, TweenCallback callback)
		{
			if (s != null && s.active && !s.creationLocked)
			{
				if (callback == null)
				{
					return s;
				}
				Sequence.DoInsertCallback(s, callback, atPosition);
				return s;
			}
			return s;
		}

		public static T From<T>(this T t) where T : Tweener
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked && ((Tweener)(object)t).isFromAllowed)
			{
				((Tween)(object)t).isFrom = true;
				t.SetFrom(false);
				return t;
			}
			return t;
		}

		public static T From<T>(this T t, bool isRelative) where T : Tweener
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked && ((Tweener)(object)t).isFromAllowed)
			{
				((Tween)(object)t).isFrom = true;
				if (!isRelative)
				{
					t.SetFrom(false);
				}
				else
				{
					t.SetFrom(!((Tween)(object)t).isBlendable);
				}
				return t;
			}
			return t;
		}

		public static T SetDelay<T>(this T t, float delay) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				if (((ABSSequentiable)(object)t).tweenType == TweenType.Sequence)
				{
					(((object)t) as Sequence).PrependInterval(delay);
				}
				else
				{
					((Tween)(object)t).delay = delay;
					((Tween)(object)t).delayComplete = (delay <= 0f);
				}
				return t;
			}
			return t;
		}

		public static T SetRelative<T>(this T t) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked && !((Tween)(object)t).isFrom && !((Tween)(object)t).isBlendable)
			{
				((Tween)(object)t).isRelative = true;
				return t;
			}
			return t;
		}

		public static T SetRelative<T>(this T t, bool isRelative) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked && !((Tween)(object)t).isFrom && !((Tween)(object)t).isBlendable)
			{
				((Tween)(object)t).isRelative = isRelative;
				return t;
			}
			return t;
		}

		public static T SetSpeedBased<T>(this T t) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				((Tween)(object)t).isSpeedBased = true;
				return t;
			}
			return t;
		}

		public static T SetSpeedBased<T>(this T t, bool isSpeedBased) where T : Tween
		{
			if (t != null && ((Tween)(object)t).active && !((Tween)(object)t).creationLocked)
			{
				((Tween)(object)t).isSpeedBased = isSpeedBased;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<float, float, FloatOptions> t, bool snapping)
		{
			if (t != null && t.active)
			{
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector2, Vector2, VectorOptions> t, bool snapping)
		{
			if (t != null && t.active)
			{
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector2, Vector2, VectorOptions> t, AxisConstraint axisConstraint, bool snapping = false)
		{
			if (t != null && t.active)
			{
				t.plugOptions.axisConstraint = axisConstraint;
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector3, Vector3, VectorOptions> t, bool snapping)
		{
			if (t != null && t.active)
			{
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector3, Vector3, VectorOptions> t, AxisConstraint axisConstraint, bool snapping = false)
		{
			if (t != null && t.active)
			{
				t.plugOptions.axisConstraint = axisConstraint;
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector4, Vector4, VectorOptions> t, bool snapping)
		{
			if (t != null && t.active)
			{
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector4, Vector4, VectorOptions> t, AxisConstraint axisConstraint, bool snapping = false)
		{
			if (t != null && t.active)
			{
				t.plugOptions.axisConstraint = axisConstraint;
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Quaternion, Vector3, QuaternionOptions> t, bool useShortest360Route = true)
		{
			if (t != null && t.active)
			{
				t.plugOptions.rotateMode = (RotateMode)((!useShortest360Route) ? 1 : 0);
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Color, Color, ColorOptions> t, bool alphaOnly)
		{
			if (t != null && t.active)
			{
				t.plugOptions.alphaOnly = alphaOnly;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Rect, Rect, RectOptions> t, bool snapping)
		{
			if (t != null && t.active)
			{
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<string, string, StringOptions> t, bool richTextEnabled, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
		{
			if (t != null && t.active)
			{
				t.plugOptions.richTextEnabled = richTextEnabled;
				t.plugOptions.scrambleMode = scrambleMode;
				if (!string.IsNullOrEmpty(scrambleChars))
				{
					if (scrambleChars.Length <= 1)
					{
						scrambleChars += scrambleChars;
					}
					t.plugOptions.scrambledChars = scrambleChars.ToCharArray();
					t.plugOptions.scrambledChars.ScrambleChars();
				}
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector3, Vector3[], Vector3ArrayOptions> t, bool snapping)
		{
			if (t != null && t.active)
			{
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static Tweener SetOptions(this TweenerCore<Vector3, Vector3[], Vector3ArrayOptions> t, AxisConstraint axisConstraint, bool snapping = false)
		{
			if (t != null && t.active)
			{
				t.plugOptions.axisConstraint = axisConstraint;
				t.plugOptions.snapping = snapping;
				return t;
			}
			return t;
		}

		public static TweenerCore<Vector3, Path, PathOptions> SetOptions(this TweenerCore<Vector3, Path, PathOptions> t, AxisConstraint lockPosition, AxisConstraint lockRotation = AxisConstraint.None)
		{
			return t.SetOptions(false, lockPosition, lockRotation);
		}

		public static TweenerCore<Vector3, Path, PathOptions> SetOptions(this TweenerCore<Vector3, Path, PathOptions> t, bool closePath, AxisConstraint lockPosition = AxisConstraint.None, AxisConstraint lockRotation = AxisConstraint.None)
		{
			if (t != null && t.active)
			{
				t.plugOptions.isClosedPath = closePath;
				t.plugOptions.lockPositionAxis = lockPosition;
				t.plugOptions.lockRotationAxis = lockRotation;
				return t;
			}
			return t;
		}

		public static TweenerCore<Vector3, Path, PathOptions> SetLookAt(this TweenerCore<Vector3, Path, PathOptions> t, Vector3 lookAtPosition, Vector3? forwardDirection = default(Vector3?), Vector3? up = default(Vector3?))
		{
			if (t != null && t.active)
			{
				t.plugOptions.orientType = OrientType.LookAtPosition;
				t.plugOptions.lookAtPosition = lookAtPosition;
				t.SetPathForwardDirection(forwardDirection, up);
				return t;
			}
			return t;
		}

		public static TweenerCore<Vector3, Path, PathOptions> SetLookAt(this TweenerCore<Vector3, Path, PathOptions> t, Transform lookAtTransform, Vector3? forwardDirection = default(Vector3?), Vector3? up = default(Vector3?))
		{
			if (t != null && t.active)
			{
				t.plugOptions.orientType = OrientType.LookAtTransform;
				t.plugOptions.lookAtTransform = lookAtTransform;
				t.SetPathForwardDirection(forwardDirection, up);
				return t;
			}
			return t;
		}

		public static TweenerCore<Vector3, Path, PathOptions> SetLookAt(this TweenerCore<Vector3, Path, PathOptions> t, float lookAhead, Vector3? forwardDirection = default(Vector3?), Vector3? up = default(Vector3?))
		{
			if (t != null && t.active)
			{
				t.plugOptions.orientType = OrientType.ToPath;
				if (lookAhead < 0.0001f)
				{
					lookAhead = 0.0001f;
				}
				t.plugOptions.lookAhead = lookAhead;
				t.SetPathForwardDirection(forwardDirection, up);
				return t;
			}
			return t;
		}

		private static void SetPathForwardDirection(this TweenerCore<Vector3, Path, PathOptions> t, Vector3? forwardDirection = default(Vector3?), Vector3? up = default(Vector3?))
		{
			if (t != null && t.active)
			{
				t.plugOptions.hasCustomForwardDirection = ((forwardDirection.HasValue && forwardDirection != (Vector3?)Vector3.zero) || (up.HasValue && up != (Vector3?)Vector3.zero));
				if (t.plugOptions.hasCustomForwardDirection)
				{
					if (forwardDirection == (Vector3?)Vector3.zero)
					{
						forwardDirection = Vector3.forward;
					}
					t.plugOptions.forward = Quaternion.LookRotation((!forwardDirection.HasValue) ? Vector3.forward : forwardDirection.Value, (!up.HasValue) ? Vector3.up : up.Value);
				}
			}
		}
	}
}
