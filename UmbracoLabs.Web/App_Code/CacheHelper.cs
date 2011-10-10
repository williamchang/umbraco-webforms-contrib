/**
@file
    CacheHelper.cs
@version
    0.1
@date
    - Created: 2011-10-04
    - Modified: 2011-10-04
    .
@note
    References:
    - http://johnnycoder.com/blog/2008/12/10/c-cache-helper-class/
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

public static class CacheHelper
{
    /// <summary>Static constructor.</summary>
    static CacheHelper() {}

    /// <summary>
    /// Insert value into the cache using
    /// appropriate name/value pairs
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="o">Item to be cached</param>
    /// <param name="key">Name of item</param>
    public static void Add<T>(T o, string key)
    {
        // NOTE: Apply expiration parameters as you see fit.
        // I typically pull from configuration file.

        // In this example, I want an absolute
        // timeout so changes will always be reflected
        // at that time. Hence, the NoSlidingExpiration.
        HttpContext.Current.Cache.Insert(
            key,
            o,
            null,
            DateTime.Now.AddSeconds(GetExpirationInSeconds()),
            System.Web.Caching.Cache.NoSlidingExpiration
        );
    }

    /// <summary>
    /// Remove item from cache
    /// </summary>
    /// <param name="key">Name of cached item</param>
    public static void Clear(string key)
    {
        HttpContext.Current.Cache.Remove(key);
    }

    /// <summary>
    /// Check for item in cache
    /// </summary>
    /// <param name="key">Name of cached item</param>
    /// <returns></returns>
    public static bool Exists(string key)
    {
        return HttpContext.Current.Cache[key] != null;
    }

    /// <summary>
    /// Retrieve cached item
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Name of cached item</param>
    /// <param name="value">Cached value. Default(T) if
    /// item doesn't exist.</param>
    /// <returns>Cached item as type</returns>
    public static bool Get<T>(string key, out T value)
    {
        try {
            if(!Exists(key)) {
                value = default(T);
                return false;
            }
            value = (T)HttpContext.Current.Cache[key];
        } catch {
            value = default(T);
            return false;
        }
        return true;
    }

    /// <summary>Get cache expiration in seconds from configuration file (aka web.config). Default value is 86400 seconds or 24 hours.</summary>
    /// <remarks>/configuration/appSettings</remarks>
    public static double GetExpirationInSeconds()
    {
        string value = System.Configuration.ConfigurationManager.AppSettings["CacheHelper.ExpirationInSeconds"];
        if(!String.IsNullOrEmpty(value)) {
            return Convert.ToDouble(value);
        }
        return 86400;
    }
}

} // END namespace UmbracoLabs.Web.Helpers