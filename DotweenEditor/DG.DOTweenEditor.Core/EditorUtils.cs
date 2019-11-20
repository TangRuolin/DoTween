using DG.Tweening;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor.Core
{
	public static class EditorUtils
	{
		private static bool _hasPro;

		private static string _proVersion;

		private static bool _hasCheckedForPro;

		private static string _editorADBDir;

		private static string _demigiantDir;

		private static string _dotweenDir;

		private static string _dotweenProDir;

		public static string projectPath
		{
			get;
			private set;
		}

		public static string assetsPath
		{
			get;
			private set;
		}

		public static bool hasPro
		{
			get
			{
				if (!EditorUtils._hasCheckedForPro)
				{
					EditorUtils.CheckForPro();
				}
				return EditorUtils._hasPro;
			}
		}

		public static string proVersion
		{
			get
			{
				if (!EditorUtils._hasCheckedForPro)
				{
					EditorUtils.CheckForPro();
				}
				return EditorUtils._proVersion;
			}
		}

		public static string editorADBDir
		{
			get
			{
				if (string.IsNullOrEmpty(EditorUtils._editorADBDir))
				{
					EditorUtils.StoreEditorADBDir();
				}
				return EditorUtils._editorADBDir;
			}
		}

		public static string demigiantDir
		{
			get
			{
				if (string.IsNullOrEmpty(EditorUtils._demigiantDir))
				{
					EditorUtils.StoreDOTweenDirs();
				}
				return EditorUtils._demigiantDir;
			}
		}

		public static string dotweenDir
		{
			get
			{
				if (string.IsNullOrEmpty(EditorUtils._dotweenDir))
				{
					EditorUtils.StoreDOTweenDirs();
				}
				return EditorUtils._dotweenDir;
			}
		}

		public static string dotweenProDir
		{
			get
			{
				if (string.IsNullOrEmpty(EditorUtils._dotweenProDir))
				{
					EditorUtils.StoreDOTweenDirs();
				}
				return EditorUtils._dotweenProDir;
			}
		}

		public static bool isOSXEditor
		{
			get;
			private set;
		}

		public static string pathSlash
		{
			get;
			private set;
		}

		public static string pathSlashToReplace
		{
			get;
			private set;
		}

		static EditorUtils()
		{
			EditorUtils.isOSXEditor = (Application.platform == RuntimePlatform.OSXEditor);
			bool num = Application.platform == RuntimePlatform.WindowsEditor;
			EditorUtils.pathSlash = (num ? "\\" : "/");
			EditorUtils.pathSlashToReplace = (num ? "/" : "\\");
			EditorUtils.projectPath = Application.dataPath;
			EditorUtils.projectPath = EditorUtils.projectPath.Substring(0, EditorUtils.projectPath.LastIndexOf("/"));
			EditorUtils.projectPath = EditorUtils.projectPath.Replace(EditorUtils.pathSlashToReplace, EditorUtils.pathSlash);
			EditorUtils.assetsPath = EditorUtils.projectPath + EditorUtils.pathSlash + "Assets";
		}

		public static void DelayedCall(float delay, Action callback)
		{
			new DelayedCall(delay, callback);
		}

		public static void SetEditorTexture(Texture2D texture, FilterMode filterMode = FilterMode.Point, int maxTextureSize = 32)
		{
			if (texture.wrapMode != TextureWrapMode.Clamp)
			{
				string assetPath = AssetDatabase.GetAssetPath(texture);
				TextureImporter obj = AssetImporter.GetAtPath(assetPath) as TextureImporter;
				obj.textureType = TextureImporterType.GUI;
				obj.npotScale = TextureImporterNPOTScale.None;
				obj.filterMode = filterMode;
				obj.wrapMode = TextureWrapMode.Clamp;
				obj.maxTextureSize = maxTextureSize;
				obj.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				AssetDatabase.ImportAsset(assetPath);
			}
		}

		public static bool DOTweenSetupRequired()
		{
			if (!Directory.Exists(EditorUtils.dotweenDir))
			{
				return false;
			}
			if (Directory.GetFiles(EditorUtils.dotweenDir, "*.addon").Length == 0)
			{
				if (EditorUtils.hasPro)
				{
					return Directory.GetFiles(EditorUtils.dotweenProDir, "*.addon").Length != 0;
				}
				return false;
			}
			return true;
		}

		public static void DeleteOldDemiLibCore()
		{
			string assemblyFilePath = EditorUtils.GetAssemblyFilePath(typeof(DOTween).Assembly);
			string text = (assemblyFilePath.IndexOf("/") != -1) ? "/" : "\\";
			assemblyFilePath = assemblyFilePath.Substring(0, assemblyFilePath.LastIndexOf(text));
			assemblyFilePath = assemblyFilePath.Substring(0, assemblyFilePath.LastIndexOf(text)) + text + "DemiLib";
			string text2 = EditorUtils.FullPathToADBPath(assemblyFilePath);
			if (EditorUtils.AssetExists(text2))
			{
				string text3 = text2 + "/Core";
				if (EditorUtils.AssetExists(text3))
				{
					EditorUtils.DeleteAssetsIfExist(new string[7]
					{
						text2 + "/DemiLib.dll",
						text2 + "/DemiLib.xml",
						text2 + "/DemiLib.dll.mdb",
						text2 + "/Editor/DemiEditor.dll",
						text2 + "/Editor/DemiEditor.xml",
						text2 + "/Editor/DemiEditor.dll.mdb",
						text2 + "/Editor/Imgs"
					});
					if (EditorUtils.AssetExists(text2 + "/Editor") && Directory.GetFiles(assemblyFilePath + text + "Editor").Length == 0)
					{
						AssetDatabase.DeleteAsset(text2 + "/Editor");
						AssetDatabase.ImportAsset(text3, ImportAssetOptions.ImportRecursive);
					}
				}
			}
		}

		private static void DeleteAssetsIfExist(string[] adbFilePaths)
		{
			foreach (string text in adbFilePaths)
			{
				if (EditorUtils.AssetExists(text))
				{
					AssetDatabase.DeleteAsset(text);
				}
			}
		}

		public static bool AssetExists(string adbPath)
		{
			string path = EditorUtils.ADBPathToFullPath(adbPath);
			if (!File.Exists(path))
			{
				return Directory.Exists(path);
			}
			return true;
		}

		public static string ADBPathToFullPath(string adbPath)
		{
			adbPath = adbPath.Replace(EditorUtils.pathSlashToReplace, EditorUtils.pathSlash);
			return EditorUtils.projectPath + EditorUtils.pathSlash + adbPath;
		}

		public static string FullPathToADBPath(string fullPath)
		{
			return fullPath.Substring(EditorUtils.projectPath.Length + 1).Replace("\\", "/");
		}

		public static T ConnectToSourceAsset<T>(string adbFilePath, bool createIfMissing = false) where T : ScriptableObject
		{
			if (!EditorUtils.AssetExists(adbFilePath))
			{
				if (!createIfMissing)
				{
					return null;
				}
				EditorUtils.CreateScriptableAsset<T>(adbFilePath);
			}
			T val = (T)AssetDatabase.LoadAssetAtPath(adbFilePath, typeof(T));
			if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
			{
				EditorUtils.CreateScriptableAsset<T>(adbFilePath);
				val = (T)AssetDatabase.LoadAssetAtPath(adbFilePath, typeof(T));
			}
			return val;
		}

		public static string GetAssemblyFilePath(Assembly assembly)
		{
			string text = Uri.UnescapeDataString(new UriBuilder(assembly.CodeBase).Path);
			if (text.Substring(text.Length - 3) == "dll")
			{
				return text;
			}
			return Path.GetFullPath(assembly.Location);
		}

		private static void CheckForPro()
		{
			EditorUtils._hasCheckedForPro = true;
			try
			{
				EditorUtils._proVersion = (Assembly.Load("DOTweenPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null").GetType("DG.Tweening.DOTweenPro").GetField("Version", BindingFlags.Static | BindingFlags.Public)
					.GetValue(null) as string);
				EditorUtils._hasPro = true;
			}
			catch
			{
				EditorUtils._hasPro = false;
				EditorUtils._proVersion = "-";
			}
		}

		private static void StoreEditorADBDir()
		{
			EditorUtils._editorADBDir = Path.GetDirectoryName(EditorUtils.GetAssemblyFilePath(Assembly.GetExecutingAssembly())).Substring(Application.dataPath.Length + 1).Replace("\\", "/") + "/";
		}

		private static void StoreDOTweenDirs()
		{
			EditorUtils._dotweenDir = Path.GetDirectoryName(EditorUtils.GetAssemblyFilePath(Assembly.GetExecutingAssembly()));
			string text = (EditorUtils._dotweenDir.IndexOf("/") != -1) ? "/" : "\\";
			EditorUtils._dotweenDir = EditorUtils._dotweenDir.Substring(0, EditorUtils._dotweenDir.LastIndexOf(text) + 1);
			EditorUtils._dotweenProDir = EditorUtils._dotweenDir.Substring(0, EditorUtils._dotweenDir.LastIndexOf(text));
			EditorUtils._dotweenProDir = EditorUtils._dotweenProDir.Substring(0, EditorUtils._dotweenProDir.LastIndexOf(text) + 1) + "DOTweenPro" + text;
			EditorUtils._demigiantDir = EditorUtils._dotweenDir.Substring(0, EditorUtils._dotweenDir.LastIndexOf(text));
			EditorUtils._demigiantDir = EditorUtils._demigiantDir.Substring(0, EditorUtils._demigiantDir.LastIndexOf(text) + 1);
			if (EditorUtils._demigiantDir.Substring(EditorUtils._demigiantDir.Length - 10, 9) != "Demigiant")
			{
				EditorUtils._demigiantDir = null;
			}
			EditorUtils._dotweenDir = EditorUtils._dotweenDir.Replace(EditorUtils.pathSlashToReplace, EditorUtils.pathSlash);
			EditorUtils._dotweenProDir = EditorUtils._dotweenProDir.Replace(EditorUtils.pathSlashToReplace, EditorUtils.pathSlash);
			if (EditorUtils._demigiantDir != null)
			{
				EditorUtils._demigiantDir = EditorUtils._demigiantDir.Replace(EditorUtils.pathSlashToReplace, EditorUtils.pathSlash);
			}
		}

		private static void CreateScriptableAsset<T>(string adbFilePath) where T : ScriptableObject
		{
			AssetDatabase.CreateAsset((UnityEngine.Object)(object)ScriptableObject.CreateInstance<T>(), adbFilePath);
		}
	}
}
