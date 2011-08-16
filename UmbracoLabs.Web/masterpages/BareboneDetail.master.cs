using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UmbracoLabs.Web.Templates {

public partial class BareboneDetail : UmbracoLabs.Web.BaseMasterPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.HtmlBodyCssClass = litHtmlBodyCssClass.Text;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }
}

}