using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System.Collections.Generic;
using UnityEngine;

namespace DG.Tweening
{
	[AddComponentMenu("DOTween/DOTween Path")]
	public class DOTweenPath : ABSAnimationComponent
	{
		public float delay;

		public float duration = 1f;

		public Ease easeType = Ease.OutQuad;

		public AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public int loops = 1;

		public string id = "";

		public LoopType loopType;

		public OrientType orientType;

		public Transform lookAtTransform;

		public Vector3 lookAtPosition;

		public float lookAhead = 0.01f;

		public bool autoPlay = true;

		public bool autoKill = true;

		public bool relative;

		public bool isLocal;

		public bool isClosedPath;

		public int pathResolution = 10;

		public PathMode pathMode = PathMode.Full3D;

		public AxisConstraint lockRotation;

		public bool assignForwardAndUp;

		public Vector3 forwardDirection = Vector3.forward;

		public Vector3 upDirection = Vector3.up;

		public List<Vector3> wps = new List<Vector3>();

		public List<Vector3> fullWps = new List<Vector3>();

		public Path path;

		public DOTweenInspectorMode inspectorMode;

		public PathType pathType;

		public HandlesType handlesType;

		public bool livePreview = true;

		public HandlesDrawMode handlesDrawMode;

		public float perspectiveHandleSize = 0.5f;

		public bool showIndexes = true;

		public bool showWpLength;

		public Color pathColor = new Color(1f, 1f, 1f, 0.5f);

		public Vector3 lastSrcPosition;

		public bool wpsDropdown;

		private void Awake()
		{
			if (this.path != null && this.wps.Count >= 1)
			{
				this.path.AssignDecoder(this.path.type);
				if (DOTween.isUnityEditor)
				{
					DOTween.GizmosDelegates.Add(this.path.Draw);
					this.path.gizmoColor = this.pathColor;
				}
				if (this.isLocal)
				{
					Transform transform = base.transform;
					if ((Object)transform.parent != (Object)null)
					{
						transform = transform.parent;
						Vector3 position = transform.position;
						int num = this.path.wps.Length;
						for (int i = 0; i < num; i++)
						{
							this.path.wps[i] = this.path.wps[i] - position;
						}
						num = this.path.controlPoints.Length;
						for (int j = 0; j < num; j++)
						{
							ControlPoint controlPoint = this.path.controlPoints[j];
							ref Vector3 a = ref controlPoint.a;
							a -= position;
							ref Vector3 b = ref controlPoint.b;
							b -= position;
							this.path.controlPoints[j] = controlPoint;
						}
					}
				}
				if (this.relative)
				{
					this.ReEvaluateRelativeTween();
				}
				if (this.pathMode == PathMode.Full3D && (Object)base.GetComponent<SpriteRenderer>() != (Object)null)
				{
					this.pathMode = PathMode.TopDown2D;
				}
				TweenerCore<Vector3, Path, PathOptions> tweenerCore = this.isLocal ? base.transform.DOLocalPath(this.path, this.duration, this.pathMode).SetOptions(this.isClosedPath, AxisConstraint.None, this.lockRotation) : base.transform.DOPath(this.path, this.duration, this.pathMode).SetOptions(this.isClosedPath, AxisConstraint.None, this.lockRotation);
				switch (this.orientType)
				{
				case OrientType.LookAtTransform:
					if ((Object)this.lookAtTransform != (Object)null)
					{
						if (this.assignForwardAndUp)
						{
							tweenerCore.SetLookAt(this.lookAtTransform, this.forwardDirection, this.upDirection);
						}
						else
						{
							tweenerCore.SetLookAt(this.lookAtTransform, null, null);
						}
					}
					break;
				case OrientType.LookAtPosition:
					if (this.assignForwardAndUp)
					{
						tweenerCore.SetLookAt(this.lookAtPosition, this.forwardDirection, this.upDirection);
					}
					else
					{
						tweenerCore.SetLookAt(this.lookAtPosition, null, null);
					}
					break;
				case OrientType.ToPath:
					if (this.assignForwardAndUp)
					{
						tweenerCore.SetLookAt(this.lookAhead, this.forwardDirection, this.upDirection);
					}
					else
					{
						tweenerCore.SetLookAt(this.lookAhead, null, null);
					}
					break;
				}
				tweenerCore.SetDelay(this.delay).SetLoops(this.loops, this.loopType).SetAutoKill(this.autoKill)
					.SetUpdate(base.updateType)
					.OnKill(delegate
					{
						base.tween = null;
					});
				if (base.isSpeedBased)
				{
					tweenerCore.SetSpeedBased();
				}
				if (this.easeType == Ease.INTERNAL_Custom)
				{
					tweenerCore.SetEase(this.easeCurve);
				}
				else
				{
					tweenerCore.SetEase(this.easeType);
				}
				if (!string.IsNullOrEmpty(this.id))
				{
					tweenerCore.SetId(this.id);
				}
				if (base.hasOnStart)
				{
					if (base.onStart != null)
					{
						tweenerCore.OnStart(base.onStart.Invoke);
					}
				}
				else
				{
					base.onStart = null;
				}
				if (base.hasOnPlay)
				{
					if (base.onPlay != null)
					{
						tweenerCore.OnPlay(base.onPlay.Invoke);
					}
				}
				else
				{
					base.onPlay = null;
				}
				if (base.hasOnUpdate)
				{
					if (base.onUpdate != null)
					{
						tweenerCore.OnUpdate(base.onUpdate.Invoke);
					}
				}
				else
				{
					base.onUpdate = null;
				}
				if (base.hasOnStepComplete)
				{
					if (base.onStepComplete != null)
					{
						tweenerCore.OnStepComplete(base.onStepComplete.Invoke);
					}
				}
				else
				{
					base.onStepComplete = null;
				}
				if (base.hasOnComplete)
				{
					if (base.onComplete != null)
					{
						tweenerCore.OnComplete(base.onComplete.Invoke);
					}
				}
				else
				{
					base.onComplete = null;
				}
				if (this.autoPlay)
				{
					tweenerCore.Play();
				}
				else
				{
					tweenerCore.Pause();
				}
				base.tween = tweenerCore;
				if (base.hasOnTweenCreated && base.onTweenCreated != null)
				{
					base.onTweenCreated.Invoke();
				}
			}
		}

		private void Reset()
		{
			this.path = new Path(this.pathType, this.wps.ToArray(), 10, this.pathColor);
		}

		private void OnDestroy()
		{
			if (base.tween != null && base.tween.active)
			{
				base.tween.Kill(false);
			}
			base.tween = null;
		}

		public override void DOPlay()
		{
			base.tween.Play();
		}

		public override void DOPlayBackwards()
		{
			base.tween.PlayBackwards();
		}

		public override void DOPlayForward()
		{
			base.tween.PlayForward();
		}

		public override void DOPause()
		{
			base.tween.Pause();
		}

		public override void DOTogglePause()
		{
			base.tween.TogglePause();
		}

		public override void DORewind()
		{
			base.tween.Rewind(true);
		}

		public override void DORestart(bool fromHere = false)
		{
			if (base.tween == null)
			{
				if (Debugger.logPriority > 1)
				{
					Debugger.LogNullTween(base.tween);
				}
			}
			else
			{
				if (fromHere && this.relative && !this.isLocal)
				{
					this.ReEvaluateRelativeTween();
				}
				base.tween.Restart(true);
			}
		}

		public override void DOComplete()
		{
			base.tween.Complete();
		}

		public override void DOKill()
		{
			base.tween.Kill(false);
		}

		public Tween GetTween()
		{
			if (base.tween != null && base.tween.active)
			{
				return base.tween;
			}
			if (Debugger.logPriority > 1)
			{
				if (base.tween == null)
				{
					Debugger.LogNullTween(base.tween);
				}
				else
				{
					Debugger.LogInvalidTween(base.tween);
				}
			}
			return null;
		}

		public Vector3[] GetDrawPoints()
		{
			if (this.path.wps != null && this.path.nonLinearDrawWps != null)
			{
				if (this.pathType == PathType.Linear)
				{
					return this.path.wps;
				}
				return this.path.nonLinearDrawWps;
			}
			Debugger.LogWarning("Draw points not ready yet. Returning NULL");
			return null;
		}

		internal Vector3[] GetFullWps()
		{
			int count = this.wps.Count;
			int num = count + 1;
			if (this.isClosedPath)
			{
				num++;
			}
			Vector3[] array = new Vector3[num];
			array[0] = base.transform.position;
			for (int i = 0; i < count; i++)
			{
				array[i + 1] = this.wps[i];
			}
			if (this.isClosedPath)
			{
				array[num - 1] = array[0];
			}
			return array;
		}

		private void ReEvaluateRelativeTween()
		{
			Vector3 position = base.transform.position;
			if (!(position == this.lastSrcPosition))
			{
				Vector3 b = position - this.lastSrcPosition;
				int num = this.path.wps.Length;
				for (int i = 0; i < num; i++)
				{
					this.path.wps[i] = this.path.wps[i] + b;
				}
				num = this.path.controlPoints.Length;
				for (int j = 0; j < num; j++)
				{
					ControlPoint controlPoint = this.path.controlPoints[j];
					ref Vector3 a = ref controlPoint.a;
					a += b;
					ref Vector3 b2 = ref controlPoint.b;
					b2 += b;
					this.path.controlPoints[j] = controlPoint;
				}
				this.lastSrcPosition = position;
			}
		}
	}
}
