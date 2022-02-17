using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public static class Extensions
    {
        public static string RemoveQuotes(this string oneFieldSource)
        {
            //var replaced = oneFieldSource.ReplaceFirstOccurrence("\"", "");
            //replaced = replaced.ReplaceLastOccurrence("\"", "");
            if (oneFieldSource.IsNullOrEmpty()) return oneFieldSource;
            var replaced = oneFieldSource.RemoveIfEndWith("\"");
            replaced = replaced.RemoveIfStartWith("\"");
            return replaced;
        }
        public static string FormatDateItemID(this string oneFieldSource)
        {
            //var replaced = oneFieldSource.ReplaceFirstOccurrence("\"", "");
            //replaced = replaced.ReplaceLastOccurrence("\"", "");
            if (oneFieldSource.IsNullOrEmpty()) return oneFieldSource;
            //var replaced = oneFieldSource.RemoveIfEndWith("\"");
            var replaced = oneFieldSource.ReplaceFirstOccurrence("{","");
            return replaced.Trim();
        }
        public static int IndexOfAvoidQuotes(this string str,char toSearch,int startIndex=0)
        {
            if (str.IsNullOrEmpty()) return -1;
            bool inQuotes = false;char activeQuote = Char.MinValue;
            for(int i = 0; i < str.Length; i++)
            {
                if (inQuotes)
                {
                    if (str[i] == activeQuote)
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    if (str[i] == '\'' || str[i] == '\"')
                    {
                        inQuotes = true;
                        activeQuote = str[i];
                    }
                    if (str[i] == toSearch && i>= startIndex)
                    {
                        return i;
                    }
                }

            }
            return -1;
        }
        public static int IndexOfWholeWord(this string str, string toSearch, int startIndex)
        {
            for (int j = startIndex;
                j < str.Length && (j = str.IndexOf(toSearch, j, StringComparison.OrdinalIgnoreCase)) >= 0;
                j++)
            {
                bool StartMatch = (j == startIndex || !char.IsLetterOrDigit(str, j - 1));
                bool EndMatch = (j + toSearch.Length == str.Length || !char.IsLetterOrDigit(str, j + toSearch.Length));
                if (StartMatch && EndMatch)
                {
                    return j;
                }
            }
            return -1;
        }
        public static int IndexOfAny(this string str, int startIndex,ref string foundStr, params string[] values)
        {
            var first = -1;
            foreach(var item in values)
            {
                int i = str.IndexOfWholeWord(item, startIndex);
                if (i >= 0)
                {
                    if (first > 0)
                    {
                        if (i < first)
                        {
                            first = i;
                            foundStr = item;
                        }
                    }
                    else
                    {
                        first = i;
                        foundStr = item;
                    }
                }
            }
            return first;
        }
        public static List<string> SplitAndKeepDelimiters(this string s, params string[] delimiters)
        {
            var parts = new List<string>();
            if (!s.IsNullOrEmpty())
            {
                int iFirst = 0;
                do
                {
                    var foundStr = "";
                    int iLast = s.IndexOfAny(iFirst,ref foundStr, delimiters);
                    if (iLast >= 0)
                    {
                        if (iLast > iFirst)
                        {
                            parts.Add(s.Substring(iFirst, iLast - iFirst));//expr
                        }
                        parts.Add(new string(s[iLast], foundStr.Length));//delimiter
                        iFirst = iLast + foundStr.Length;
                        continue;
                    }
                    parts.Add(s.Substring(iFirst, s.Length - iFirst));
                    break;
                } while (iFirst < s.Length);
            }
            return parts;
        }
        public static bool IsNullOrEmpty(this string[] value)
        {
            if (value == null) return true;
            return (value.Length == 0);
        }
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }
            return source.Any() == false;
        }
        public static bool IsNotNullOrEmpty(this string[] value)
        {
            return !value.IsNullOrEmpty();
        }
        public static bool IsNullOrEmpty(this string value)
        {
            //return value == null || value == string.Empty;
            return String.IsNullOrEmpty(value);
        }
        public static string FormatEndOfElement(this string str)
        {
            if (str.EndsWith("}"))
            {
                str = str.ReplaceLastOccurrence("}", "");
            }
            if (str.EndsWith(";"))
            {
                str = str.ReplaceLastOccurrence(";", "");
            }
            return str;
            //return str.Replace(";", "").Replace("}", "");
        }
        public static bool IsNotNullOrEmpty(this string value)
        {
            //return value == null || value == string.Empty;
            return !String.IsNullOrEmpty(value);
        }
        /// <summary>
        /// Count of start spaces
        /// </summary>
        public static int StartSpacesCount(this string value)
        {

                int spacesCount = 0;
                for (int i = 0; i < value.Length; i++)
                    if (value[i] == ' ')
                        spacesCount++;
                    else
                        break;
                return spacesCount;
            
        }
        public static string RemoveAtBorders(this string value, string removeAtStart,string removeAtEnd)
        {
            var rep = value.RemoveIfStartWith(removeAtStart);
            rep = rep.RemoveIfEndWith(removeAtEnd);
            return rep;
        }
        public static bool EqualsIgnoreCase(this string value,string test)
        {
            return value.Equals(test, StringComparison.OrdinalIgnoreCase);
        }
        public static string[] SplitLines(this string oneFieldSource)
        {
            return oneFieldSource.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
        public static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
        {
            if (Source.IsNullOrEmpty()) return Source;
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
        public static string RemoveIfEndWith(this string Source, string Find)
        {
            if (Source.EndsWith(Find))
            {
                return Source.ReplaceLastOccurrence(Find, "");
            }
            return Source;
        }
        public static string RemoveIfStartWith(this string Source, string Find)
        {
            if (Source.StartsWith(Find))
            {
                return Source.ReplaceFirstOccurrence(Find, "");
            }
            return Source;
        }
        public static string ReplaceFirstOccurrence(this string Source, string Find, string Replace)
        {
            if (Source.IsNullOrEmpty()) return Source;
            int place = Source.IndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
        public static int FirstOccurrence(this string Source, string Find)
        {
            return Source.IndexOf(Find);
        }
        public static int LastOccurrence(this string Source, string Find)
        {
            return Source.LastIndexOf(Find);
        }
        public static int IndexOfLastChar(this string Source, string Find)
        {
            return Source.IndexOf(Find) + Find.Length ;
        }
    }
}
