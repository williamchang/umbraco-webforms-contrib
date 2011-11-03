/**
@file
    PageCmsEvent.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2011-10-18
    - Modified: 2011-10-26
    .
@note
    References:
    - General:
        - http://our.umbraco.org/forum/templating/templates-and-document-types/19543-Switching-between-templates-at-runtime
        - http://our.umbraco.org/forum/templating/templates-and-document-types/24455-mobiletablet-detection-with-51degreesmobi-an-alternative-approach
        .
    .
*/

using System;
using System.Collections.Generic;
using System.Web;
using umbraco;
using umbraco.BusinessLogic;
using UmbracoLabs.Web.Helpers;

namespace UmbracoLabs {

/// <summary>Hooks for Umbraco (CMS) page events.</summary>
/// <remarks>This class inherits from ApplicationBase and is therefore automatically instantiated on application_start.</remarks>
public class PageCmsEvent : ApplicationBase
{
    /// <summary>Default constructor.</summary>
    /// <remarks>All events are registered using this constructor.</remarks>
    public PageCmsEvent()
    {
        // Subscribe event.
        UmbracoDefault.BeforeRequestInit += new UmbracoDefault.RequestInitEventHandler(UmbracoDefault_BeforeRequestInit);
    }

    /// <summary>On event, before page request initialized for rendering.</summary>
    protected void UmbracoDefault_BeforeRequestInit(Object sender, RequestInitEventArgs e)
    {
        var httpRequest = HttpContext.Current.Request;
        var page = (UmbracoDefault)sender;
        var qsScreenType = httpRequest.QueryString["screentype"];
        var currentScreenType = WebContext.GetStateScreenType();
        var saveState = true;

        // Validate state.
        if(String.IsNullOrEmpty(qsScreenType) && String.IsNullOrEmpty(currentScreenType)) {
            // Validate browser.
            if(httpRequest.Browser.IsMobileDevice || httpRequest.UserAgent.ToLower().IndexOf("android") >= 0) {
                currentScreenType = "mobile";
            } else {
                currentScreenType = "desktop";
            }
        } else if(!String.IsNullOrEmpty(qsScreenType)) {
            // Validate HTTP query string.
            switch(qsScreenType) {
                case "mobile":
                    currentScreenType = "mobile";
                    break;
                case "desktop_temp":
                    currentScreenType = "desktop";saveState = false;
                    break;
                case "desktop":
                default:
                    currentScreenType = "desktop";
                    break;
            }

        }

        // Set state.
        if(saveState) {WebContext.SetStateScreenType(currentScreenType);}

        // Set main template (aka master page).
        try {
            switch(currentScreenType) {
                case "mobile":
                    page.MasterPageFile = template.GetMasterPageName(e.Page.Template, "Mobile");
                    break;
                case "desktop":
                default:
                    // Do nothing.
                    break;
            }
        } catch(System.IO.DirectoryNotFoundException) {
            // Do nothing.
        }
    }
}

} // END namespace UmbracoLabs