using DG.DemiEditor;
using UnityEngine;

namespace DG.DOTweenEditor.Core
{
	public class StylePalette
	{
		public class Custom
		{
			public GUIStyle stickyToolbar;

			public GUIStyle stickyTitle;

			/// <summary>
			/// Needs to be overridden in order to initialize new styles added from inherited classes
			/// </summary>
			public override void Init()
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				this.stickyToolbar = new GUIStyle(DeGUI.styles.toolbar.flat);
				this.stickyTitle = GUIStyleExtensions.ContentOffsetX(GUIStyleExtensions.MarginBottom(GUIStyleExtensions.Clone(new GUIStyle(GUI.skin.label), new object[2]
				{
					FontStyle.Bold,
					11
				}), 0), -2f);
			}

			public Custom()
				: this()
			{
			}
		}

		public readonly Custom custom = new Custom();

		public StylePalette()
			: this()
		{
		}
	}
}
