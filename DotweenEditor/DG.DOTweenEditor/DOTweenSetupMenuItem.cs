using DG.DOTweenEditor.Core;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
	internal class DOTweenSetupMenuItem
	{
		private const string _Title = "DOTween Setup";

		private static Assembly _proEditorAssembly;

		public static void Setup(bool partiallySilent = false)
		{
			if (EditorUtils.DOTweenSetupRequired())
			{
				string message = "Based on your Unity version (" + Application.unityVersion + ") and eventual plugins, DOTween will now activate additional tween elements, if available.";
				if (!EditorUtility.DisplayDialog("DOTween Setup", message, "Ok", "Cancel"))
				{
					return;
				}
			}
			else
			{
				if (partiallySilent)
				{
					return;
				}
				string message2 = "This project has already been setup for your version of DOTween.\nReimport DOTween if you added new compatible external assets or upgraded your Unity version.";
				if (!EditorUtility.DisplayDialog("DOTween Setup", message2, "Force Setup", "Cancel"))
				{
					return;
				}
			}
			string dotweenDir = EditorUtils.dotweenDir;
			string dotweenProDir = EditorUtils.dotweenProDir;
			EditorUtility.DisplayProgressBar("DOTween Setup", "Please wait...", 0.25f);
			int num = 0;
			string[] array = Application.unityVersion.Split("."[0]);
			int num2 = Convert.ToInt32(array[0]);
			int num3 = Convert.ToInt32(array[1]);
			if (num2 < 4)
			{
				DOTweenSetupMenuItem.SetupComplete(dotweenDir, dotweenProDir, num);
			}
			else
			{
				if (num2 == 4)
				{
					if (num3 < 3)
					{
						DOTweenSetupMenuItem.SetupComplete(dotweenDir, dotweenProDir, num);
						return;
					}
					num += DOTweenSetupMenuItem.ImportAddons("43", dotweenDir);
					if (num3 >= 6)
					{
						num += DOTweenSetupMenuItem.ImportAddons("46", dotweenDir);
					}
				}
				else
				{
					num += DOTweenSetupMenuItem.ImportAddons("43", dotweenDir);
					num += DOTweenSetupMenuItem.ImportAddons("46", dotweenDir);
					num += DOTweenSetupMenuItem.ImportAddons("50", dotweenDir);
				}
				if (EditorUtils.hasPro)
				{
					if (DOTweenSetupMenuItem.Has2DToolkit())
					{
						num += DOTweenSetupMenuItem.ImportAddons("Tk2d", dotweenProDir);
						DOTweenSetupMenuItem.ProEditor_AddGlobalDefine("DOTWEEN_TK2D");
					}
					else
					{
						DOTweenSetupMenuItem.ProEditor_RemoveGlobalDefine("DOTWEEN_TK2D");
					}
					if (DOTweenSetupMenuItem.HasTextMeshPro())
					{
						num += DOTweenSetupMenuItem.ImportAddons("TextMeshPro", dotweenProDir);
						DOTweenSetupMenuItem.ProEditor_AddGlobalDefine("DOTWEEN_TMP");
					}
					else
					{
						DOTweenSetupMenuItem.ProEditor_RemoveGlobalDefine("DOTWEEN_TMP");
					}
				}
				DOTweenSetupMenuItem.SetupComplete(dotweenDir, dotweenProDir, num);
			}
		}

		private static void SetupComplete(string addonsDir, string proAddonsDir, int totImported)
		{
			int num = 0;
			string[] files = Directory.GetFiles(addonsDir, "*.addon");
			if (files.Length != 0)
			{
				EditorUtility.DisplayProgressBar("DOTween Setup", "Removing " + files.Length + " unused additional files...", 0.5f);
				string[] array = files;
				foreach (string path in array)
				{
					num++;
					File.Delete(path);
				}
			}
			if (EditorUtils.hasPro)
			{
				files = Directory.GetFiles(proAddonsDir, "*.addon");
				if (files.Length != 0)
				{
					EditorUtility.DisplayProgressBar("DOTween Setup", "Removing " + files.Length + " unused additional files...", 0.5f);
					string[] array = files;
					foreach (string path2 in array)
					{
						num++;
						File.Delete(path2);
					}
				}
			}
			files = Directory.GetFiles(addonsDir, "*.addon.meta");
			if (files.Length != 0)
			{
				EditorUtility.DisplayProgressBar("DOTween Setup", "Removing " + files.Length + " unused additional meta files...", 0.75f);
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					File.Delete(array[i]);
				}
			}
			if (EditorUtils.hasPro)
			{
				files = Directory.GetFiles(proAddonsDir, "*.addon.meta");
				if (files.Length != 0)
				{
					EditorUtility.DisplayProgressBar("DOTween Setup", "Removing " + files.Length + " unused additional meta files...", 0.75f);
					string[] array = files;
					for (int i = 0; i < array.Length; i++)
					{
						File.Delete(array[i]);
					}
				}
			}
			EditorUtility.DisplayProgressBar("DOTween Setup", "Refreshing...", 0.9f);
			AssetDatabase.Refresh();
			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("DOTween Setup", "DOTween setup is now complete." + ((totImported == 0) ? "" : ("\n" + totImported + " additional libraries were imported or updated.")) + ((num == 0) ? "" : ("\n" + num + " extra files were removed.")), "Ok");
		}

		private static int ImportAddons(string version, string addonsDir)
		{
			bool flag = false;
			string[] array = new string[4]
			{
				"DOTween" + version + ".dll",
				"DOTween" + version + ".xml",
				"DOTween" + version + ".dll.mdb",
				"DOTween" + version + ".cs"
			};
			foreach (string str in array)
			{
				string text = addonsDir + str + ".addon";
				string text2 = addonsDir + str;
				if (File.Exists(text))
				{
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
					File.Move(text, text2);
					flag = true;
				}
			}
			if (!flag)
			{
				return 0;
			}
			return 1;
		}

		private static bool Has2DToolkit()
		{
			string[] directories = Directory.GetDirectories(EditorUtils.projectPath, "TK2DROOT", SearchOption.AllDirectories);
			if (directories.Length == 0)
			{
				return false;
			}
			string[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				if (Directory.GetFiles(array[i], "tk2dSprite.cs", SearchOption.AllDirectories).Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		private static bool HasTextMeshPro()
		{
			string[] directories = Directory.GetDirectories(EditorUtils.projectPath, "TextMesh Pro", SearchOption.AllDirectories);
			if (directories.Length == 0)
			{
				return false;
			}
			string[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				if (Directory.GetFiles(array[i], "TextMeshPro.cs", SearchOption.AllDirectories).Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		private static Assembly ProEditorAssembly()
		{
			if (DOTweenSetupMenuItem._proEditorAssembly == null)
			{
				DOTweenSetupMenuItem._proEditorAssembly = Assembly.LoadFile(EditorUtils.dotweenProDir + "Editor" + EditorUtils.pathSlash + "DOTweenProEditor.dll");
			}
			return DOTweenSetupMenuItem._proEditorAssembly;
		}

		private static void ProEditor_AddGlobalDefine(string id)
		{
			if (EditorUtils.hasPro && DOTweenSetupMenuItem.ProEditorAssembly() != null)
			{
				DOTweenSetupMenuItem._proEditorAssembly.GetType("DG.DOTweenEditor.Core.ProEditorUtils").GetMethod("AddGlobalDefine", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[1]
				{
					id
				});
			}
		}

		private static void ProEditor_RemoveGlobalDefine(string id)
		{
			if (EditorUtils.hasPro && DOTweenSetupMenuItem.ProEditorAssembly() != null)
			{
				DOTweenSetupMenuItem._proEditorAssembly.GetType("DG.DOTweenEditor.Core.ProEditorUtils").GetMethod("RemoveGlobalDefine", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[1]
				{
					id
				});
			}
		}
	}
}
