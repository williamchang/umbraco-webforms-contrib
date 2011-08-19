/**
@file
    CmsHttpModule.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2011-08-17
    - Modified: 2011-08-18
    .
@note
    References:
    - General:
        - http://forum.umbraco.org/yaf_postst726.aspx
        - http://our.umbraco.org/forum/developers/api-questions/3732-IHttpModule-and-Umbraco
        - http://msdn.microsoft.com/en-us/library/ms227673.aspx
        - http://www.kowitz.net/archive/2006/03/08/ihttpmodule-vs-ihttphandler
        .
    - Examples:
        - http://stackoverflow.com/questions/792851/how2-what-event-to-hook-in-httpmodule-for-putting-js-links-into-head-element
        - http://stackoverflow.com/questions/3675469/http-module-and-cookies-in-sharepoint-2007
        .
    .

    Register module in the web.config file:

    To register the module for IIS 6.0 and IIS 7.0 running in Classic mode
    <configuration>
      <system.web>
        <httpModules>
          <add name="CmsHttpModule" type="UmbracoLabs.Web.CmsHttpModule, UmbracoLabs.Web"/>
         </httpModules>
      </system.web>
    </configuration>

    To register the module for IIS 7.0 running in Integrated mode
    <configuration>
      <system.webServer>
        <modules>
          <add name="CmsHttpModule" type="UmbracoLabs.Web.CmsHttpModule, UmbracoLabs.Web"/>
        </modules>
      </system.webServer>
    </configuration>
*/

using System.Web;
using umbraco.MacroEngines;
using UmbracoLabs.Web.Helpers;

namespace UmbracoLabs.Web {

/// <summary>HttpModule for System.Web.HttpContext.Current.Items and Umbraco (CMS)</summary>
public class CmsHttpModule : IHttpModule
{

#region Fields

    protected DynamicNode _cmsThisNode = null;
    protected DynamicNode _cmsHomeNode = null;

#endregion

#region IHttpModule Members

    /// <summary>Initializes a module and prepares it to handle requests.</summary>
    public void Init(HttpApplication app)
    {
        app.PreRequestHandlerExecute += new System.EventHandler(app_PreRequestHandlerExecute);
    }

    /// <summary>Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.</summary>
    public void Dispose() {}

#endregion

#region Events

    protected void app_PreRequestHandlerExecute(object sender, System.EventArgs e)
    {
        var page = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
        if(page != null) {
            page.PreInit += new System.EventHandler(page_PreInit);
        }
    }

    void page_PreInit(object sender, System.EventArgs e)
    {
        var page = sender as System.Web.UI.Page;
        if(page != null) {
        }
    }

#endregion

}

} // END namespace UmbracoLabs.Web