/**
@file
    WebHelper.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2010-07-13
    - Modified: 2011-08-29
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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace UmbracoLabs.Web.Helpers {

public static class WebHelper
{
    /// <summary>Static constructor.</summary>
    static WebHelper() {}

    /// <summary>Add value to HTML element's attribute.</summary>
    /// <remarks>Extension method.</remarks>
    public static void AddAttribute(this System.Web.UI.HtmlControls.HtmlControl ctrl, string attributeName, params string[] args)
    {
        ctrl.Attributes[attributeName] = String.Concat(ctrl.Attributes[attributeName] ?? String.Empty, " ", String.Join(" ", args)).Trim();
    }

    /// <summary>Add value to HTML element's CSS class attribute.</summary>
    /// <remarks>Extension method.</remarks>
    public static void AddCssClass(this System.Web.UI.WebControls.WebControl ctrl, params string[] args)
    {
        ctrl.CssClass = String.Concat(ctrl.CssClass ?? String.Empty, " ", String.Join(" ", args)).Trim();
    }

    /// <summary>Append query string to URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string AppendQueryString(this string url, IDictionary<string, string> parameters)
    {
        if(String.IsNullOrEmpty(url)) {return String.Empty;}

        var sb1 = new StringBuilder(url);
        var sb2 = new StringBuilder();
        var hasSeparator = url.Contains("?");
        if(!hasSeparator) {
            sb1.Append("?");
        }
        foreach(var pair in parameters) {
            if(!String.IsNullOrEmpty(pair.Value)) {
                sb2.AppendFormat("&{0}={1}", EncodeUrl(pair.Key), EncodeUrl(pair.Value));
            }
        }
        if(hasSeparator) {
            sb1.Append(sb2.ToString());
        } else {
            sb1.Append(sb2.ToString().Substring(1));
        }
        return sb1.ToString();
    }

    /// <summary>Create attribute markup code inside an element for Razor.</summary>
    public static System.Web.HtmlString CreateAttributeMarkup(string attributeName, string attributeValue)
    {
        if(!String.IsNullOrEmpty(attributeValue)) {
            return String.Concat(attributeName, "=\"", attributeValue, "\"").ToHtmlRaw();
        }
        return ToHtmlRaw(String.Empty);
    }

    /// <summary>Create options markup code for select (drop-down list).</summary>
    public static string CreateOptionsMarkup(IDictionary<string, string> options, string keySelected)
    {
        var sb1 = new StringBuilder();
        var htmlSelected = String.Empty;
        foreach(var pair in options) {
            if(pair.Key.Equals(keySelected)) {
                htmlSelected = " selected=\"selected\"";
            } else {
                htmlSelected = String.Empty;
            }
            sb1.AppendFormat("<option value=\"{0}\"{1}>{2}</option>", pair.Key, htmlSelected, pair.Value);
        }
        return sb1.ToString();
    }

    /// <summary>Converts a string to an HTML encoded string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string EncodeHtml(this string s)
    {
        return System.Web.HttpUtility.HtmlEncode(s);
    }

    /// <summary>Converts a string to an URL encoded string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string EncodeUrl(this string s)
    {
        return System.Web.HttpUtility.UrlEncode(s);
    }

    /// <summary>Searches the collection for a property that contains the specified text.</summary>
    /// <remarks>Extension method.</remarks>
    public static void FindByTextFromList(this System.Web.UI.WebControls.ListItemCollection list, string value)
    {
        var item = list.FindByText(value);
        if(item != null) {
            item.Selected = true;
        }
    }

    /// <summary>Searches the collection for a property that contains the specified value.</summary>
    /// <remarks>Extension method.</remarks>
    public static void FindByValueFromList(this System.Web.UI.WebControls.ListItemCollection list, string value)
    {
        var item = list.FindByValue(value);
        if(item != null) {
            item.Selected = true;
        }
    }

    /// <summary>Converts a string that has been HTML encoded for HTTP transmission into a decoded string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string DecodeHtml(this string s)
    {
        return System.Web.HttpUtility.HtmlDecode(s);
    }

    /// <summary>Converts a string that has been encoded for transmission in a URL into a decoded string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string DecodeUrl(this string s)
    {
        return System.Web.HttpUtility.UrlDecode(s);
    }

    /// <summary>Get attribute value from HTML (markup).</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetHtmlAttributeValue(this string html, string attributeName)
    {
        var result = String.Empty;
        var attribute = String.Format("{0}=\"", attributeName);
        var indexCharacter = html.IndexOf(attribute);
        if(indexCharacter > 0) {
            var s = html.Substring(indexCharacter + attribute.Length);
            indexCharacter = s.IndexOf("\"");
            if(indexCharacter > 0) {
                return s.Substring(0, indexCharacter);
            }
        }
        return String.Empty;
    }

    /// <summary>Get HTTP response as string.</summary>
    public static string GetHttpResponseAsString(Uri requestUri)
    {
        // Validate argument.
        if(requestUri == null) {throw new ArgumentNullException("requestUri");}
        // Declare for response.
        var result = String.Empty;
        // Create request.
        var httpRequest = System.Net.WebRequest.Create(requestUri) as System.Net.HttpWebRequest;
        // Get response.
        using(var httpResponse = httpRequest.GetResponse() as System.Net.HttpWebResponse) {
            // Get response stream.
            var reader = new System.IO.StreamReader(httpResponse.GetResponseStream());
            // Read the stream and return as a string.
            result = reader.ReadToEnd();
        }
        return result;
    }

    /// <summary>Get page name for CSS class.</summary>
    public static string GetPageNameForCssClass(string pageName)
    {
        return pageName.RemoveWhitespaces().StripSpecialCharacters();
    }

    /// <summary>Get query string. Using current HTTP request.</summary>
    public static string GetQueryString(string key)
    {
        return DecodeUrl(System.Web.HttpContext.Current.Request.QueryString[EncodeUrl(key)]);
    }

    /// <summary>Get this page's url. Using current HTTP request.</summary>
    /// <remarks>
    /// References:
    /// http://timstall.dotnetdevelopersjournal.com/understanding_httprequest_urls.htm
    /// http://www.pluralsight.com/blogs/keith/archive/2007/02/16/46127.aspx
    /// </remarks>
    public static string GetThisUrl()
    {
        return System.Web.HttpContext.Current.Request.Path;
    }

    /// <summary>Get this page's absolute url without query string. Using current HTTP request.</summary>
    public static string GetThisUrlAbsolute()
    {
        var url = System.Web.HttpContext.Current.Request.Url.ToString();
        int index = url.LastIndexOf('?');
        if(index >= 0) {
            return url.Substring(0, index);
        } else {
            return url;
        }
    }

    /// <summary>Get value from HTTP web request.</summary>
    /// <remarks>First check form and then query string.</remarks>
    public static string GetValue(this System.Web.HttpRequest request, string parameter)
    {
        var val = request.Form[parameter];
        if(String.IsNullOrEmpty(val)) {
            val = request.QueryString[parameter];
        }
        if(String.IsNullOrEmpty(val)) {
            return String.Empty;
        } else {
            return val;
        }
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

    /// <summary>Parse URL into a NameValueCollection using UTF8 encoding.</summary>
    public static NameValueCollection ParseQueryStringByUrl(string url)
    {
        var qs = url.Substring(url.IndexOf('?'));
        return System.Web.HttpUtility.ParseQueryString(qs);
    }

    /// <summary>Remove query string from URL.</summary>
    public static string RemoveQueryString(string url)
    {
        if(url.Contains('?')) {
            return url.Remove(url.IndexOf('?'));
        } else {
            return url;
        }
    }

    /// <summary>To dictionary (no duplicate keys, order by insert).</summary>
    /// <remarks>Extension method.</remarks>
    public static OrderedDictionary ToDictionaryUnique(this NameValueCollection parameters)
    {
        var items = new OrderedDictionary();
        foreach(string key in parameters.Keys) {
            items.Add(key, parameters[key]);
        }
        return items;
    }

    /// <summary>Toggle value from delimited list. If value does not exist, append value. If value exist, then remove value.</summary>
    public static string ToggleDelimitedValue(string[] tokens, string value, char separator, out bool selected)
    {
        var items = tokens.ToList();
        if(items.Remove(value)) {
            selected = true;
        } else {
            items.Add(value);
            selected = false;
        }
        return String.Join(Convert.ToString(separator), items);
    }

    /// <summary>To System.Web.HtmlString for Razor.</summary>
    /// <remarks>Extension method.</remarks>
    public static System.Web.HtmlString ToHtmlRaw(this string str)
    {
        return new System.Web.HtmlString(str);
    }

    /// <summary>To System.Web.HtmlString and replace newlines for Razor.</summary>
    /// <remarks>Extension method.</remarks>
    public static System.Web.HtmlString ToHtmlRawAndReplaceNewlines(this string str, string replace)
    {
        return new System.Web.HtmlString(str.ReplaceNewlines(replace));
    }

    /// <summary>To HTML. Using String.Format and replacing two single quotes with double quotes character.</summary>
    /// <remarks>Extension method.</remarks>
    public static System.Web.UI.WebControls.Literal ToHtml(this System.Web.UI.WebControls.Literal ctrl, params Object[] args)
    {
        var s = ctrl.Text.Replace("''", "\"");
        ctrl.Text =  String.Format(s, args);
        return ctrl;
    }

    /// <summary>To HTML. Using String.Format and replacing two single quotes with double quotes character.</summary>
    /// <remarks>Extension method.</remarks>
    public static System.Web.UI.WebControls.Literal ToHtml(this System.Web.UI.WebControls.Literal ctrl, bool isVisible, params Object[] args)
    {
        var s = ctrl.Text.Replace("''", "\"");
        ctrl.Visible = isVisible;
        ctrl.Text =  String.Format(s, args);
        return ctrl;
    }

    /// <summary>To query string with URL.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToQueryStringWithUrl(this OrderedDictionary parameters, string url)
    {
        var items = new List<string>(parameters.Count);
        var itemKey = String.Empty;
        var itemValue = String.Empty;

        foreach(System.Collections.DictionaryEntry pair in parameters) {
            itemKey = pair.Key as string;
            itemValue = pair.Value as string;
            if(!String.IsNullOrEmpty(itemValue)) {
                items.Add(String.Concat(EncodeUrl(itemKey), "=", EncodeUrl(itemValue)));
            }
        }
        if(items.Count > 0) {
            return String.Concat(RemoveQueryString(url), "?", String.Join("&", items.ToArray()));
        } else {
            return url;
        }
    }
}

} // END namespace UmbracoLabs.Web.Helpers