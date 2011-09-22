/**
@file
    RelationStaticBackofficeEvent.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2011-08-12
    - Modified: 2011-09-20
    .
@note
    References:
    - General:
        - http://our.umbraco.org/wiki/reference/backoffice-apis/tree-api-to-create-custom-treesapplications
        - http://our.umbraco.org/wiki/reference/api-cheatsheet/using-applicationbase-to-register-events/event-examples
        .
    .
*/

using System;
using System.Collections.Generic;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.relation;
using umbraco.cms.businesslogic.web;
using umbraco.cms.presentation.Trees;
using UmbracoLabs.Web.Helpers;

namespace UmbracoLabs {

/// <summary>Using document API and context menu API with Umbraco (CMS) backoffice events.</summary>
/// <remarks>This class inherits from ApplicationBase and is therefore automatically instantiated on application_start.</remarks>
public class RelationStaticBackofficeEvent : ApplicationBase
{

#region Fields

    public const string TYPEALIAS__HomeLocationFolder = "HomeLocationFolder";

    public const string PROPERTYALIAS__RelationStaticId = "relationStaticId";

#endregion

    /// <summary>Default constructor.</summary>
    /// <remarks>All events are registered using this constructor.</remarks>
    public RelationStaticBackofficeEvent()
    {
        // Subscribe event to context menu.
        BaseTree.BeforeNodeRender += new umbraco.cms.presentation.Trees.BaseTree.BeforeNodeRenderEventHandler(BaseTree_BeforeNodeRender);

        // Subscribe event.
        Document.New += new Document.NewEventHandler(Document_New);
        Document.AfterCopy += new Document.CopyEventHandler(Document_AfterCopy);
        //Document.BeforeSave += new Document.SaveEventHandler(Document_BeforeSave);
    }

#region Backoffice Events

    /// <summary>On event, before node render for context menu.</summary>
    protected void BaseTree_BeforeNodeRender(ref umbraco.cms.presentation.Trees.XmlTree sender, ref umbraco.cms.presentation.Trees.XmlTreeNode node, EventArgs e)
    {
        if(String.Equals(node.NodeType, "content") && node.Menu != null) {
            int index = node.Menu.FindIndex(x => String.Equals(x.Alias, "publish")) + 1;

            node.Menu.Insert(index + 0, umbraco.BusinessLogic.Actions.ContextMenuSeperator.Instance);
            node.Menu.Insert(index + 1, new OverrideAllAction());
            node.Menu.Insert(index + 2, umbraco.BusinessLogic.Actions.ContextMenuSeperator.Instance);
        }
    }

    /// <summary>On event, new document.</summary>
    protected void Document_New(Document sender, NewEventArgs e)
    {
        SetRelationStaticId(sender);
    }

    /// <summary>On event, after copy.</summary>
    protected void Document_AfterCopy(Document sender, CopyEventArgs e)
    {
        SetRelationStaticId(e.NewDocument);
    }

    /// <summary>On event, before save.</summary>
    void Document_BeforeSave(Document sender, SaveEventArgs e)
    {
        throw new NotImplementedException();
    }

#endregion

#region Static Methods

    /// <summary>Get documents by property relation static id. Documents must be published. Using CMS cache.</summary>
    public static IList<Document> GetDocumentsByPropertyRelationStaticId(Document cmsDocument)
    {
        IList<Document> items = new List<Document>();
        var cmsRootNode = CmsHelper.GetRootItem();
        var relationStaticIdProperty = cmsDocument.getProperty(PROPERTYALIAS__RelationStaticId);

        if(cmsRootNode != null && relationStaticIdProperty != null) {
            var cmsNodes = cmsRootNode.DescendantsOrSelf(cmsDocument.ContentType.Alias).Items;
            var cmsNodesCount = cmsNodes.Count;

            for(int i = 0;i < cmsNodesCount;i += 1) {
                if(cmsDocument.Id != cmsNodes[i].Id && String.Equals(cmsNodes[i].GetPropertyValue(PROPERTYALIAS__RelationStaticId), relationStaticIdProperty.Value)) {
                    items.Add(new Document(cmsNodes[i].Id));
                }
            }
        }
        return items;
    }

    /// <summary>Set relation static id (if property exist) and not part of the location subtree.</summary>
    public static Document SetRelationStaticId(Document cmsDocument)
    {
        var relationStaticIdProperty = cmsDocument.getProperty(PROPERTYALIAS__RelationStaticId);
        if(relationStaticIdProperty != null) {
            var cmsCurrentPath = cmsDocument.Path;
            // Validate path for document type alias.
            if(CmsDocumentHelper.GetFirstDescendant(cmsCurrentPath, TYPEALIAS__HomeLocationFolder) == null) {
                relationStaticIdProperty.Value = Guid.NewGuid().ToString();
            }
        }
        return cmsDocument;
    }

#endregion

#region Presentation Models

    public class OverrideAllAction : umbraco.interfaces.IAction
    {
        public string Alias {get {return "Location Override All";}}

        /// <summary>Icon for context menu.</summary>
        /// <summary>Examples in file cms.dll, namespace umbraco.BusinessLogic.Actions</summary>
        public string Icon
        {
            get {return ".sprCopy";}
        }

        public string JsFunctionName
        {
            get {return "openModal('dialogs/LocationOverrideAll.aspx?id=' + nodeID, 'Location Override All', 540, 280);";}
        }

        public string JsSource {get {return String.Empty;}}

        public char Letter {get {return '@';}}

        public bool ShowInNotifier {get {return false;}}

        public bool CanBePermissionAssigned {get {return true;}}
    }

#endregion

}

} // END namespace UmbracoLabs