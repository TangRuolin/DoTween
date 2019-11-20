using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
	[CustomEditor(typeof(DOTweenSettings))]
	public class DOTweenSettingsInspector : Editor
	{
		private DOTweenSettings _src;

		private void OnEnable()
		{
			this._src = (base.target as DOTweenSettings);
		}

		public override void OnInspectorGUI()
		{
			GUI.enabled = false;
			base.DrawDefaultInspector();
			GUI.enabled = true;
		}
	}
}
