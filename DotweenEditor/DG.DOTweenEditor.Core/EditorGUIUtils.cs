using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor.Core
{
	public static class EditorGUIUtils
	{
		private static bool _stylesSet;

		private static bool _additionalStylesSet;

		public static GUIStyle boldLabelStyle;

		public static GUIStyle setupLabelStyle;

		public static GUIStyle redLabelStyle;

		public static GUIStyle btStyle;

		public static GUIStyle btImgStyle;

		public static GUIStyle wrapCenterLabelStyle;

		public static GUIStyle handlelabelStyle;

		public static GUIStyle handleSelectedLabelStyle;

		public static GUIStyle wordWrapLabelStyle;

		public static GUIStyle wordWrapItalicLabelStyle;

		public static GUIStyle titleStyle;

		public static GUIStyle logoIconStyle;

		public static GUIStyle sideBtStyle;

		public static GUIStyle sideLogoIconBoldLabelStyle;

		public static GUIStyle wordWrapTextArea;

		public static GUIStyle popupButton;

		public static GUIStyle btIconStyle;

		private static Texture2D _logo;

		internal static readonly string[] FilteredEaseTypes = new string[32]
		{
			"Linear",
			"InSine",
			"OutSine",
			"InOutSine",
			"InQuad",
			"OutQuad",
			"InOutQuad",
			"InCubic",
			"OutCubic",
			"InOutCubic",
			"InQuart",
			"OutQuart",
			"InOutQuart",
			"InQuint",
			"OutQuint",
			"InOutQuint",
			"InExpo",
			"OutExpo",
			"InOutExpo",
			"InCirc",
			"OutCirc",
			"InOutCirc",
			"InElastic",
			"OutElastic",
			"InOutElastic",
			"InBack",
			"OutBack",
			"InOutBack",
			"InBounce",
			"OutBounce",
			"InOutBounce",
			":: AnimationCurve"
		};

		public static Texture2D logo
		{
			get
			{
				if ((UnityEngine.Object)EditorGUIUtils._logo == (UnityEngine.Object)null)
				{
					EditorGUIUtils._logo = (AssetDatabase.LoadAssetAtPath("Assets/" + EditorUtils.editorADBDir + "Imgs/DOTweenIcon.png", typeof(Texture2D)) as Texture2D);
					EditorUtils.SetEditorTexture(EditorGUIUtils._logo, FilterMode.Bilinear, 128);
				}
				return EditorGUIUtils._logo;
			}
		}

		public static Ease FilteredEasePopup(Ease currEase)
		{
			int num = (currEase == Ease.INTERNAL_Custom) ? (EditorGUIUtils.FilteredEaseTypes.Length - 1) : Array.IndexOf(EditorGUIUtils.FilteredEaseTypes, currEase.ToString());
			if (num == -1)
			{
				num = 0;
			}
			num = EditorGUILayout.Popup("Ease", num, EditorGUIUtils.FilteredEaseTypes);
			if (num != EditorGUIUtils.FilteredEaseTypes.Length - 1)
			{
				return (Ease)Enum.Parse(typeof(Ease), EditorGUIUtils.FilteredEaseTypes[num]);
			}
			return Ease.INTERNAL_Custom;
		}

		public static void InspectorLogo()
		{
			GUILayout.Box(EditorGUIUtils.logo, EditorGUIUtils.logoIconStyle);
		}

		public static bool ToggleButton(bool toggled, GUIContent content, GUIStyle guiStyle = null, params GUILayoutOption[] options)
		{
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = (toggled ? Color.green : Color.white);
			if ((guiStyle == null) ? GUILayout.Button(content, options) : GUILayout.Button(content, guiStyle, options))
			{
				toggled = !toggled;
				GUI.changed = true;
			}
			GUI.backgroundColor = backgroundColor;
			return toggled;
		}

		public static void SetGUIStyles(Vector2? footerSize = default(Vector2?))
		{
			if (!EditorGUIUtils._additionalStylesSet && footerSize.HasValue)
			{
				EditorGUIUtils._additionalStylesSet = true;
				Vector2 value = footerSize.Value;
				EditorGUIUtils.btImgStyle = new GUIStyle(GUI.skin.button);
				EditorGUIUtils.btImgStyle.normal.background = null;
				EditorGUIUtils.btImgStyle.imagePosition = ImagePosition.ImageOnly;
				EditorGUIUtils.btImgStyle.padding = new RectOffset(0, 0, 0, 0);
				EditorGUIUtils.btImgStyle.fixedWidth = value.x;
				EditorGUIUtils.btImgStyle.fixedHeight = value.y;
			}
			if (!EditorGUIUtils._stylesSet)
			{
				EditorGUIUtils._stylesSet = true;
				EditorGUIUtils.boldLabelStyle = new GUIStyle(GUI.skin.label);
				EditorGUIUtils.boldLabelStyle.fontStyle = FontStyle.Bold;
				EditorGUIUtils.redLabelStyle = new GUIStyle(GUI.skin.label);
				EditorGUIUtils.redLabelStyle.normal.textColor = Color.red;
				EditorGUIUtils.setupLabelStyle = new GUIStyle(EditorGUIUtils.boldLabelStyle);
				EditorGUIUtils.setupLabelStyle.alignment = TextAnchor.MiddleCenter;
				EditorGUIUtils.wrapCenterLabelStyle = new GUIStyle(GUI.skin.label);
				EditorGUIUtils.wrapCenterLabelStyle.wordWrap = true;
				EditorGUIUtils.wrapCenterLabelStyle.alignment = TextAnchor.MiddleCenter;
				EditorGUIUtils.btStyle = new GUIStyle(GUI.skin.button);
				EditorGUIUtils.btStyle.padding = new RectOffset(0, 0, 10, 10);
				EditorGUIUtils.titleStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 12,
					fontStyle = FontStyle.Bold
				};
				EditorGUIUtils.handlelabelStyle = new GUIStyle(GUI.skin.label)
				{
					normal = 
					{
						textColor = Color.white
					},
					alignment = TextAnchor.MiddleLeft
				};
				EditorGUIUtils.handleSelectedLabelStyle = new GUIStyle(EditorGUIUtils.handlelabelStyle)
				{
					normal = 
					{
						textColor = Color.yellow
					},
					fontStyle = FontStyle.Bold
				};
				EditorGUIUtils.wordWrapLabelStyle = new GUIStyle(GUI.skin.label);
				EditorGUIUtils.wordWrapLabelStyle.wordWrap = true;
				EditorGUIUtils.wordWrapItalicLabelStyle = new GUIStyle(EditorGUIUtils.wordWrapLabelStyle);
				EditorGUIUtils.wordWrapItalicLabelStyle.fontStyle = FontStyle.Italic;
				EditorGUIUtils.logoIconStyle = new GUIStyle(GUI.skin.box);
				GUIStyleState active = EditorGUIUtils.logoIconStyle.active;
				GUIStyleState normal = EditorGUIUtils.logoIconStyle.normal;
				Texture2D texture2D3 = active.background = (normal.background = null);
				EditorGUIUtils.logoIconStyle.margin = new RectOffset(0, 0, 0, 0);
				EditorGUIUtils.logoIconStyle.padding = new RectOffset(0, 0, 0, 0);
				EditorGUIUtils.sideBtStyle = new GUIStyle(GUI.skin.button);
				EditorGUIUtils.sideBtStyle.margin.top = 1;
				EditorGUIUtils.sideBtStyle.padding = new RectOffset(0, 0, 2, 2);
				EditorGUIUtils.sideLogoIconBoldLabelStyle = new GUIStyle(EditorGUIUtils.boldLabelStyle);
				EditorGUIUtils.sideLogoIconBoldLabelStyle.alignment = TextAnchor.MiddleLeft;
				EditorGUIUtils.sideLogoIconBoldLabelStyle.padding.top = 2;
				EditorGUIUtils.wordWrapTextArea = new GUIStyle(GUI.skin.textArea);
				EditorGUIUtils.wordWrapTextArea.wordWrap = true;
				EditorGUIUtils.popupButton = new GUIStyle(EditorStyles.popup);
				EditorGUIUtils.popupButton.fixedHeight = 18f;
				EditorGUIUtils.popupButton.margin.top++;
				EditorGUIUtils.btIconStyle = new GUIStyle(GUI.skin.button);
				EditorGUIUtils.btIconStyle.padding.left -= 2;
				EditorGUIUtils.btIconStyle.fixedWidth = 24f;
				EditorGUIUtils.btIconStyle.stretchWidth = false;
			}
		}
	}
}
