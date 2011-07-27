/**
@file
    CmsHelper.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2011-06-21
    - Modified: 2011-07-25
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
using umbraco.cms.businesslogic.macro;
using umbraco.interfaces;
using umbraco.MacroEngines;
using umbraco.NodeFactory;

namespace UmbracoLabs.Web.Helpers {

public static class CmsHelper
{
    /// <summary>Static constructor.</summary>
    static CmsHelper() {}

    /// <summary>Get parents (aka ancestors). Optionally, include self and nodeTypeAlias (null to disable).</summary>
    /// <remarks>Extension method.</remarks>
    public static IList<INode> GetAncestors(this INode cmsItem, bool includeSelf = false, string nodeTypeAlias = null)
    {
        var cmsItems = new List<INode>();

        if(includeSelf == false) {
            cmsItem = (Node)cmsItem.Parent;
        }
        while(cmsItem != null) {
            if(String.IsNullOrEmpty(nodeTypeAlias)) {
                cmsItems.Add(cmsItem);
            } else if(BaseUtility.Equals(nodeTypeAlias, cmsItem.NodeTypeAlias)) {
                cmsItems.Add(cmsItem);
            }
            cmsItem = (Node)cmsItem.Parent;
        }
        return cmsItems;
    }

    /// <summary>Get first parent (aka ancestor) that has a property value. Optionally, include self and nodeTypeAlias (null to disable).</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetFirstAncestorPropertyValue(this DynamicNode cmsItem, string propertyAlias, bool includeSelf = false, string nodeTypeAlias = null)
    {
        if(includeSelf == false) {
            cmsItem = cmsItem.Parent;
        }
        while(cmsItem != null) {
            var value = cmsItem.GetPropertyValue(propertyAlias);
            if(value != null) {
                if(String.IsNullOrEmpty(nodeTypeAlias)) {
                    return value;
                } else if(BaseUtility.Equals(nodeTypeAlias, cmsItem.NodeTypeAlias)) {
                    return value;
                }
            }
            cmsItem = cmsItem.Parent;
        }
        return null;
    }

    /// <summary>Get first descendant. Optionally, include self.</summary>
    /// <remarks>Extension method.</remarks>
    public static DynamicNode GetFirstDescendant(this DynamicNode cmsItem, string nodeTypeAlias, string pageName, bool includeSelf = false)
    {
        IList<DynamicNode> cmsItems;

        if(cmsItem != null) {
            if(includeSelf) {
                cmsItems = cmsItem.DescendantsOrSelf(nodeTypeAlias).Items;
            } else {
                cmsItems = cmsItem.Descendants(nodeTypeAlias).Items;
            }
            if(cmsItems.Count > 0) {
                return cmsItems.Where(x => x.Name == pageName).FirstOrDefault();
            }
        }
        return null;
    }

    /// <summary>Get first descendant that has a property value. Optionally, include self and nodeTypeAlias (null to disable).</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetFirstDescendantPropertyValue(this DynamicNode cmsItem, string propertyAlias, string pageName, bool includeSelf = false, string nodeTypeAlias = null)
    {
        IList<DynamicNode> cmsItems;

        if(cmsItem != null) {
            if(nodeTypeAlias != null && includeSelf) {
                cmsItems = cmsItem.DescendantsOrSelf(nodeTypeAlias).Items;
            } else if(nodeTypeAlias != null) {
                cmsItems = cmsItem.Descendants(nodeTypeAlias).Items;
            } else if(includeSelf) {
                cmsItems = cmsItem.DescendantsOrSelf().Items;
            } else {
                cmsItems = cmsItem.Descendants().Items;
            }
            for(int i = 0;i < cmsItems.Count;i += 1) {
                if(BaseUtility.Equals(cmsItems[i].Name, pageName)) {
                    return cmsItems[i].GetPropertyValue(propertyAlias);
                }
            }
        }
        return null;
    }

    /// <summary>Get an array of id's (comma delimited) matched items' property (alias).</summary>
    public static IList<string> GetIds(DynamicNodeList cmsItems, string alias, string[] tokens)
    {
        IList<string> items = new List<string>();
        int cmsItemsCount = cmsItems.Items.Count;
        int tokensCount = tokens.Length;
        int i, j = 0;

        for(i = 0;i < cmsItemsCount;i += 1) {
            for(j = 0;j < tokensCount;j += 1) {
                if(BaseUtility.Equals(cmsItems.Items[i].GetProperty(alias).Value, tokens[j])) {
                    items.Add(cmsItems.Items[i].Id.ToString());
                }
            }
        }
        return items;
    }

    /// <summary>Get item by id.</summary>
    public static DynamicNode GetItem(int? id)
    {
        return id != null && id > 0 ? new DynamicNode(id) : null;
    }

    /// <summary>Get item by id.</summary>
    public static DynamicNode GetItem(string id)
    {
        return !String.IsNullOrEmpty(id) ? new DynamicNode(id) : null;
    }

    /// <summary>Get items by id's (comma delimited).</summary>
    /// <remarks>
    /// References:
    /// http://our.umbraco.org/forum/developers/xslt/1741-getting-ultimate-picker-to-return-content-nodes-instead-of-id
    /// </remarks>
    public static IList<INode> GetItems(string ids)
    {
        INode cmsItem;
        IList<INode> cmsItems = new List<INode>();

        if(!String.IsNullOrEmpty(ids)) {
            var cmsItemsId = ids.SplitClean(new char[] {',', '.'});

            for(int i = 0;i < cmsItemsId.Length;i += 1) {
                cmsItem = new Node(cmsItemsId[i].ToTypeOrDefault<int>(0));
                if(cmsItem != null) {
                    cmsItems.Add(cmsItem);
                }
            }
        }
        return cmsItems;
    }

    /// <summary>Get file URL from media item.</summary>
    public static string GetMediaFileUrl(dynamic cmsMediaItem)
    {
        return cmsMediaItem.umbracoFile;
    }

    /// <summary>Get file URL from media item.</summary>
    public static string GetMediaFileUrl(int? id)
    {
        if(id != null) {
            var cmsMediaItem = new umbraco.cms.businesslogic.media.Media(id ?? 0);
            if(cmsMediaItem != null) {
                var propMediaFile = cmsMediaItem.getProperty("umbracoFile");
                if(propMediaFile != null) {
                    return propMediaFile.Value.ToString();
                }
            }
        }
        return null;
    }

    /// <summary>Get file URL from media item.</summary>
    public static string GetMediaFileUrl(string id)
    {
        return !String.IsNullOrEmpty(id) ? GetMediaFileUrl(Convert.ToInt32(id)) : null;
    }

    /// <summary>Get media item by id.</summary>
    public static dynamic GetMediaItem(Object id)
    {
        return !BaseUtility.IsNullOrStringEmpty(id) ? new DynamicMedia(id) : null;
    }

    /// <summary>Get media item by id.</summary>
    public static dynamic GetMediaItem(string id)
    {
        return !String.IsNullOrEmpty(id) ? new DynamicMedia(id) : null;
    }

    /// <summary>Get node by id.</summary>
    public static INode GetNode(string id)
    {
        return !String.IsNullOrEmpty(id) ? new Node(Convert.ToInt32(id)) : null;
    }

    /// <summary>Get property alias from macro's alias.</summary>
    public static string GetPropertyAliasByMacro(DynamicNode cmsItem, MacroModel macro)
    {
        if(macro != null) {
            var properties = cmsItem.PropertiesAsList;
            var alias = macro.Alias;
            var value = String.Empty;

            for(int i = 0;i < properties.Count;i += 1) {
                value = properties[i].Value;
                if(value.Contains(alias)) {
                    return properties[i].Alias;
                }
            }
        }
        return null;
    }

    /// <summary>Get property alias from value.</summary>
    public static string GetPropertyAliasByValue(DynamicNode cmsItem, string value)
    {
        if(!String.IsNullOrEmpty(value)) {
            var properties = cmsItem.PropertiesAsList;
            var alias = String.Empty;

            for(int i = 0;i < properties.Count;i += 1) {
                alias = properties[i].Alias;
                if(BaseUtility.Equals(value, alias)) {
                    return alias;
                }
            }
        }
        return null;
    }

    /// <summary>Get value from dynamicnode's property.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetPropertyValue(this DynamicNode cmsItem, string alias, bool nullIfEmpty = true)
    {
        if(cmsItem != null) {
            var cmsProperty = cmsItem.GetProperty(alias);
            if(cmsProperty != null) {
                var value = cmsProperty.Value;
                if(nullIfEmpty == false) {
                    return value;
                } else if(nullIfEmpty == true && !String.IsNullOrEmpty(value)) {
                    return value;
                }
            }
        }
        return null;
    }

    /// <summary>Get value from node's property.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetPropertyValue(this INode cmsItem, string alias, bool nullIfEmpty = true)
    {
        return cmsItem != null ? GetPropertyValue(new DynamicNode(cmsItem), alias, nullIfEmpty) : null;
    }

    /// <summary>Get URL by content item.</summary>
    public static string GetUrl(DynamicNode cmsItem)
    {
        return cmsItem != null ? cmsItem.Url : null;
    }

    /// <summary>Get URL by content item.</summary>
    /// <remarks>Extension method.</remarks>
    public static string GetUrl(this INode cmsItem)
    {
        return cmsItem != null ? cmsItem.Url : null;
    }

    /// <summary>Get URL by content item.</summary>
    public static string GetUrl(Object id)
    {
        var cmsItem = new DynamicNode(id);
        var url = GetUrl(cmsItem);
        return String.IsNullOrEmpty(url) ? null : url;
    }

    /// <summary>Contains any from a list.</summary>
    /// <remarks>Extension method.</remarks>
    public static bool ContainsAnySpecial(this string haystack, IList<string> needles)
    {
        if(!String.IsNullOrEmpty(haystack) || needles.Count > 0) {
            foreach(var value in needles) {
                if(haystack.Contains(value)) {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>Indicates whether the specified string is null or an System.String.Empty string.</summary>
    public static bool IsNullOrStringEmpty(Object value)
    {
        return BaseUtility.IsNullOrStringEmpty(value);
    }

    /// <summary>Order list by distance ascending.</summary>
    public static IList<DynamicNode> OrderByDistance(DynamicNodeList cmsItems, double currentLatitude, double currentLogitude, string cmsLatitudePropertyAlias, string cmsLongitudePropertyAlias)
    {
        IDictionary<DynamicNode, double> pairItems = new Dictionary<DynamicNode, double>();
        int cmsItemsCount = cmsItems.Items.Count;
        string propLatitude, propLogitude;
        GeolocationUtility.LatLong pointCurrent = new GeolocationUtility.LatLong(currentLatitude, currentLogitude);
        GeolocationUtility.LatLong pointCompare;

        for(int i = 0;i < cmsItemsCount;i += 1) {
            propLatitude = GetPropertyValue(cmsItems.Items[i], cmsLatitudePropertyAlias);
            propLogitude = GetPropertyValue(cmsItems.Items[i], cmsLongitudePropertyAlias);
            if(!String.IsNullOrEmpty(propLatitude) && !String.IsNullOrEmpty(propLogitude)) {
                pointCompare = new GeolocationUtility.LatLong(Convert.ToDouble(propLatitude), Convert.ToDouble(propLogitude));
                pairItems.Add(cmsItems.Items[i], GeolocationUtility.GetDistance(pointCurrent, pointCompare, GeolocationUtility.DistanceUnit.Miles));
            }
        }
        return pairItems.OrderBy(x => x.Value).Select(x => x.Key).ToList();
    }

    /// <summary>To list from razor and lambda expressions using dynamic.</summary>
    public static IList<DynamicNode> ToList(DynamicNodeList cmsItems)
    {
       return cmsItems.Items;
    }

    /// <summary>Converts the value of the specified object to its equivalent string representation.</summary>
    /// <returns>The string representation of value, or System.String.Empty if value is null.</returns>
    public static string ToString(Object value, string defaultValue)
    {
        return BaseUtility.ToString(value, defaultValue);
    }
}

} // END namespace UmbracoLabs.Web.Helpers