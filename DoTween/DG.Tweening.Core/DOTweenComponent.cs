using System.Collections;
using UnityEngine;

namespace DG.Tweening.Core
{
	[AddComponentMenu("")]
	public class DOTweenComponent : MonoBehaviour, IDOTweenInit
	{
		public int inspectorUpdater;

		private float _unscaledTime;

		private float _unscaledDeltaTime;

		private bool _duplicateToDestroy;

		private void Awake()
		{
			this.inspectorUpdater = 0;
			this._unscaledTime = Time.realtimeSinceStartup;
		}

		private void Start()
		{
			if ((Object)DOTween.instance != (Object)this)
			{
				this._duplicateToDestroy = true;
				Object.Destroy(base.gameObject);
			}
		}

		private void Update()
		{
			this._unscaledDeltaTime = Time.realtimeSinceStartup - this._unscaledTime;
			if (TweenManager.hasActiveDefaultTweens)
			{
				TweenManager.Update(UpdateType.Normal, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) * DOTween.timeScale, this._unscaledDeltaTime * DOTween.timeScale);
			}
			this._unscaledTime = Time.realtimeSinceStartup;
			if (DOTween.isUnityEditor)
			{
				this.inspectorUpdater++;
				if (DOTween.showUnityEditorReport && TweenManager.hasActiveTweens)
				{
					if (TweenManager.totActiveTweeners > DOTween.maxActiveTweenersReached)
					{
						DOTween.maxActiveTweenersReached = TweenManager.totActiveTweeners;
					}
					if (TweenManager.totActiveSequences > DOTween.maxActiveSequencesReached)
					{
						DOTween.maxActiveSequencesReached = TweenManager.totActiveSequences;
					}
				}
			}
		}

		private void LateUpdate()
		{
			if (TweenManager.hasActiveLateTweens)
			{
				TweenManager.Update(UpdateType.Late, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) * DOTween.timeScale, this._unscaledDeltaTime * DOTween.timeScale);
			}
		}

		private void FixedUpdate()
		{
			if (TweenManager.hasActiveFixedTweens && Time.timeScale > 0f)
			{
				TweenManager.Update(UpdateType.Fixed, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) * DOTween.timeScale, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) / Time.timeScale * DOTween.timeScale);
			}
		}

		private void OnDrawGizmos()
		{
			if (DOTween.drawGizmos && DOTween.isUnityEditor)
			{
				int count = DOTween.GizmosDelegates.Count;
				if (count != 0)
				{
					for (int i = 0; i < count; i++)
					{
						DOTween.GizmosDelegates[i]();
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (!this._duplicateToDestroy)
			{
				if (DOTween.showUnityEditorReport)
				{
					Debugger.LogReport("REPORT > Max overall simultaneous active Tweeners/Sequences: " + DOTween.maxActiveTweenersReached + "/" + DOTween.maxActiveSequencesReached);
				}
				if ((Object)DOTween.instance == (Object)this)
				{
					DOTween.instance = null;
				}
			}
		}

		private void OnApplicationQuit()
		{
			DOTween.isQuitting = true;
		}

		public IDOTweenInit SetCapacity(int tweenersCapacity, int sequencesCapacity)
		{
			TweenManager.SetCapacities(tweenersCapacity, sequencesCapacity);
			return this;
		}

		internal IEnumerator WaitForCompletion(Tween t)
		{
			while (t.active && !t.isComplete)
			{
				yield return (object)null;
			}
		}

		internal IEnumerator WaitForRewind(Tween t)
		{
			while (true)
			{
				if (!t.active)
				{
					break;
				}
				if (t.playedOnce && !(t.position * (float)(t.completedLoops + 1) > 0f))
				{
					break;
				}
				yield return (object)null;
			}
		}

		internal IEnumerator WaitForKill(Tween t)
		{
			while (t.active)
			{
				yield return (object)null;
			}
		}

		internal IEnumerator WaitForElapsedLoops(Tween t, int elapsedLoops)
		{
			while (t.active && t.completedLoops < elapsedLoops)
			{
				yield return (object)null;
			}
		}

		internal IEnumerator WaitForPosition(Tween t, float position)
		{
			while (t.active && t.position * (float)(t.completedLoops + 1) < position)
			{
				yield return (object)null;
			}
		}

		internal IEnumerator WaitForStart(Tween t)
		{
			while (t.active && !t.playedOnce)
			{
				yield return (object)null;
			}
		}

		internal static void Create()
		{
			if (!((Object)DOTween.instance != (Object)null))
			{
				GameObject gameObject = new GameObject("[DOTween]");
				Object.DontDestroyOnLoad(gameObject);
				DOTween.instance = gameObject.AddComponent<DOTweenComponent>();
			}
		}

		internal static void DestroyInstance()
		{
			if ((Object)DOTween.instance != (Object)null)
			{
				Object.Destroy(DOTween.instance.gameObject);
			}
			DOTween.instance = null;
		}
	}
}
