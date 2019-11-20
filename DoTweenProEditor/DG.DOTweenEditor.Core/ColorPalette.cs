using DG.DemiLib;
using System;
using UnityEngine;

namespace DG.DOTweenEditor.Core
{
	[Serializable]
	public class ColorPalette
	{
		/// <summary>
		/// Custom colors
		/// </summary>
		[Serializable]
		public class Custom
		{
			public DeSkinColor stickyDivider = new DeSkinColor(Color.black, new Color(0.5f, 0.5f, 0.5f));
		}

		public Custom custom = new Custom();

		public ColorPalette()
			: this()
		{
		}
	}
}
