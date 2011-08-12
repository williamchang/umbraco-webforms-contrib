/**
@file
    CmsDocumentHelper.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2011-08-03
    - Modified: 2011-08-12
    .
@note
    References:
    - General:
        - Nothing.
        .
    .
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.relation;
using umbraco.cms.businesslogic.web;

namespace UmbracoLabs.Web.Helpers {

public static class CmsDocumentHelper
{
    /// <summary>Static constructor.</summary>
    static CmsDocumentHelper() {}

    /// <summary>Copy document properties to another document.</summary>
    public static void CopyDocumentProperties(Document cmsSourceDocument, Document cmsTargetDocument)
    {
        // Validate dependencies.
        if(cmsSourceDocument != null && cmsTargetDocument != null && String.Equals(cmsSourceDocument.ContentType.Alias, cmsTargetDocument.ContentType.Alias)) {
            var sourceProperties = cmsSourceDocument.GenericProperties;
            var sourcePropertiesCount = sourceProperties.Count;
            var targetProperties = cmsTargetDocument.GenericProperties;
            var targetPropertiesCount = sourceProperties.Count;

            if(sourcePropertiesCount != targetPropertiesCount) {
                throw new ArrayTypeMismatchException("Properties count is not equal between cmsSourceDocument and cmsTargetDocument");
            }
            for(int i = 0;i < sourcePropertiesCount;i += 1) {
                if(String.Equals(targetProperties[i].PropertyType.Alias, sourceProperties[i].PropertyType.Alias)) {
                    targetProperties[i].Value = sourceProperties[i].Value;
                } else {
                    throw new ArrayTypeMismatchException("PropertyType.Alias mismatch between cmsSourceDocument and cmsTargetDocument");
                }
            }
        }
    }

    /// <summary>Copy document under another document.</summary>
    public static void CopyDocument(Document cmsSourceDocument, int cmsTargetDocumentId, bool relateToOriginal = false)
    {
        // Validate dependencies.
        if(cmsSourceDocument != null && cmsTargetDocumentId >= 0) {
            cmsSourceDocument.Copy(cmsTargetDocumentId, cmsSourceDocument.User, relateToOriginal);
        }
    }

    /// <summary>Create or get document if exist.</summary>
    public static Document CreateOrGetDocument(string newName, DocumentType newType, Document parent)
    {
        // Get document and validate existence.
        var cmsDocument = GetDocumentsByParentAndName(parent, newName, true).FirstOrDefault();
        if(cmsDocument != null) {
            return cmsDocument;
        } else {
            // Create document.
            return Document.MakeNew(newName, newType, parent.User, parent.Id);
        }
    }

    /// <summary>Create a relation using alias relateDocumentOnCopy. Both document type alias must be the same.</summary>
    public static void CreateRelation(Document cmsParent, Document cmsChild)
    {
        var relationType = RelationType.GetByAlias("relateDocumentOnCopy");
        if(!Relation.IsRelated(cmsParent.Id, cmsChild.Id, relationType) && String.Equals(cmsParent.ContentType.Alias, cmsChild.ContentType.Alias)) {
            Relation.MakeNew(cmsParent.Id, cmsChild.Id, relationType, "");
        }
    }

    /// <summary>Get documents by parent and name.</summary>
    public static IList<Document> GetDocumentsByParentAndName(Document cmsParent, string name, bool getFirst = false)
    {
        IList<Document> items = new List<Document>();

        if(cmsParent != null) {
            var cmsItems = cmsParent.Children;
            var cmsItemsCount = cmsItems.Length;

            for(int i = 0;i < cmsItemsCount;i += 1) {
                if(BaseUtility.Equals(cmsItems[i].Text, name)) {
                    items.Add(cmsItems[i]);
                    if(getFirst) {break;}
                }
            }
        }
        return items;
    }

    /// <summary>Get documents by parent and property (alias and value).</summary>
    public static IList<Document> GetDocumentsByParentAndProperty(Document cmsParent, string propertyAlias, object propertyValue, bool getFirst = false)
    {
        IList<Document> items = new List<Document>();

        if(cmsParent != null) {
            var cmsItems = cmsParent.Children;
            var cmsItemsCount = cmsItems.Length;
            Property cmsProperty = null;

            for(int i = 0;i < cmsItemsCount;i += 1) {
                cmsProperty = cmsItems[i].getProperty(propertyAlias);
                if(cmsProperty != null && cmsProperty.Value.Equals(propertyValue)) {
                    items.Add(cmsItems[i]);
                    if(getFirst) {break;}
                }
            }
        }
        return items;
    }

    /// <summary>Get first descendant of parent document. Optionally, include self.</summary>
    public static IList<Document> GetDocumentDescendants(Document cmsParent, string typeAlias, bool allSameLevel = false, bool includeSelf = false)
    {
        IList<Document> items = new List<Document>();

        if(cmsParent != null) {
            if(includeSelf && BaseUtility.Equals(cmsParent.ContentType.Alias, typeAlias)) {
                items.Add(cmsParent);
            }

            int currentLevel = -1;
            var cmsItems = cmsParent.GetDescendants().Cast<Document>().ToList();
            var cmsItemsCount = cmsItems.Count;

            for(int i = 0;i < cmsItemsCount;i += 1) {
                if(BaseUtility.Equals(cmsItems[i].ContentType.Alias, typeAlias)) {
                    if(allSameLevel && i == 0) {
                        currentLevel = cmsItems[i].Level;
                    }
                    if(allSameLevel && currentLevel == cmsItems[i].Level) {
                        items.Add(cmsItems[i]);
                    } else if(currentLevel == -1) {
                        items.Add(cmsItems[i]);
                    }
                }
            }
        }
        return items;
    }

    /// <summary>Get first descendant of parent document. Optionally, include self.</summary>
    public static Document GetDocumentFirstDescendant(Document cmsParent, string itemTypeAlias, bool includeSelf = false)
    {
        if(cmsParent != null) {
            if(includeSelf && BaseUtility.Equals(cmsParent.ContentType.Alias, itemTypeAlias)) {
                return cmsParent;
            }
            return cmsParent.GetDescendants().Cast<Document>()
                .Where(x => BaseUtility.Equals(x.ContentType.Alias, itemTypeAlias))
                .FirstOrDefault();
        }
        return null;
    }

    /// <summary>Get property value from document.</summary>
    public static object GetDocumentPropertyValue(Document cmsItem, string propertyAlias)
    {
        if(cmsItem != null) {
            var cmsProperty = cmsItem.getProperty(propertyAlias);
            if(cmsProperty != null) {
                return cmsProperty.Value;
            }
        }
        return null;
    }
}

} // END namespace UmbracoLabs.Web.Helpers