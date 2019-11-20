using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DG.Tweening.Plugins
{
	public class StringPlugin : ABSTweenPlugin<string, string, StringOptions>
	{
		private static readonly StringBuilder _Buffer = new StringBuilder();

		private static readonly List<char> _OpenedTags = new List<char>();

		public override void SetFrom(TweenerCore<string, string, StringOptions> t, bool isRelative)
		{
			string endValue = t.endValue;
			t.endValue = t.getter();
			t.startValue = endValue;
			t.setter(t.startValue);
		}

		public override void Reset(TweenerCore<string, string, StringOptions> t)
		{
			t.startValue = (t.endValue = (t.changeValue = null));
		}

		public override string ConvertToStartValue(TweenerCore<string, string, StringOptions> t, string value)
		{
			return value;
		}

		public override void SetRelativeEndValue(TweenerCore<string, string, StringOptions> t)
		{
		}

		public override void SetChangeValue(TweenerCore<string, string, StringOptions> t)
		{
			t.changeValue = t.endValue;
			t.plugOptions.startValueStrippedLength = Regex.Replace(t.startValue, "<[^>]*>", "").Length;
			t.plugOptions.changeValueStrippedLength = Regex.Replace(t.changeValue, "<[^>]*>", "").Length;
		}

		public override float GetSpeedBasedDuration(StringOptions options, float unitsXSecond, string changeValue)
		{
			float num = (float)(options.richTextEnabled ? options.changeValueStrippedLength : changeValue.Length) / unitsXSecond;
			if (num < 0f)
			{
				num = 0f - num;
			}
			return num;
		}

		public override void EvaluateAndApply(StringOptions options, Tween t, bool isRelative, DOGetter<string> getter, DOSetter<string> setter, float elapsed, string startValue, string changeValue, float duration, bool usingInversePosition, UpdateNotice updateNotice)
		{
			StringPlugin._Buffer.Remove(0, StringPlugin._Buffer.Length);
			if (isRelative && t.loopType == LoopType.Incremental)
			{
				int num = t.isComplete ? (t.completedLoops - 1) : t.completedLoops;
				if (num > 0)
				{
					StringPlugin._Buffer.Append(startValue);
					for (int i = 0; i < num; i++)
					{
						StringPlugin._Buffer.Append(changeValue);
					}
					startValue = StringPlugin._Buffer.ToString();
					StringPlugin._Buffer.Remove(0, StringPlugin._Buffer.Length);
				}
			}
			int num2 = options.richTextEnabled ? options.startValueStrippedLength : startValue.Length;
			int num3 = options.richTextEnabled ? options.changeValueStrippedLength : changeValue.Length;
			int num4 = (int)Math.Round((double)((float)num3 * EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod)));
			if (num4 > num3)
			{
				num4 = num3;
			}
			else if (num4 < 0)
			{
				num4 = 0;
			}
			if (isRelative)
			{
				StringPlugin._Buffer.Append(startValue);
				if (options.scrambleMode != 0)
				{
					setter(this.Append(changeValue, 0, num4, options.richTextEnabled).AppendScrambledChars(num3 - num4, this.ScrambledCharsToUse(options)).ToString());
				}
				else
				{
					setter(this.Append(changeValue, 0, num4, options.richTextEnabled).ToString());
				}
			}
			else if (options.scrambleMode != 0)
			{
				setter(this.Append(changeValue, 0, num4, options.richTextEnabled).AppendScrambledChars(num3 - num4, this.ScrambledCharsToUse(options)).ToString());
			}
			else
			{
				int num5 = num2 - num3;
				int num6 = num2;
				if (num5 > 0)
				{
					float num7 = (float)num4 / (float)num3;
					num6 -= (int)((float)num6 * num7);
				}
				else
				{
					num6 -= num4;
				}
				this.Append(changeValue, 0, num4, options.richTextEnabled);
				if (num4 < num3 && num4 < num2)
				{
					this.Append(startValue, num4, options.richTextEnabled ? (num4 + num6) : num6, options.richTextEnabled);
				}
				setter(StringPlugin._Buffer.ToString());
			}
		}

		private StringBuilder Append(string value, int startIndex, int length, bool richTextEnabled)
		{
			if (!richTextEnabled)
			{
				StringPlugin._Buffer.Append(value, startIndex, length);
				return StringPlugin._Buffer;
			}
			StringPlugin._OpenedTags.Clear();
			bool flag = false;
			int length2 = value.Length;
			int i;
			for (i = 0; i < length; i++)
			{
				char c = value[i];
				if (c == '<')
				{
					bool flag2 = flag;
					char c2 = value[i + 1];
					flag = (i >= length2 - 1 || c2 != '/');
					if (flag)
					{
						StringPlugin._OpenedTags.Add((c2 == '#') ? 'c' : c2);
					}
					else
					{
						StringPlugin._OpenedTags.RemoveAt(StringPlugin._OpenedTags.Count - 1);
					}
					Match match = Regex.Match(value.Substring(i), "<.*?(>)");
					if (match.Success)
					{
						if (!flag && !flag2)
						{
							char c3 = value[i + 1];
							char[] array = (c3 != 'c') ? new char[1]
							{
								c3
							} : new char[2]
							{
								'#',
								'c'
							};
							int num = i - 1;
							while (num > -1)
							{
								if (value[num] != '<' || value[num + 1] == '/' || Array.IndexOf(array, value[num + 2]) == -1)
								{
									num--;
									continue;
								}
								StringPlugin._Buffer.Insert(0, value.Substring(num, value.IndexOf('>', num) + 1 - num));
								break;
							}
						}
						StringPlugin._Buffer.Append(match.Value);
						int num2 = match.Groups[1].Index + 1;
						length += num2;
						startIndex += num2;
						i += num2 - 1;
					}
				}
				else if (i >= startIndex)
				{
					StringPlugin._Buffer.Append(c);
				}
			}
			if (StringPlugin._OpenedTags.Count > 0 && i < length2 - 1)
			{
				while (StringPlugin._OpenedTags.Count > 0 && i < length2 - 1)
				{
					Match match2 = Regex.Match(value.Substring(i), "(</).*?>");
					if (!match2.Success)
					{
						break;
					}
					if (match2.Value[2] == StringPlugin._OpenedTags[StringPlugin._OpenedTags.Count - 1])
					{
						StringPlugin._Buffer.Append(match2.Value);
						StringPlugin._OpenedTags.RemoveAt(StringPlugin._OpenedTags.Count - 1);
					}
					i += match2.Value.Length;
				}
			}
			return StringPlugin._Buffer;
		}

		private char[] ScrambledCharsToUse(StringOptions options)
		{
			switch (options.scrambleMode)
			{
			case ScrambleMode.Uppercase:
				return StringPluginExtensions.ScrambledCharsUppercase;
			case ScrambleMode.Lowercase:
				return StringPluginExtensions.ScrambledCharsLowercase;
			case ScrambleMode.Numerals:
				return StringPluginExtensions.ScrambledCharsNumerals;
			case ScrambleMode.Custom:
				return options.scrambledChars;
			default:
				return StringPluginExtensions.ScrambledCharsAll;
			}
		}
	}
}
