using DG.DemiEditor;
using UnityEditor;

namespace DG.DOTweenEditor.Core
{
	public class ABSAnimationInspector : Editor
	{
		public static ColorPalette colors = new ColorPalette();

		public static StylePalette styles = new StylePalette();

		public SerializedProperty onStartProperty;

		public SerializedProperty onPlayProperty;

		public SerializedProperty onUpdateProperty;

		public SerializedProperty onStepCompleteProperty;

		public SerializedProperty onCompleteProperty;

		public SerializedProperty onTweenCreatedProperty;

		public override void OnInspectorGUI()
		{
			DeGUI.BeginGUI(ABSAnimationInspector.colors, ABSAnimationInspector.styles);
		}
	}
}
