using DG.DOTweenEditor.Core;
using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
	public class UtilityWindowProcessor : AssetPostprocessor
	{
		private static bool _setupDialogRequested;

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (!UtilityWindowProcessor._setupDialogRequested && Array.FindAll(importedAssets, delegate(string name)
			{
				if (name.Contains("DOTween") && !name.EndsWith(".meta") && !name.EndsWith(".jpg"))
				{
					return !name.EndsWith(".png");
				}
				return false;
			}).Length != 0)
			{
				EditorUtils.DeleteOldDemiLibCore();
				if (EditorUtils.DOTweenSetupRequired() && (EditorPrefs.GetString(Application.dataPath + "DOTweenVersion") != Application.dataPath + DOTween.Version || EditorPrefs.GetString(Application.dataPath + "DOTweenProVersion") != Application.dataPath + EditorUtils.proVersion))
				{
					UtilityWindowProcessor._setupDialogRequested = true;
					EditorPrefs.SetString(Application.dataPath + "DOTweenVersion", Application.dataPath + DOTween.Version);
					EditorPrefs.SetString(Application.dataPath + "DOTweenProVersion", Application.dataPath + EditorUtils.proVersion);
					EditorUtility.DisplayDialog("DOTween", "DOTween needs to be setup.\n\nSelect \"Tools > DOTween Utility Panel\" and press \"Setup DOTween...\" in the panel that opens.", "Ok");
					if (Convert.ToInt32(Application.unityVersion.Split("."[0])[0]) >= 4)
					{
						EditorUtils.DelayedCall(0.5f, DOTweenUtilityWindow.Open);
					}
					EditorUtils.DelayedCall(8f, delegate
					{
						UtilityWindowProcessor._setupDialogRequested = false;
					});
				}
			}
		}
	}
}
