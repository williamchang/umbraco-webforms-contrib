/**
@file
    RelationDocumentEvent.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2010-06-09
    - Modified: 2011-07-22
    .
@note
    References:
    - General:
        - http://umbraco.com/help-and-support/video-tutorials/developing-with-umbraco/events/sync-2-websites-with-events-and-the-relationship-api
        .
    .
*/

using System;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.relation;
using umbraco.cms.businesslogic.web;

namespace UmbracoLabs {

/// <summary>Using relation API with Umbraco (CMS) document events.</summary>
/// <remarks>This class inherits from ApplicationBase and is therefore automatically instantiated on application_start.</remarks>
public class RelationDocumentEvent : ApplicationBase
{
    /// <summary>Default constructor.</summary>
    /// <remarks>All events are registered using this constructor.</remarks>
    public RelationDocumentEvent()
    {
        // Subscribe event.
        Document.AfterPublish += new Document.PublishEventHandler(Document_AfterPublish);
        //Document.BeforeMoveToTrash += new Document.MoveToTrashEventHandler(Document_BeforeMoveToTrash);
        Document.AfterMoveToTrash += new Document.MoveToTrashEventHandler(Document_AfterMoveToTrash);
    }

    /// <summary>On event after publishing document.</summary>
    /// <param name="sender">The sender (a documet object).</param>
    /// <param name="e">The <see cref="umbraco.cms.businesslogic.PublishEventArgs"/> instance containing the event data.</param>
    protected void Document_AfterPublish(Document sender, PublishEventArgs e)
    {
        var isCopied = false;

        // Validate document have a relation.
        if(sender.Relations.Length > 0 && sender.Level > 1) {
            foreach(var r in sender.Parent.Relations) {
                // Validate document has been copied by relation type.
                if(r.RelType.Alias == "relateDocumentOnCopy") {
                    isCopied = true;
                    break;
                }
            }
        }
        // Validate document is new.
        if(!isCopied && sender.Level > 1) {
            var parent = new Document(sender.Parent.Id);
            // Validate document's parent have a relation.
            if(parent.Relations.Length > 0) {
                foreach(var r in parent.Relations) {
                    // Validate document's parent is from "Main Site" sender's parent and relation type.
                    if(r.Parent.Id == sender.ParentId && r.RelType.Alias == "relateDocumentOnCopy") {
                        // Copy document (including current data) under parent of related child.
                        sender.Copy(r.Child.Id, sender.User, true);
                        // Append log, audit trail.
                        Log.Add(LogTypes.Copy, sender.Id, String.Format("Copy related document under parent document (name:{0} id:{1})", r.Child.Text, r.Child.Id));
                    }
                }
            }
        }
    }

    /// <summary>On event before moving document to trash.</summary>
    protected void Document_BeforeMoveToTrash(Document sender, MoveToTrashEventArgs e)
    {
        // Future implemetation, ask user for confirmation or give user options.
    }

    /// <summary>On event after moving document to trash.</summary>
    protected void Document_AfterMoveToTrash(Document sender, MoveToTrashEventArgs e)
    {
        // Validate document have a relation and is trashed.
        if(sender.Relations.Length > 0) {
            foreach(var r in sender.Relations) {
                // Validate document is from "Main Site" sender and relation type.
                if(r.Parent.Id == sender.Id && r.RelType.Alias == "relateDocumentOnCopy") {
                    // Get document from relation object.
                    var doc = new Document(r.Child.Id);
                    // Append log, audit trail.
                    Log.Add(LogTypes.Delete, doc.ParentId, String.Format("Document (name:{0} id:{1}) related to document (name:{2} id:{3}) moved to trash.", r.Child.Text, r.Child.Id, sender.Text, sender.Id));
                    // Unpublish and move document (that is child related) to trash.
                    doc.delete();
                }
            }
        }
    }
}

} // END namespace UmbracoLabs