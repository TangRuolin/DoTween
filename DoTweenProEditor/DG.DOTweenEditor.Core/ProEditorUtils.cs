using System;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor.Core
{
	public static class ProEditorUtils
	{
		public static void AddGlobalDefine(string id)
		{
			bool flag = false;
			foreach (BuildTargetGroup value in Enum.GetValues(typeof(BuildTargetGroup)))
			{
				if (value != 0)
				{
					string scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(value);
					if (!scriptingDefineSymbolsForGroup.Contains(id))
					{
						flag = true;
						scriptingDefineSymbolsForGroup += ((scriptingDefineSymbolsForGroup.Length > 0) ? (";" + id) : id);
						PlayerSettings.SetScriptingDefineSymbolsForGroup(value, scriptingDefineSymbolsForGroup);
					}
				}
			}
			if (flag)
			{
				Debug.Log("DOTween : added global define " + id);
			}
		}

		public static void RemoveGlobalDefine(string id)
		{
			bool flag = false;
			foreach (BuildTargetGroup value in Enum.GetValues(typeof(BuildTargetGroup)))
			{
				if (value != 0)
				{
					string scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(value);
					if (scriptingDefineSymbolsForGroup.Contains(id))
					{
						flag = true;
						scriptingDefineSymbolsForGroup = ((!scriptingDefineSymbolsForGroup.Contains(id + ";")) ? ((!scriptingDefineSymbolsForGroup.Contains(";" + id)) ? scriptingDefineSymbolsForGroup.Replace(id, "") : scriptingDefineSymbolsForGroup.Replace(";" + id, "")) : scriptingDefineSymbolsForGroup.Replace(id + ";", ""));
						PlayerSettings.SetScriptingDefineSymbolsForGroup(value, scriptingDefineSymbolsForGroup);
					}
				}
			}
			if (flag)
			{
				Debug.Log("DOTween : removed global define " + id);
			}
		}
	}
}
