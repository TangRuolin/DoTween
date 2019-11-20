using DG.DemiEditor;
using DG.DemiLib;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor.Core
{
	public class AnimationInspectorGUI
	{
		public static void AnimationEvents(ABSAnimationInspector inspector, ABSAnimationComponent src)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			GUILayout.Space(6f);
			AnimationInspectorGUI.StickyTitle("Events");
			GUILayout.BeginHorizontal();
			src.hasOnStart = DeGUILayout.ToggleButton(src.hasOnStart, new GUIContent("OnStart", "Event called the first time the tween starts, after any eventual delay"), ABSAnimationInspector.styles.button.tool, new GUILayoutOption[0]);
			src.hasOnPlay = DeGUILayout.ToggleButton(src.hasOnPlay, new GUIContent("OnPlay", "Event called each time the tween status changes from a pause to a play state (including the first time the tween starts playing), after any eventual delay"), ABSAnimationInspector.styles.button.tool, new GUILayoutOption[0]);
			src.hasOnUpdate = DeGUILayout.ToggleButton(src.hasOnUpdate, new GUIContent("OnUpdate", "Event called every frame while the tween is playing"), ABSAnimationInspector.styles.button.tool, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			src.hasOnStepComplete = DeGUILayout.ToggleButton(src.hasOnStepComplete, new GUIContent("OnStep", "Event called at the end of each loop cycle"), ABSAnimationInspector.styles.button.tool, new GUILayoutOption[0]);
			src.hasOnComplete = DeGUILayout.ToggleButton(src.hasOnComplete, new GUIContent("OnComplete", "Event called at the end of the tween, all loops included"), ABSAnimationInspector.styles.button.tool, new GUILayoutOption[0]);
			src.hasOnTweenCreated = DeGUILayout.ToggleButton(src.hasOnTweenCreated, new GUIContent("OnCreated", "Event called as soon as the tween is instantiated"), ABSAnimationInspector.styles.button.tool, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			if (src.hasOnStart || src.hasOnPlay || src.hasOnUpdate || src.hasOnStepComplete || src.hasOnComplete || src.hasOnTweenCreated)
			{
				inspector.serializedObject.Update();
				DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
				if (src.hasOnStart)
				{
					EditorGUILayout.PropertyField(inspector.onStartProperty);
				}
				if (src.hasOnPlay)
				{
					EditorGUILayout.PropertyField(inspector.onPlayProperty);
				}
				if (src.hasOnUpdate)
				{
					EditorGUILayout.PropertyField(inspector.onUpdateProperty);
				}
				if (src.hasOnStepComplete)
				{
					EditorGUILayout.PropertyField(inspector.onStepCompleteProperty);
				}
				if (src.hasOnComplete)
				{
					EditorGUILayout.PropertyField(inspector.onCompleteProperty);
				}
				if (src.hasOnTweenCreated)
				{
					EditorGUILayout.PropertyField(inspector.onTweenCreatedProperty);
				}
				inspector.serializedObject.ApplyModifiedProperties();
				DeGUILayout.EndVBox();
			}
			else
			{
				GUILayout.Space(4f);
			}
		}

		public static void StickyTitle(string text)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			GUILayout.Label(text, ABSAnimationInspector.styles.custom.stickyTitle);
			DeGUILayout.HorizontalDivider((Color?)DeSkinColor.op_Implicit(ABSAnimationInspector.colors.custom.stickyDivider), 2, 0, 0);
		}
	}
}
