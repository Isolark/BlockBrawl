using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ProfanityFilter
{
    private static List<string> CensoredWordList;
    private static readonly string EN_PROFANE_JSON_PATH = "Data/en_profanity_list";

    public static void Initialize()
    {
        var censoredWordStr = Resources.Load(EN_PROFANE_JSON_PATH).ToString().Replace("[", "").Replace("]", "").Replace("\"", "");
        CensoredWordList = censoredWordStr.Split(',').ToList();
    }

    public static string ToCensoredStr(this string str)
    {
        foreach (var censoredWord in CensoredWordList)
        {
            var regularExpression = ToRegexPattern(censoredWord);
            str = Regex.Replace(str, regularExpression, StarCensoredMatch,
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
        return str;
    }

    private static string StarCensoredMatch(Match m)
    {
        string word = m.Captures[0].Value;
        return new string('*', word.Length);
    }

    private static string ToRegexPattern(this string str)
    {
        string regexPattern = Regex.Escape(str);

        regexPattern = regexPattern.Replace(@"\*", ".*?");
        regexPattern = regexPattern.Replace(@"\?", ".");

        if (regexPattern.StartsWith(".*?"))
        {
            regexPattern = regexPattern.Substring(3);
            regexPattern = @"(^\b)*?" + regexPattern;
        }

        regexPattern = @"\b" + regexPattern + @"\b";

        return regexPattern;
    }
}