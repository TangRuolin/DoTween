using DG.DOTweenEditor.Core;
using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
	internal class DOTweenUtilityWindow : EditorWindow
	{
		private struct LocationData
		{
			public string dir;

			public string filePath;

			public string adbFilePath;

			public string adbParentDir;

			public LocationData(string srcDir)
			{
				this = default(LocationData);
				this.dir = srcDir;
				this.filePath = this.dir + EditorUtils.pathSlash + "DOTweenSettings.asset";
				this.adbFilePath = EditorUtils.FullPathToADBPath(this.filePath);
				this.adbParentDir = EditorUtils.FullPathToADBPath(this.dir.Substring(0, this.dir.LastIndexOf(EditorUtils.pathSlash)));
			}
		}

		private const string _Title = "DOTween Utility Panel";

		private static readonly Vector2 _WinSize = new Vector2(300f, 405f);

		public const string Id = "DOTweenVersion";

		public const string IdPro = "DOTweenProVersion";

		private static readonly float _HalfBtSize = DOTweenUtilityWindow._WinSize.x * 0.5f - 6f;

		private DOTweenSettings _src;

		private Texture2D _headerImg;

		private Texture2D _footerImg;

		private Vector2 _headerSize;

		private Vector2 _footerSize;

		private string _innerTitle;

		private bool _setupRequired;

		private int _selectedTab;

		private string[] _tabLabels = new string[2]
		{
			"Setup",
			"Preferences"
		};

		private string[] _settingsLocation = new string[3]
		{
			"Assets > Resources",
			"DOTween > Resources",
			"Demigiant > Resources"
		};

		[MenuItem("Tools/Demigiant/DOTween Utility Panel")]
		private static void ShowWindow()
		{
			DOTweenUtilityWindow.Open();
		}

		public static void Open()
		{
			DOTweenUtilityWindow window = EditorWindow.GetWindow<DOTweenUtilityWindow>(true, "DOTween Utility Panel", true);
			window.minSize = DOTweenUtilityWindow._WinSize;
			window.maxSize = DOTweenUtilityWindow._WinSize;
			window.ShowUtility();
			EditorPrefs.SetString("DOTweenVersion", DOTween.Version);
			EditorPrefs.SetString("DOTweenProVersion", EditorUtils.proVersion);
		}

		private void OnHierarchyChange()
		{
			base.Repaint();
		}

		private void OnEnable()
		{
			this._innerTitle = "DOTween v" + DOTween.Version + (DOTween.isDebugBuild ? " [Debug build]" : " [Release build]");
			if (EditorUtils.hasPro)
			{
				this._innerTitle = this._innerTitle + "\nDOTweenPro v" + EditorUtils.proVersion;
			}
			else
			{
				this._innerTitle += "\nDOTweenPro not installed";
			}
			if ((UnityEngine.Object)this._headerImg == (UnityEngine.Object)null)
			{
				this._headerImg = (AssetDatabase.LoadAssetAtPath("Assets/" + EditorUtils.editorADBDir + "Imgs/Header.jpg", typeof(Texture2D)) as Texture2D);
				EditorUtils.SetEditorTexture(this._headerImg, FilterMode.Bilinear, 512);
				this._headerSize.x = DOTweenUtilityWindow._WinSize.x;
				this._headerSize.y = (float)(int)(DOTweenUtilityWindow._WinSize.x * (float)this._headerImg.height / (float)this._headerImg.width);
				this._footerImg = (AssetDatabase.LoadAssetAtPath("Assets/" + EditorUtils.editorADBDir + (EditorGUIUtility.isProSkin ? "Imgs/Footer.png" : "Imgs/Footer_dark.png"), typeof(Texture2D)) as Texture2D);
				EditorUtils.SetEditorTexture(this._footerImg, FilterMode.Bilinear, 256);
				this._footerSize.x = DOTweenUtilityWindow._WinSize.x;
				this._footerSize.y = (float)(int)(DOTweenUtilityWindow._WinSize.x * (float)this._footerImg.height / (float)this._footerImg.width);
			}
			this._setupRequired = EditorUtils.DOTweenSetupRequired();
		}

		private void OnGUI()
		{
			this.Connect(false);
			EditorGUIUtils.SetGUIStyles(this._footerSize);
			if (Application.isPlaying)
			{
				GUILayout.Space(40f);
				GUILayout.BeginHorizontal();
				GUILayout.Space(40f);
				GUILayout.Label("DOTween Utility Panel\nis disabled while in Play Mode", EditorGUIUtils.wrapCenterLabelStyle, GUILayout.ExpandWidth(true));
				GUILayout.Space(40f);
				GUILayout.EndHorizontal();
			}
			else
			{
				Rect position = new Rect(0f, 0f, this._headerSize.x, 30f);
				this._selectedTab = GUI.Toolbar(position, this._selectedTab, this._tabLabels);
				int selectedTab = this._selectedTab;
				if (selectedTab == 1)
				{
					this.DrawPreferencesGUI();
				}
				else
				{
					this.DrawSetupGUI();
				}
			}
		}

		private void DrawSetupGUI()
		{
			Rect position = new Rect(0f, 30f, this._headerSize.x, this._headerSize.y);
			GUI.DrawTexture(position, this._headerImg, ScaleMode.StretchToFill, false);
			GUILayout.Space(position.y + this._headerSize.y + 2f);
			GUILayout.Label(this._innerTitle, DOTween.isDebugBuild ? EditorGUIUtils.redLabelStyle : EditorGUIUtils.boldLabelStyle);
			if (this._setupRequired)
			{
				GUI.backgroundColor = Color.red;
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label("DOTWEEN SETUP REQUIRED", EditorGUIUtils.setupLabelStyle);
				GUILayout.EndVertical();
				GUI.backgroundColor = Color.white;
			}
			else
			{
				GUILayout.Space(8f);
			}
			if (GUILayout.Button("Setup DOTween...", EditorGUIUtils.btStyle))
			{
				DOTweenSetupMenuItem.Setup(false);
				this._setupRequired = EditorUtils.DOTweenSetupRequired();
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Documentation", EditorGUIUtils.btStyle, GUILayout.Width(DOTweenUtilityWindow._HalfBtSize)))
			{
				Application.OpenURL("http://dotween.demigiant.com/documentation.php");
			}
			if (GUILayout.Button("Support", EditorGUIUtils.btStyle, GUILayout.Width(DOTweenUtilityWindow._HalfBtSize)))
			{
				Application.OpenURL("http://dotween.demigiant.com/support.php");
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Changelog", EditorGUIUtils.btStyle, GUILayout.Width(DOTweenUtilityWindow._HalfBtSize)))
			{
				Application.OpenURL("http://dotween.demigiant.com/download.php");
			}
			if (GUILayout.Button("Check Updates", EditorGUIUtils.btStyle, GUILayout.Width(DOTweenUtilityWindow._HalfBtSize)))
			{
				Application.OpenURL("http://dotween.demigiant.com/download.php?v=" + DOTween.Version);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(14f);
			if (GUILayout.Button(this._footerImg, EditorGUIUtils.btImgStyle))
			{
				Application.OpenURL("http://www.demigiant.com/");
			}
		}

		private void DrawPreferencesGUI()
		{
			GUILayout.Space(40f);
			if (GUILayout.Button("Reset", EditorGUIUtils.btStyle))
			{
				this._src.useSafeMode = true;
				this._src.showUnityEditorReport = false;
				this._src.timeScale = 1f;
				this._src.useSmoothDeltaTime = false;
				this._src.logBehaviour = LogBehaviour.ErrorsOnly;
				this._src.drawGizmos = true;
				this._src.defaultRecyclable = false;
				this._src.defaultAutoPlay = AutoPlay.All;
				this._src.defaultUpdateType = UpdateType.Normal;
				this._src.defaultTimeScaleIndependent = false;
				this._src.defaultEaseType = Ease.OutQuad;
				this._src.defaultEaseOvershootOrAmplitude = 1.70158f;
				this._src.defaultEasePeriod = 0f;
				this._src.defaultAutoKill = true;
				this._src.defaultLoopType = LoopType.Restart;
				EditorUtility.SetDirty(this._src);
			}
			GUILayout.Space(8f);
			this._src.useSafeMode = EditorGUILayout.Toggle("Safe Mode", this._src.useSafeMode);
			this._src.timeScale = EditorGUILayout.FloatField("DOTween's TimeScale", this._src.timeScale);
			this._src.useSmoothDeltaTime = EditorGUILayout.Toggle("Smooth DeltaTime", this._src.useSmoothDeltaTime);
			this._src.showUnityEditorReport = EditorGUILayout.Toggle("Editor Report", this._src.showUnityEditorReport);
			this._src.logBehaviour = (LogBehaviour)EditorGUILayout.EnumPopup("Log Behaviour", (Enum)(object)this._src.logBehaviour);
			this._src.drawGizmos = EditorGUILayout.Toggle("Draw Path Gizmos", this._src.drawGizmos);
			DOTweenSettings.SettingsLocation storeSettingsLocation = this._src.storeSettingsLocation;
			this._src.storeSettingsLocation = (DOTweenSettings.SettingsLocation)EditorGUILayout.Popup("Settings Location", (int)this._src.storeSettingsLocation, this._settingsLocation);
			if (this._src.storeSettingsLocation != storeSettingsLocation)
			{
				if (this._src.storeSettingsLocation == DOTweenSettings.SettingsLocation.DemigiantDirectory && EditorUtils.demigiantDir == null)
				{
					EditorUtility.DisplayDialog("Change DOTween Settings Location", "Demigiant directory not present (must be the parent of DOTween's directory)", "Ok");
					if (storeSettingsLocation == DOTweenSettings.SettingsLocation.DemigiantDirectory)
					{
						this._src.storeSettingsLocation = DOTweenSettings.SettingsLocation.AssetsDirectory;
						this.Connect(true);
					}
					else
					{
						this._src.storeSettingsLocation = storeSettingsLocation;
					}
				}
				else
				{
					this.Connect(true);
				}
			}
			GUILayout.Space(8f);
			GUILayout.Label("DEFAULTS ▼");
			this._src.defaultRecyclable = EditorGUILayout.Toggle("Recycle Tweens", this._src.defaultRecyclable);
			this._src.defaultAutoPlay = (AutoPlay)EditorGUILayout.EnumPopup("AutoPlay", (Enum)(object)this._src.defaultAutoPlay);
			this._src.defaultUpdateType = (UpdateType)EditorGUILayout.EnumPopup("Update Type", (Enum)(object)this._src.defaultUpdateType);
			this._src.defaultTimeScaleIndependent = EditorGUILayout.Toggle("TimeScale Independent", this._src.defaultTimeScaleIndependent);
			this._src.defaultEaseType = (Ease)EditorGUILayout.EnumPopup("Ease", (Enum)(object)this._src.defaultEaseType);
			this._src.defaultEaseOvershootOrAmplitude = EditorGUILayout.FloatField("Ease Overshoot", this._src.defaultEaseOvershootOrAmplitude);
			this._src.defaultEasePeriod = EditorGUILayout.FloatField("Ease Period", this._src.defaultEasePeriod);
			this._src.defaultAutoKill = EditorGUILayout.Toggle("AutoKill", this._src.defaultAutoKill);
			this._src.defaultLoopType = (LoopType)EditorGUILayout.EnumPopup("Loop Type", (Enum)(object)this._src.defaultLoopType);
			if (GUI.changed)
			{
				EditorUtility.SetDirty(this._src);
			}
		}

		private void Connect(bool forceReconnect = false)
		{
			if ((UnityEngine.Object)this._src != (UnityEngine.Object)null && !forceReconnect)
			{
				return;
			}
			LocationData locationData = new LocationData(EditorUtils.assetsPath + EditorUtils.pathSlash + "Resources");
			LocationData locationData2 = new LocationData(EditorUtils.dotweenDir + "Resources");
			bool flag = EditorUtils.demigiantDir != null;
			LocationData locationData3 = flag ? new LocationData(EditorUtils.demigiantDir + "Resources") : default(LocationData);
			if ((UnityEngine.Object)this._src == (UnityEngine.Object)null)
			{
				this._src = EditorUtils.ConnectToSourceAsset<DOTweenSettings>(locationData.adbFilePath, false);
				if ((UnityEngine.Object)this._src == (UnityEngine.Object)null)
				{
					this._src = EditorUtils.ConnectToSourceAsset<DOTweenSettings>(locationData2.adbFilePath, false);
				}
				if ((UnityEngine.Object)this._src == (UnityEngine.Object)null & flag)
				{
					this._src = EditorUtils.ConnectToSourceAsset<DOTweenSettings>(locationData3.adbFilePath, false);
				}
			}
			if ((UnityEngine.Object)this._src == (UnityEngine.Object)null)
			{
				if (!Directory.Exists(locationData.dir))
				{
					AssetDatabase.CreateFolder(locationData.adbParentDir, "Resources");
				}
				this._src = EditorUtils.ConnectToSourceAsset<DOTweenSettings>(locationData.adbFilePath, true);
			}
			switch (this._src.storeSettingsLocation)
			{
			case DOTweenSettings.SettingsLocation.AssetsDirectory:
				this.MoveSrc(new LocationData[2]
				{
					locationData2,
					locationData3
				}, locationData);
				break;
			case DOTweenSettings.SettingsLocation.DOTweenDirectory:
				this.MoveSrc(new LocationData[2]
				{
					locationData,
					locationData3
				}, locationData2);
				break;
			case DOTweenSettings.SettingsLocation.DemigiantDirectory:
				this.MoveSrc(new LocationData[2]
				{
					locationData,
					locationData2
				}, locationData3);
				break;
			}
		}

		private void MoveSrc(LocationData[] from, LocationData to)
		{
			if (!Directory.Exists(to.dir))
			{
				AssetDatabase.CreateFolder(to.adbParentDir, "Resources");
			}
			foreach (LocationData locationData in from)
			{
				if (File.Exists(locationData.filePath))
				{
					AssetDatabase.MoveAsset(locationData.adbFilePath, to.adbFilePath);
					AssetDatabase.DeleteAsset(locationData.adbFilePath);
					if (Directory.GetDirectories(locationData.dir).Length == 0 && Directory.GetFiles(locationData.dir).Length == 0)
					{
						AssetDatabase.DeleteAsset(EditorUtils.FullPathToADBPath(locationData.dir));
					}
				}
			}
			this._src = EditorUtils.ConnectToSourceAsset<DOTweenSettings>(to.adbFilePath, true);
		}
	}
}
