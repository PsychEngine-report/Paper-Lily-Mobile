using System;
using System.Collections.Generic;
using System.Globalization;

namespace LacieEngine.Core
{
	public static class StringExtension
	{
		private static readonly string[] StringLineSplit = new string[3] { "\r\n", "\r", "\n" };

		private static readonly TextInfo TextInfo = new CultureInfo("en-US", useUserOverride: false).TextInfo;

		public static bool In(this string str, params string[] list)
		{
			return list.Contains(str);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static bool IsQuotedText(this string str)
		{
			if (str.StartsWith("\""))
			{
				return str.EndsWith("\"");
			}
			return false;
		}

		public static string StripPrefix(this string str, string prefix)
		{
			if (!str.StartsWith(prefix))
			{
				return str;
			}
			int length = prefix.Length;
			return str.Substring(length, str.Length - length);
		}

		public static string StripSuffix(this string str, string suffix)
		{
			if (!str.EndsWith(suffix))
			{
				return str;
			}
			return str.Substring(0, str.LastIndexOf(suffix));
		}

		public static string StripEdges(this string str, string edges)
		{
			return str.StripPrefix(edges).StripSuffix(edges);
		}

		public static string ToPascalCase(this string str)
		{
			return TextInfo.ToTitleCase(str).Replace(" ", string.Empty);
		}

		public static string[] SplitLines(this string str, bool keepEmpty = false)
		{
			string[] array = str.Split(StringLineSplit, StringSplitOptions.None);
			List<string> tempList = new List<string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string trimmedStr = array2[i].Trim();
				if (keepEmpty || !(trimmedStr == string.Empty))
				{
					tempList.Add(trimmedStr);
				}
			}
			return tempList.ToArray();
		}

		public static int[] SplitInts(this string str, char separator)
		{
			string[] array = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			List<int> output = new List<int>();
			string[] array2 = array;
			foreach (string tempItem in array2)
			{
				if (int.TryParse(tempItem.Trim(), out var number))
				{
					output.Add(number);
					continue;
				}
				Log.Warn("SplitInts: Invalid string to be converted to a number: ", tempItem);
			}
			return output.ToArray();
		}

		public static float[] SplitFloats(this string str, char separator)
		{
			string[] array = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			List<float> output = new List<float>();
			string[] array2 = array;
			foreach (string tempItem in array2)
			{
				if (float.TryParse(tempItem.Trim(), out var number))
				{
					output.Add(number);
					continue;
				}
				Log.Warn("SplitFloats: Invalid string to be converted to a number: ", tempItem);
			}
			return output.ToArray();
		}
	}
}
