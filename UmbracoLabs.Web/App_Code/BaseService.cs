using System;
using System.Linq;
using System.Text;
using System.Web;
using umbraco.MacroEngines;
using umbraco.presentation.umbracobase;
using UmbracoLabs.Web.Helpers;

namespace UmbracoLabs.Web.Services {

/// <summary>Base Service</summary>
public class BaseService
{

#region Properties

    /// <summary>Current HTTP request (sent from the browser to server).</summary>
    protected static HttpRequest HttpRequest
    {
        get {return HttpContext.Current != null ? HttpContext.Current.Request : null;}
    }

    /// <summary>Current HTTP response (sent from server to the browser.).</summary>
    protected static HttpResponse HttpResponse
    {
        get {return HttpContext.Current != null ? HttpContext.Current.Response : null;}
    }

#endregion

#region Helper Methods

    /// <summary>Get value from HTTP cookie.</summary>
    public static string GetCookie(string name) {
        var cookie = HttpRequest.Cookies[name];
        if(cookie != null && !String.IsNullOrEmpty(cookie.Value)) {
            return HttpUtility.HtmlEncode(cookie.Value);
        } else {
            return null;
        }
    }

    /// <summary>Set value to HTTP non-persistent cookie.</summary>
    public static bool SetCookie(string name, string value)
    {
        HttpResponse.Cookies[name].Value = value;
        HttpResponse.Cookies[name].Expires = DateTime.MinValue;
        return true;
    }

    /// <summary>Set value to HTTP persistent cookie.</summary>
    public static bool SetCookie(string name, string value, DateTime expire)
    {
        HttpResponse.Cookies[name].Value = value;
        HttpResponse.Cookies[name].Expires = expire;
        return true;
    }

    /// <summary>Clear HTTP cookie.</summary>
    public static bool ClearCookie(string name)
    {
        HttpResponse.Cookies[name].Value = null;
        HttpResponse.Cookies[name].Expires = DateTime.Now.AddYears(-1);
        return true;
    }

#endregion

#region Debug Methods

    /// <summary>Debug HTTP Handler.</summary>
    /// <example>http://localhost/Base/Geolocation/Debug/WilliamChang.aspx</example>
    [RestExtensionMethod()]
    public static string Debug(string input)
    {
        // Set HTTP web response.
        HttpResponse.Clear();
        HttpResponse.Cache.SetCacheability(HttpCacheability.Public);
        HttpResponse.Cache.SetExpires(DateTime.MinValue);
        HttpResponse.Cache.SetNoStore();
        HttpResponse.ContentType = "text/plain";
        HttpResponse.ContentEncoding = System.Text.Encoding.UTF8;

        System.Diagnostics.Debug.Write(input, "UmbracoLabs.Web.Services.GeolocationService");
        return String.Format("Debug: {0}", input);
    }

#endregion

}

} // END namespace UmbracoLabs.Web.Services