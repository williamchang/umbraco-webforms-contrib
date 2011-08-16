using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;
using umbraco.NodeFactory;
using UmbracoLabs;
using UmbracoLabs.Web.Helpers;

namespace UmbracoLabs.Web.Templates {

public partial class Barebone : UmbracoLabs.Web.BaseMasterPage
{

#region Fields

    protected INode _cmsThisNode = null;
    protected INode _cmsHomeNode = null;

#endregion

#region Properties

    public string HtmlCssClass {get;set;}

    public string HtmlBodyCssClass {get;set;}

    public string CmsHomeTypeAlias {get;set;}

#endregion

    /// <summary>Default constructor.</summary>
    public Barebone()
    {
        HtmlCssClass = String.Empty;
        HtmlBodyCssClass = String.Empty;
        CmsHomeTypeAlias = "Home";
    }

    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current CMS content item.
        _cmsThisNode = Node.GetCurrent();

        // Get home from current CMS content item.
        _cmsHomeNode = _cmsThisNode.ToDynamicNode().AncestorOrSelf(CmsHomeTypeAlias).ToNode();

        // Process HTML title.
        if(_cmsThisNode != null && _cmsThisNode.Level == 1) {
            plhTitleHome.Visible = true;
            plhTitleInterior.Visible = false;
        } else {
            plhTitleHome.Visible = false;
            plhTitleInterior.Visible = true;
        }

        // Process HTML script of global variables.
        if(_cmsThisNode != null && _cmsHomeNode != null) {
            SetScriptGlobal();
        }

        // Process HTML markup element (aka <html></html>).
        if(!String.IsNullOrEmpty(HtmlCssClass)) {
            html.Attributes["class"] = HtmlCssClass;
        }

        // Process HTML body.
        var pageNameCssClass = Page.GetType().Name.Replace("_aspx", String.Empty).Replace('_', '-');
        if(_cmsThisNode != null) {
            var cmsHtmlBodyCssClass = _cmsThisNode.GetPropertyValue("htmlBodyCssClass");
            if(cmsHtmlBodyCssClass != null) {
                HtmlBodyCssClass = String.Concat(HtmlBodyCssClass, " ", cmsHtmlBodyCssClass).Trim();
            } else if(String.IsNullOrEmpty(HtmlBodyCssClass)) {
                HtmlBodyCssClass = _cmsThisNode.Name.ToSeoName();
            }
        } else if(String.IsNullOrEmpty(HtmlBodyCssClass)) {
            HtmlBodyCssClass = pageNameCssClass;
        }
        body.Attributes["class"] = String.Concat(body.Attributes["class"] ?? String.Empty, " ", HtmlBodyCssClass).Trim();
    }

    /// <summary>Set HTML script of global variables.</summary>
    public void SetScriptGlobal()
    {
        litScriptGlobalHtml.Text = String.Format(
            @"
            <script type=""text/javascript"">
            //<![CDATA[
            window.cms = {{
                homeId:'{0}',
                homeName:'{1}',
                pageId:'{2}',
                pageName:'{3}'
            }};
            //]]>
            </script>
            ",
            _cmsHomeNode.Id,
            _cmsHomeNode.Name,
            _cmsThisNode.Id,
            _cmsThisNode.Name
        );
    }
}

}