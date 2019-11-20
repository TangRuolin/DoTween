using DG.Tweening.Core;
using UnityEngine;

namespace DG.Tweening
{
	[AddComponentMenu("")]
	public class DOTweenVisualManager : MonoBehaviour
	{
		public VisualManagerPreset preset;

		public OnEnableBehaviour onEnableBehaviour;

		public OnDisableBehaviour onDisableBehaviour;

		private bool _requiresRestartFromSpawnPoint;

		private void Update()
		{
			if (this._requiresRestartFromSpawnPoint)
			{
				this._requiresRestartFromSpawnPoint = false;
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if (!((Object)component == (Object)null))
				{
					component.DORestart(true);
				}
			}
		}

		private void OnEnable()
		{
			switch (this.onEnableBehaviour)
			{
			case OnEnableBehaviour.Play:
			{
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if ((Object)component != (Object)null)
				{
					component.DOPlay();
				}
				break;
			}
			case OnEnableBehaviour.Restart:
			{
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if ((Object)component != (Object)null)
				{
					component.DORestart(false);
				}
				break;
			}
			case OnEnableBehaviour.RestartFromSpawnPoint:
				this._requiresRestartFromSpawnPoint = true;
				break;
			}
		}

		private void OnDisable()
		{
			this._requiresRestartFromSpawnPoint = false;
			switch (this.onDisableBehaviour)
			{
			case OnDisableBehaviour.Pause:
			{
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if ((Object)component != (Object)null)
				{
					component.DOPause();
				}
				break;
			}
			case OnDisableBehaviour.Rewind:
			{
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if ((Object)component != (Object)null)
				{
					component.DORewind();
				}
				break;
			}
			case OnDisableBehaviour.Kill:
			{
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if ((Object)component != (Object)null)
				{
					component.DOKill();
				}
				break;
			}
			case OnDisableBehaviour.KillAndComplete:
			{
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if ((Object)component != (Object)null)
				{
					component.DOComplete();
					component.DOKill();
				}
				break;
			}
			case OnDisableBehaviour.DestroyGameObject:
			{
				ABSAnimationComponent component = base.GetComponent<ABSAnimationComponent>();
				if ((Object)component != (Object)null)
				{
					component.DOKill();
				}
				Object.Destroy(base.gameObject);
				break;
			}
			}
		}
	}
}
