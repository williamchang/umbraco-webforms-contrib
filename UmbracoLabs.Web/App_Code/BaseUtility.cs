/**
@file
    BaseUtility.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2010-06-09
    - Modified: 2011-08-19
    .
@note
    References:
    - General:
        - Nothing.
        .
    .
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UmbracoLabs {

public static class BaseUtility
{
    /// <summary>Static constructor.</summary>
    static BaseUtility() {}

    /// <summary>Encodes a string to be represented as a string literal. The format is essentially a JSON string.</summary>
    /// <remarks>http://www.west-wind.com/weblog/posts/2007/Jul/14/Embedding-JavaScript-Strings-from-an-ASPNET-Page</remarks>
    public static string EncodeJsonString(string s, bool includeOuterQuotes = false)
    {
        var sb1 = new StringBuilder();
        if(includeOuterQuotes) {sb1.Append("\"");}
        foreach(char c in s) {
            switch(c) {
                case '\"':
                    sb1.Append("\\\"");
                    break;
                case '\\':
                    sb1.Append("\\\\");
                    break;
                case '\b':
                    sb1.Append("\\b");
                    break;
                case '\f':
                    sb1.Append("\\f");
                    break;
                case '\n':
                    sb1.Append("\\n");
                    break;
                case '\r':
                    sb1.Append("\\r");
                    break;
                case '\t':
                    sb1.Append("\\t");
                    break;
                default:
                    int i = (int)c;
                    if(i < 32 || i > 127) {
                        sb1.AppendFormat("\\u{0:X04}", i);
                    } else {
                        sb1.Append(c);
                    }
                    break;
            }
        }
        if(includeOuterQuotes) {sb1.Append("\"");}
        return sb1.ToString();
    }

    /// <summary>Determines whether two specified System.String objects have the same value. Compare strings ignoring the case of the strings being compared.</summary>
    public static bool Equals(string s1, string s2)
    {
        return String.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Find key by value.</summary>
    /// <remarks>Extension method.</remarks>
    public static TKey FindKey<TKey, TValue>(this IDictionary<TKey, TValue> list, TValue value, TKey defaultValue)
    {
        foreach(var pair in list) {
            if(pair.Value.Equals(value)) {
                return pair.Key;
            }
        }
        return defaultValue;
    }

    /// <summary>Get string of JSON array.</summary>
    public static string GetJsonArray(this string[] items)
    {
        if(items != null) {
            var sb1 = new StringBuilder();
            for(int i = 0;i < items.Length;i += 1) {
                sb1.AppendFormat(", \"{0}\"", items[i]);
            }
            return String.Concat("[", sb1.ToString().Substring(2), "]");
        }
        return String.Empty;
    }

    /// <summary>Get date string for JSON object.</summary>
    public static String GetJsonDate(DateTime date)
    {
        return date.Year.ToString() + "/" + date.Month.ToString() + "/" + date.Day.ToString();
    }

    /// <summary>Get string of JSON object.</summary>
    /// <remarks>http://www.blog.activa.be/2007/08/12/WritingAFullJSONSerializerIn100LinesOfCCode.aspx</remarks>
    public static string GetJsonObject(Object obj)
    {
        if(obj is DBNull) {
            return "null";
        } else if(obj is sbyte || obj is byte || obj is short || obj is ushort || obj is int || obj is uint || obj is long || obj is ulong || obj is decimal || obj is double || obj is float) {
            return Convert.ToString(obj, System.Globalization.NumberFormatInfo.InvariantInfo);
        } else if(obj is bool) {
            return obj.ToString().ToLower();
        } else if(obj is char || obj is Enum || obj is Guid) {
            return "" + obj;
        } else if(obj is DateTime) {
            return "\"" + GetJsonDate((DateTime)obj) + "\"";
        } else if(obj is String) {
            return "\"" + obj.ToString() + "\"";
        } else {
            return obj.ToString();
        }
    }

    /// <summary>Get list of months.</summary>
    public static IDictionary<string, string> GetMonths(bool useMonthNameForKey, IFormatProvider culture)
    {
        var items = new Dictionary<string, string>();
        var dt1 = new DateTime(1900, 1, 1);
        var month = String.Empty;
        for(int i = 0;i < 12;i += 1) {
            month = dt1.AddMonths(i).ToString("MMMM", culture);
            if(useMonthNameForKey) {
                items.Add(month, month);
            } else {
                items.Add((i + 1).ToString(), month);
            }
        }
        return items;
    }

    /// <summary>Get object's property value by name.</summary>
    /// <remarks>
    /// References:
    /// http://stackoverflow.com/questions/2535287/getting-nested-object-property-value-using-reflection
    /// http://stackoverflow.com/questions/987982/c-how-can-i-get-the-value-of-a-string-property-via-reflection
    /// </remarks>
    public static T GetPropertyValue<T>(Object obj, string propertyName)
    {
        return (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
    }

    /// <summary>Get list of numbers between a range (include endpoints).</summary>
    public static IDictionary<string, string> GetRange(int minimum, int maximum)
    {
        var items = new Dictionary<string, string>();
        var s = String.Empty;
        for(int i = minimum;i <= maximum;i += 1) {
            s = i.ToString();
            items.Add(s, s);
        }
        return items;
    }

    /// <summary>Get subvalue from on array of names (e.g. Foobar1, Foobar2) in resource.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetResourceSubvalue(this System.Resources.ResourceManager rm, string format, string subname)
    {
        if(String.IsNullOrEmpty(subname)) {return null;}

        var repeat = true;
        var index = 0;
        string value = null;
        string[] tokens = null;

        while(repeat) {
            value = rm.GetString(String.Format(format, index));
            if(value != null) {
                tokens = value.Split('=');
                if(tokens.Length > 0 && BaseUtility.Equals(tokens[0], subname)) {
                    value = tokens[1];
                    repeat = false;
                } else {
                    index++;
                }
            } else {
                repeat = false;
            }
        }
        return value;
    }

    /// <summary>Get string in between two strings.</summary>
    /// <remarks>http://www.mycsharpcorner.com//Post.aspx?postID=15</remarks>
    /// <returns>An array of System.String instance containing the string in the middle.</returns>
    public static string[] GetStringInBetween(string strBegin, string strEnd, string strSource, bool includeBegin, bool includeEnd)
    {
        string[] result = {String.Empty, String.Empty};
        int indexBegin = strSource.IndexOf(strBegin);
        if(indexBegin != -1) {
            // Include the begin string if desired.
            if(includeBegin) {
                indexBegin -= strBegin.Length;
            }
            strSource = strSource.Substring(indexBegin + strBegin.Length);
            int indexEnd = strSource.IndexOf(strEnd);
            if(indexEnd != -1) {
                // Include the End string if desired
                if(includeEnd) {
                    indexEnd += strEnd.Length;
                }
                result[0] = strSource.Substring(0, indexEnd);
                // Advance beyond this segment.
                if(indexEnd + strEnd.Length < strSource.Length) {
                    result[1] = strSource.Substring(indexEnd + strEnd.Length);
                }
            }
        } else {
            // Stay where we are.
            result[1] = strSource;
        }
        return result;
    }

    /// <summary>Is value equal to one of the listed values.</summary>
    /// <remarks>Extension method.</remarks>
    public static bool IsEqual(this string s1, params string[] tokens)
    {
        if(!String.IsNullOrEmpty(s1)) {
            for(int i = 0;i < tokens.Length;i += 1) {
                if(BaseUtility.Equals(s1, tokens[i])) {return true;}
            }
        }
        return false;
    }

    /// <summary>Is client input valid.</summary>
    public static bool IsInputValid(string clientInput)
    {
        if(clientInput.Contains('<') || clientInput.Contains('>')) {
            return false;
        } else {
            return true;
        }
    }

    /// <summary>Indicates whether the specified string is null or an System.String.Empty string.</summary>
    public static bool IsNullOrStringEmpty(this Object value)
    {
        return value != null ? String.IsNullOrEmpty(Convert.ToString(value)) : false;
    }

    /// <summary>Remove extra whitespaces from string. (Another regular expression: @"\s{2,}" pattern.)</summary>
    /// <remarks>
    /// Extension method.
    /// 
    /// References:
    /// http://nlakkakula.wordpress.com/2008/09/16/removing-additional-white-spaces-in-sentence-c/
    /// http://authors.aspalliance.com/stevesmith/articles/removewhitespace.asp
    /// </remarks>
    public static string RemoveExtraWhitespaces(this string source)
    {
        return System.Text.RegularExpressions.Regex.Replace(source.Trim(), @"\s+", " ");
    }

    /// <summary>Remove string in between two strings.</summary>
    /// <remarks>http://www.mycsharpcorner.com//Post.aspx?postID=15</remarks>
    /// <returns>An array of System.String instance containing the string in the middle.</returns>
    public static string RemoveStringInBetween(string strBegin, string strEnd, string strSource, bool removeBegin, bool removeEnd)
    {
        string[] result = GetStringInBetween(strBegin, strEnd, strSource, removeBegin, removeEnd);
        if(result[0] != String.Empty) {
            return strSource.Replace(result[0], String.Empty);
        }
        // Return if nothing found between begin and end.
        return strSource;
    }

    /// <summary>Remove tabs from string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string RemoveTabs(this string source)
    {
        return source.Replace("\t", String.Empty);
    }

    /// <summary>Remove all whitespaces from string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string RemoveWhitespaces(this string source)
    {
        return source.Trim().Replace(" ", String.Empty);
    }

    /// <summary>Replace diacritics (e.g. accents) with normal english characters.</summary>
    /// <remarks>
    /// Extension method.
    /// 
    /// References:
    /// http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
    /// </remarks>
    public static string ReplaceDiacritics(this string s)
    {
        var norm = s.Normalize(NormalizationForm.FormD);
        var sb1 = new StringBuilder();

        for(int i = 0;i < norm.Length;i += 1) {
            var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(norm[i]);
            if(uc != System.Globalization.UnicodeCategory.NonSpacingMark) {
                sb1.Append(norm[i]);
            }
        }
        return (sb1.ToString().Normalize(NormalizationForm.FormC));
    }

    /// <summary>Replace HTML line breaks with value.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ReplaceLineBreaks(this string source, string value)
    {
        return source.Replace("<br/>", value).Replace("<br />", value).Replace("<br>", value);
    }

    /// <summary>Replace escape '\n' (newline) and '\r' (carriage return) with text before and text after.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ReplaceNewlines(this string source, string textBefore, string textAfter)
    {
        var sb1 = new StringBuilder();
        var tokens = source.SplitClean("\r\n", "\r", "\n");
        for(int i = 0;i < tokens.Length;i += 1) {
            sb1.Append(textBefore);
            sb1.Append(tokens[i]);
            sb1.Append(textAfter);
        }
        return sb1.ToString();
    }

    /// <summary>Replace escape '\n' (newline) and '\r' (carriage return) with value.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ReplaceNewlines(this string source, string value)
    {
        return source.Replace("\r\n", value).Replace("\r", value).Replace("\n", value);
    }

    /// <summary>Set time in DateTime.</summary>
    /// <remarks>Extension method.</remarks>
    public static DateTime SetTime(this DateTime dt, int hours, int minutes, int seconds, int milliseconds)
    {
        return new DateTime(
            dt.Year,
            dt.Month,
            dt.Day,
            hours,
            minutes,
            seconds,
            milliseconds,
            dt.Kind
        );
    }

    /// <summary>Split string to array by delimiter.</summary>
    /// <remarks>Extension method.</remarks>
    public static string[] SplitClean(this string s, params char[] separator)
    {
        return s.RemoveWhitespaces().Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>Split string to array by delimiter.</summary>
    /// <remarks>Extension method.</remarks>
    public static string[] SplitClean(this string s, params string[] separator)
    {
        return s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>Shorten string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ShortenString(this string s, int limit, string markEnd)
    {
        if(s.Length > limit) {
            return s.Substring(0, limit) + markEnd;
        } else {
            return s;
        }
    }

    /// <summary>Strip (remove) all HTML from string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string StripHtml(this string s)
    {
        return System.Text.RegularExpressions.Regex.Replace(s, @"<(.|\n)*?>", String.Empty);
    }

    /// <summary>Strip (remove) all non-numeric from string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string StripNonnumeric(this string s)
    {
        return System.Text.RegularExpressions.Regex.Replace(s, @"[^0-9]", String.Empty);
    }

    /// <summary>Strip (remove) all special characters from string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string StripSpecialCharacters(this string s)
    {
        return System.Text.RegularExpressions.Regex.Replace(s, @"[^a-zA-Z0-9 ]", String.Empty);
    }

    /// <summary>Converts the specified string representation of a logical value to its System.Boolean equivalent.</summary>
    /// <remarks>Extension method.</remarks>
    public static bool ToBoolean(this string str)
    {
        return str.ToNullableBoolean() ?? false;
    }

    /// <summary>Parse UTC timestamp string to DateTime.</summary>
    /// <remarks>Extension method.</remarks>
    public static DateTime ToDateTime(this string s)
    {
        string dayOfWeek = s.Substring(0, 3).Trim();
        string month = s.Substring(4, 3).Trim();
        string dayInMonth = s.Substring(8, 2).Trim();
        string time = s.Substring(11, 9).Trim();
        string offset = s.Substring(20, 5).Trim();
        string year = s.Substring(25, 5).Trim();
        string dateTime = String.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);
        return DateTime.Parse(dateTime);
    }

    /// <summary>To GUID.</summary>
    /// <remarks>Extension method.</remarks>
    public static Guid ToGuid(this string value)
    {
        if(!String.IsNullOrEmpty(value)) {
            return new Guid(value);
        } else {
            return Guid.Empty;
        }
    }

    /// <summary>Converts the specified string representation of a logical value to its System.Boolean equivalent.</summary>
    /// <remarks>Extension method.</remarks>
    public static bool? ToNullableBoolean(this string str)
    {
        if(!String.IsNullOrEmpty(str)) {
            if(String.Equals(str, "1")) {return true;}
            if(String.Equals(str, "0")) {return false;}

            // Converts the specified string representation of a logical value to its System.Boolean equivalent.
            bool value;
            if(Boolean.TryParse(str, out value)) {return value;}
        }
        return null;
    }

    /// <summary>Converts the string representation of a number to its 32-bit signed integer equivalent.</summary>
    /// <remarks>Extension method.</remarks>
    public static int? ToNullableInt32(this string str)
    {
        int value;
        if(Int32.TryParse(str, out value)) {return value;}
        return null;
    }

    /// <summary>To SEO (Search Engine Optimization)’.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToSeoName(this string s)
    {
        return s.Trim().ToLower()
            .ReplaceDiacritics()
            .Replace("-", " ")
            .Replace("& ", String.Empty)
            .StripSpecialCharacters()
            .Replace(" ", "-");
    }

    /// <summary>Converts the value of the specified object to its equivalent string representation.</summary>
    /// <returns>The string representation of value, or System.String.Empty if value is null.</returns>
    public static string ToString(Object value, string defaultValue)
    {
        if(value != null) {
            var s = Convert.ToString(value);
            if(!String.IsNullOrEmpty(s)) {
                return s;
            }
        }
        return defaultValue;
    }

    /// <summary>Convert string (from request form) to type. If null, return default value.</summary>
    /// <remarks>Extension method.</remarks>
    public static T ToTypeOrDefault<T>(this string value, T defaultValue) where T : struct
    {
        if(!String.IsNullOrEmpty(value)) {
            return (T)Convert.ChangeType(value.Trim(), typeof(T));
        } else {
            return defaultValue;
        }
    }

    /// <summary>Validate string only. If null or empty, return default value.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToTypeOrDefault(this string value, string defaultValue)
    {
        if(!String.IsNullOrEmpty(value)) {
            return value;
        } else {
            return defaultValue;
        }
    }
}

} // END namespace UmbracoLabs