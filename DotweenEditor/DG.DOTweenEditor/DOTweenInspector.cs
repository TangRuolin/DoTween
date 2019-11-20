using DG.DOTweenEditor.Core;
using DG.Tweening;
using DG.Tweening.Core;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
	[CustomEditor(typeof(DOTweenComponent))]
	public class DOTweenInspector : Editor
	{
		private string _title;

		private readonly StringBuilder _strBuilder = new StringBuilder();

		private bool _showPlayingTweensData;

		private bool _showPausedTweensData;

		private void OnEnable()
		{
			this._strBuilder.Remove(0, this._strBuilder.Length);
			this._strBuilder.Append("DOTween v").Append(DOTween.Version);
			if (DOTween.isDebugBuild)
			{
				this._strBuilder.Append(" [Debug build]");
			}
			else
			{
				this._strBuilder.Append(" [Release build]");
			}
			if (EditorUtils.hasPro)
			{
				this._strBuilder.Append("\nDOTweenPro v").Append(EditorUtils.proVersion);
			}
			else
			{
				this._strBuilder.Append("\nDOTweenPro not installed");
			}
			this._title = this._strBuilder.ToString();
		}

		public override void OnInspectorGUI()
		{
			EditorGUIUtils.SetGUIStyles(null);
			int totActiveTweens = TweenManager.totActiveTweens;
			int num = TweenManager.TotalPlayingTweens();
			int value = totActiveTweens - num;
			int totActiveDefaultTweens = TweenManager.totActiveDefaultTweens;
			int totActiveLateTweens = TweenManager.totActiveLateTweens;
			GUILayout.Space(4f);
			GUILayout.Label(this._title, DOTween.isDebugBuild ? EditorGUIUtils.redLabelStyle : EditorGUIUtils.boldLabelStyle);
			GUILayout.Space(6f);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Documentation"))
			{
				Application.OpenURL("http://dotween.demigiant.com/documentation.php");
			}
			if (GUILayout.Button("Check Updates"))
			{
				Application.OpenURL("http://dotween.demigiant.com/download.php?v=" + DOTween.Version);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(this._showPlayingTweensData ? "Hide Playing Tweens" : "Show Playing Tweens"))
			{
				this._showPlayingTweensData = !this._showPlayingTweensData;
			}
			if (GUILayout.Button(this._showPausedTweensData ? "Hide Paused Tweens" : "Show Paused Tweens"))
			{
				this._showPausedTweensData = !this._showPausedTweensData;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Play all"))
			{
				DOTween.PlayAll();
			}
			if (GUILayout.Button("Pause all"))
			{
				DOTween.PauseAll();
			}
			if (GUILayout.Button("Kill all"))
			{
				DOTween.KillAll(false);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(8f);
			this._strBuilder.Length = 0;
			this._strBuilder.Append("Active tweens: ").Append(totActiveTweens).Append(" (")
				.Append(TweenManager.totActiveTweeners)
				.Append("/")
				.Append(TweenManager.totActiveSequences)
				.Append(")")
				.Append("\nDefault/Late tweens: ")
				.Append(totActiveDefaultTweens)
				.Append("/")
				.Append(totActiveLateTweens)
				.Append("\nPlaying tweens: ")
				.Append(num);
			if (this._showPlayingTweensData)
			{
				Tween[] activeTweens = TweenManager._activeTweens;
				foreach (Tween tween in activeTweens)
				{
					if (tween != null && tween.isPlaying)
					{
						this._strBuilder.Append("\n   - [").Append(tween.tweenType).Append("] ")
							.Append(tween.target);
					}
				}
			}
			this._strBuilder.Append("\nPaused tweens: ").Append(value);
			if (this._showPausedTweensData)
			{
				Tween[] activeTweens = TweenManager._activeTweens;
				foreach (Tween tween2 in activeTweens)
				{
					if (tween2 != null && !tween2.isPlaying)
					{
						this._strBuilder.Append("\n   - [").Append(tween2.tweenType).Append("] ")
							.Append(tween2.target);
					}
				}
			}
			this._strBuilder.Append("\nPooled tweens: ").Append(TweenManager.TotalPooledTweens()).Append(" (")
				.Append(TweenManager.totPooledTweeners)
				.Append("/")
				.Append(TweenManager.totPooledSequences)
				.Append(")");
			GUILayout.Label(this._strBuilder.ToString());
			GUILayout.Space(8f);
			this._strBuilder.Remove(0, this._strBuilder.Length);
			this._strBuilder.Append("Tweens Capacity: ").Append(TweenManager.maxTweeners).Append("/")
				.Append(TweenManager.maxSequences)
				.Append("\nMax Simultaneous Active Tweens: ")
				.Append(DOTween.maxActiveTweenersReached)
				.Append("/")
				.Append(DOTween.maxActiveSequencesReached);
			GUILayout.Label(this._strBuilder.ToString());
			GUILayout.Space(8f);
			this._strBuilder.Remove(0, this._strBuilder.Length);
			this._strBuilder.Append("SETTINGS ▼");
			this._strBuilder.Append("\nSafe Mode: ").Append(DOTween.useSafeMode ? "ON" : "OFF");
			this._strBuilder.Append("\nLog Behaviour: ").Append(DOTween.logBehaviour);
			this._strBuilder.Append("\nShow Unity Editor Report: ").Append(DOTween.showUnityEditorReport);
			this._strBuilder.Append("\nTimeScale (Unity/DOTween): ").Append(Time.timeScale).Append("/")
				.Append(DOTween.timeScale);
			GUILayout.Label(this._strBuilder.ToString());
			GUILayout.Label("NOTE: DOTween's TimeScale is not the same as Unity's Time.timeScale: it is actually multiplied by it except for tweens that are set to update independently", EditorGUIUtils.wordWrapItalicLabelStyle);
			GUILayout.Space(8f);
			this._strBuilder.Remove(0, this._strBuilder.Length);
			this._strBuilder.Append("DEFAULTS ▼");
			this._strBuilder.Append("\ndefaultRecyclable: ").Append(DOTween.defaultRecyclable);
			this._strBuilder.Append("\ndefaultUpdateType: ").Append(DOTween.defaultUpdateType);
			this._strBuilder.Append("\ndefaultTSIndependent: ").Append(DOTween.defaultTimeScaleIndependent);
			this._strBuilder.Append("\ndefaultAutoKill: ").Append(DOTween.defaultAutoKill);
			this._strBuilder.Append("\ndefaultAutoPlay: ").Append(DOTween.defaultAutoPlay);
			this._strBuilder.Append("\ndefaultEaseType: ").Append(DOTween.defaultEaseType);
			this._strBuilder.Append("\ndefaultLoopType: ").Append(DOTween.defaultLoopType);
			GUILayout.Label(this._strBuilder.ToString());
			GUILayout.Space(10f);
		}
	}
}
