using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;
using umbraco.NodeFactory;

namespace UmbracoLabs.Web {

public partial class List : UmbracoLabs.Web.BaseUserControl
{

#region Fields

    protected INode _cmsThisNode = null;

#endregion

#region Properties

    public string Header {get;set;}

#endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        if(!Page.IsPostBack) {
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current CMS content item.
        _cmsThisNode = Node.GetCurrent();

        litHeader.Text = Header;
        lisItems.DataSource = _cmsThisNode.ChildrenAsList;
        lisItems.DataBind();

        if(!Page.IsPostBack) {
        }
    }

#region Events

    protected void lisItems_ItemDataBound(Object sender, ListViewItemEventArgs e)
    {
        // This event is raised for the header, the footer, separators, and items.
        if(e.Item.ItemType == ListViewItemType.DataItem) {
            var cmsNode = (Node)e.Item.DataItem;
        }
    }

    protected void lisItems_ItemCommand(Object sender, ListViewCommandEventArgs e)
    {
        switch(e.CommandName) {
            default:
                break;
        }
    }

#endregion

}

}