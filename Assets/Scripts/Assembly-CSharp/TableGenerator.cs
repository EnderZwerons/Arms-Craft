using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TableGenerator
{
	public static Dictionary<int, string> GetIndexIntKey(string text)
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		string[] array = text.Split('\r', '\n');
		for (int i = 1; i < array.Length; i++)
		{
			if (string.IsNullOrEmpty(array[i]))
			{
				continue;
			}
			string[] array2 = SplitCsvLine(array[i]);
			for (int j = 0; j < array2.Length; j++)
			{
				if (j != 0)
				{
					dictionary.Add(int.Parse(array2[0]), array2[j]);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<string, Dictionary<string, string>> GetNameTableStringKey(TextAsset text)
	{
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
		string[] array = text.text.Split('\r', '\n');
		string[] array2 = array[0].Split(',');
		for (int i = 1; i < array.Length; i++)
		{
			if (string.IsNullOrEmpty(array[i]))
			{
				continue;
			}
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			string[] array3 = SplitCsvLine(array[i]);
			for (int j = 0; j < array3.Length; j++)
			{
				if (j != 0)
				{
					dictionary2.Add(array2[j], array3[j]);
				}
			}
			dictionary.Add(array3[0], dictionary2);
		}
		return dictionary;
	}

	public static string[] SplitCsvLine(string line)
	{
		return (from Match m in Regex.Matches(line, "(((?<x>(?=[,]+))|\"(?<x>([^\"]|\"\")+)\"|(?<x>[^,\\r\\n]+)),?)", RegexOptions.ExplicitCapture)
			select m.Groups[1].Value).ToArray();
	}
}
