/**
@file
    WebContext.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2011-08-17
    - Modified: 2011-10-20
    .
@note
    References:
    - General:
        - http://www.rosscode.com/blog/index.php?title=using_httpcontext_current_items_effectiv
        - http://our.umbraco.org/forum/developers/razor/21722-Set-Razor-macro-parameters-in-code-behind
        - http://our.umbraco.org/forum/developers/api-questions/5407-Macro-parameters-from-codebehind
        - http://www.delphicsage.com/home/blog.aspx/d=854/title=Code_Expressions_to_Programmaticify_Your_Umbraco_Site
        .
    - AspNet Web Forms Life Cycle:
        - http://blogs.thesitedoctor.co.uk/tim/2006/06/30/Complete+Lifecycle+Of+An+ASPNet+Page+And+Controls.aspx
        - http://www.codeproject.com/KB/aspnet/ASPDOTNETPageLifecycle.aspx
        - http://www.delphicsage.com/home/blog.aspx/d=854/title=Code_Expressions_to_Programmaticify_Your_Umbraco_Site
        .
    .
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using umbraco.MacroEngines;
using UmbracoLabs.Web.Services;

namespace UmbracoLabs.Web.Helpers {

/// <summary>Strongly-typed class for System.Web.HttpContext.Current.Items</summary>
public class WebContext
{

#region Fields

    public const string COOKIE__ScreenType = "screentype";

    public const string TYPEALIAS__Home = "Home";

#endregion

#region Properties

    public static double? UserLatitude
    {
        get {return (double?)HttpContext.Current.Items["WebContext.UserLatitude"];}
        set {HttpContext.Current.Items["WebContext.UserLatitude"] = value;}
    }

    public static double? UserLongitude
    {
        get {return (double?)HttpContext.Current.Items["WebContext.UserLongitude"];}
        set {HttpContext.Current.Items["WebContext.UserLongitude"] = value;}
    }

#endregion

#region State Methods

    /// <summary>Clear state of screen type. Using HTTP cookie.</summary>
    public static bool ClearStateScreenType()
    {
        BaseService.ClearCookie(COOKIE__ScreenType);
        return true;
    }

    /// <summary>Set state of screen type. Using HTTP cookie.</summary>
    public static string GetStateScreenType()
    {
        return BaseService.GetCookie(COOKIE__ScreenType);
    }

    /// <summary>Set state of screen type. Using HTTP cookie.</summary>
    public static bool SetStateScreenType(string value)
    {
        BaseService.ClearCookie(COOKIE__ScreenType);
        if(!String.IsNullOrEmpty(value)) {
            BaseService.SetCookie(COOKIE__ScreenType, value);
            return true;
        }
        return false;
    }

#endregion

}

} // END namespace UmbracoLabs.Web.Helpers