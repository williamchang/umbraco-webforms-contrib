using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace UmbracoLabs.Web.Templates {

public partial class GoogleSitemap : UmbracoLabs.Web.BaseMasterPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set HTTP web response.
        Response.Clear();
        Response.Cache.SetCacheability(HttpCacheability.Public);
        Response.Cache.SetExpires(DateTime.MinValue);
        Response.Cache.SetNoStore();
        Response.ContentType = "text/plain";
        Response.ContentEncoding = System.Text.Encoding.UTF8;

        var requestUrl = HttpContext.Current.Request.Url;
        var baseUrl = String.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Host, requestUrl.IsDefaultPort ? String.Empty : ":" + requestUrl.Port);

        // Init XML writer.
        XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8);
        writer.Formatting = Formatting.Indented;

        // Begin XML document.
        writer.WriteStartDocument();
        writer.WriteStartElement("urlset");
        writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

        // End XML document.
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
    }

    private void AppendUrlElement(XmlTextWriter writer, string url, string lastModified, string changeFrequency, string priority)
    {
        writer.WriteStartElement("url");
        writer.WriteElementString("loc", url);
        writer.WriteElementString("lastmod", lastModified);
        writer.WriteElementString("changefreq", changeFrequency);
        writer.WriteElementString("priority", priority);
        writer.WriteEndElement();
    }

    public string GetLastModifiedDate(DateTime dateModified)
    {
        try {
            return dateModified.ToString("yyyy-MM-ddThh:mm:sszzzz");
        } catch {
            return String.Empty; 
        }
    }

    public string GetChangeFrequency(DateTime lastModified)
    {
        var defaultValue = "weekly";
        try {
            var interval = DateTime.Today.Subtract(lastModified).Hours; 
 
            if(interval < 1) {
                return "hourly";
            } else if(interval <= 24 & interval > 1) {
                return "daily";
            } else if(interval <= 168 & interval > 24) {
                return "weekly";
            } else if(interval <= 672 & interval > 168) {
                return "monthly";
            } else if(interval <= 8766 & interval > 672) {
                return "yearly";
            }
            return defaultValue;
        }
        catch {
            return defaultValue;
        }
    }
}

}