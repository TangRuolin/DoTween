using DG.DOTweenEditor.Core;
using DG.Tweening;
using DG.Tweening.Core;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DG.DOTweenEditor
{
	[CustomEditor(typeof(DOTweenVisualManager))]
	public class DOTweenVisualManagerInspector : Editor
	{
		private DOTweenVisualManager _src;

		private void OnEnable()
		{
			this._src = (base.target as DOTweenVisualManager);
			if (!Application.isPlaying)
			{
				MonoBehaviour[] components = ((Component)this._src).GetComponents<MonoBehaviour>();
				int num = ArrayUtility.IndexOf(components, this._src);
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (components[i] is ABSAnimationComponent)
					{
						num2++;
					}
				}
				while (num2 > 0)
				{
					num2--;
					ComponentUtility.MoveComponentUp(this._src);
				}
			}
		}

		public override void OnInspectorGUI()
		{
			EditorGUIUtils.SetGUIStyles(null);
			EditorGUIUtility.labelWidth = 80f;
			EditorGUIUtils.InspectorLogo();
			VisualManagerPreset preset = this._src.preset;
			this._src.preset = (VisualManagerPreset)EditorGUILayout.EnumPopup("Preset", (Enum)(object)this._src.preset);
			if (preset != this._src.preset)
			{
				VisualManagerPreset preset2 = this._src.preset;
				if (preset2 == VisualManagerPreset.PoolingSystem)
				{
					this._src.onEnableBehaviour = OnEnableBehaviour.RestartFromSpawnPoint;
					this._src.onDisableBehaviour = OnDisableBehaviour.Rewind;
				}
			}
			GUILayout.Space(6f);
			bool flag = this._src.preset != VisualManagerPreset.Custom;
			OnEnableBehaviour onEnableBehaviour = this._src.onEnableBehaviour;
			OnDisableBehaviour onDisableBehaviour = this._src.onDisableBehaviour;
			this._src.onEnableBehaviour = (OnEnableBehaviour)EditorGUILayout.EnumPopup(new GUIContent("On Enable", "Eventual actions to perform when this gameObject is activated"), (Enum)(object)this._src.onEnableBehaviour);
			this._src.onDisableBehaviour = (OnDisableBehaviour)EditorGUILayout.EnumPopup(new GUIContent("On Disable", "Eventual actions to perform when this gameObject is deactivated"), (Enum)(object)this._src.onDisableBehaviour);
			if (flag && onEnableBehaviour != this._src.onEnableBehaviour)
			{
				goto IL_0156;
			}
			if (onDisableBehaviour != this._src.onDisableBehaviour)
			{
				goto IL_0156;
			}
			goto IL_0162;
			IL_0162:
			if (GUI.changed)
			{
				EditorUtility.SetDirty(this._src);
			}
			return;
			IL_0156:
			this._src.preset = VisualManagerPreset.Custom;
			goto IL_0162;
		}
	}
}
