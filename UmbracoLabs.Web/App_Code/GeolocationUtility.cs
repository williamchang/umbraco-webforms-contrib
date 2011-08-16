/**
@file
    GeolocationUtility.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2011-07-19
    - Modified: 2011-08-15
    .
@note
    References:
    - General:
        - http://www.storm-consultancy.com/blog/development/code-snippets/the-haversine-formula-in-c-and-sql/
        - http://www.storm-consultancy.com/blog/development/code-snippets/convert-an-angle-in-degrees-to-radians-in-c/
        .
    .

    Usage:
    
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace UmbracoLabs {

public static class GeolocationUtility
{

#region Enumerations

    public enum DistanceUnit {
        Kilometers = 0,
        Miles = 1
    }

#endregion

    /// <summary>Static constructor.</summary>
    static GeolocationUtility() {}

    /// <summary>Convert an angle in degrees to radians.</summary>
    /// <remarks>Extension method.</remarks>
    public static double ToRadians(this double angle)
    {
        return (Math.PI / 180) * angle;
    }

    /// <summary>Returns the distance using Haversine formula in miles or kilometers of any two latitude and longitude points.</summary>
    /// <param name="p1">Location 1</param>
    /// <param name="p2">Location 2</param>
    /// <param name="unit">Miles or Kilometers</param>
    /// <returns>Distance in the requested unit.</returns>
    public static double GetDistance(LatLong p1, LatLong p2, DistanceUnit unit)
    {
        double r = (unit == DistanceUnit.Miles) ? 3960 : 6371;
        var lat = (p2.Latitude - p1.Latitude).ToRadians();
        var lng = (p2.Longitude - p1.Longitude).ToRadians();
        var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) + Math.Cos(p1.Latitude.ToRadians()) * Math.Cos(p2.Latitude.ToRadians()) * Math.Sin(lng / 2) * Math.Sin(lng / 2);
        var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));
        return r * h2;
    }

#region Presentation Models

    /// <summary>Specifies a latitude and longitude point.</summary>
    /// <remarks>Presentation model object.</remarks>
    public class LatLong
    {
        public double Latitude {get;set;}
        public double Longitude {get;set;}

        /// <summary>Default constructor.</summary>
        public LatLong() {}

        /// <summary>Argument constructor.</summary>
        public LatLong(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }

}

#endregion

} // END namespace UmbracoLabs