using DG.Tweening.Core.Enums;

namespace DG.Tweening.Core
{
	public static class Extensions
	{
		internal static T SetSpecialStartupMode<T>(this T t, SpecialStartupMode mode) where T : Tween
		{
			((Tween)(object)t).specialStartupMode = mode;
			return t;
		}

		internal static TweenerCore<T1, T2, TPlugOptions> NoFrom<T1, T2, TPlugOptions>(this TweenerCore<T1, T2, TPlugOptions> t) where TPlugOptions : struct
		{
			t.isFromAllowed = false;
			return t;
		}

		internal static TweenerCore<T1, T2, TPlugOptions> Blendable<T1, T2, TPlugOptions>(this TweenerCore<T1, T2, TPlugOptions> t) where TPlugOptions : struct
		{
			t.isBlendable = true;
			return t;
		}
	}
}
