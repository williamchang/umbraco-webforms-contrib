/**
@file
    SqlUtility.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2010-10-12
    - Modified: 2011-08-20
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
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace UmbracoLabs.Data {

public static class SqlUtility
{
    /// <summary>Static constructor.</summary>
    static SqlUtility() {}

    /// <summary>Escape string for SQL.</summary>
    /// <remarks>Extension method. Alternatively, use SQL parameters.</remarks>
    public static string EscapeSql(this string s)
    {
        if(!String.IsNullOrEmpty(s)) {
            //s = s.Replace("'", @"\'"); // MySQL
            s = s.Trim().Replace("'", "''"); // Microsoft SQL Server
        }
        return s;
    }

    /// <summary>Get SQL null.</summary>
    public static string GetSqlNull()
    {
        return "NULL";
    }

    /// <summary>Get SQL token from System.Object.</summary>
    public static string GetSqlObject(Object obj)
    {
        var s = String.Empty;
        if(obj is DBNull || obj == null) {
            s = GetSqlNull();
        } else if(obj is sbyte || obj is byte || obj is short || obj is ushort || obj is int || obj is uint || obj is long || obj is ulong || obj is decimal || obj is double || obj is float) {
            s = Convert.ToString(obj, System.Globalization.NumberFormatInfo.InvariantInfo);
        } else if(obj is bool) {
            s = ToSqlBit(obj);
        } else if(obj is char || obj is Enum || obj is Guid) {
            s = ToSqlString(obj);
        } else if(obj is DateTime) {
            s = ToSqlDate(obj);
        } else if(obj is String) {
            s = ToSqlString(obj);
        } else {
            s = ToSqlString(obj);
        }
        return s;
    }

    /// <summary>Is System.Data.DataColumn numeric.</summary>
    /// <remarks>Extension method.</remarks>
    public static bool IsNumeric(this DataColumn col) {
        if(col == null) {return false;}
        var numericTypes = new [] {
            typeof(Byte),
            typeof(Decimal),
            typeof(Double),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64),
            typeof(SByte),
            typeof(Single),
            typeof(UInt16),
            typeof(UInt32),
            typeof(UInt64)
        };
        return numericTypes.Contains(col.DataType);
    }

    /// <summary>Escape string for SQL.</summary>
    public static string SqlFormat(string format, params Object[] objs)
    {
        var tokens = new List<string>();
        if(!String.IsNullOrEmpty(format)) {
            for(int i = 0;i < objs.Length;i += 1) {
                tokens.Add(GetSqlObject(objs[i]));
            }
            return String.Format(format, tokens.ToArray());
        }
        return null;
    }

    /// <summary>To JSON from System.Data.DateTable.</summary>
    public static string ToJson(this DataTable dt) {
        var sb1 = new System.Text.StringBuilder();

        sb1.Append("{"); // BEGIN: Object
        // Validate table name.
        if(dt.TableName == null || String.IsNullOrEmpty(dt.TableName)) {
            dt.TableName = "table1";
        }
        sb1.AppendFormat("\"{0}\":", dt.TableName); // Member
        sb1.Append("["); // BEGIN: Array
        int rowIndex = 0;
        foreach(System.Data.DataRow row in dt.Rows) {
            sb1.Append("{"); // BEGIN: Object
            int colIndex = 0;
            foreach(System.Data.DataColumn col in dt.Columns) {
                // Header
                sb1.AppendFormat("\"{0}\":", col.ColumnName); // Name
                // Body
                sb1.Append(BaseUtility.GetJsonObject(row[col])); // Value
                // Separated by a comma for next pair.
                if(dt.Columns.Count != colIndex + 1) {
                    sb1.Append(", ");
                }
                colIndex++;
            }
            sb1.Append("}"); // END: Object
            // Separated by a comma for next object.
            if(dt.Rows.Count != rowIndex + 1) {
                sb1.Append(", ");
            }
            rowIndex++;
        }
        sb1.Append("]"); // END: Array
        sb1.Append("}"); // END: Object
        return sb1.ToString();
    }

    /// <summary>To SQL string.</summary>
    /// <remarks>Extension method. Alternatively, use SQL parameters.</remarks>
    public static string ToSqlString(this Object o)
    {
        return String.Concat("'", EscapeSql(o.ToString()), "'");
    }

    /// <summary>To SQL bit.</summary>
    /// <remarks>Extension method. Alternatively, use SQL parameters.</remarks>
    public static string ToSqlBit(this Object o)
    {
        if(o != null) {
            bool v = (bool)o;
            if(v == true) {
                return "1";
            } else if(v == false) {
                return "0";
            }
        }
        return GetSqlNull();
    }

    /// <summary>To SQL of System.DateTime (or use GETDATE() for SQL).</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToSqlDate(this Object o)
    {
        if(o != null) {
            return ToSqlString(((DateTime)o).ToString("yyyyMMdd HH:mm:ss.fff"));
        }
        return GetSqlNull();
    }

    /// <summary>To string of System.Data.DataSet.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToStringDebug(this DataSet ds) {
        var sb1 = new System.Text.StringBuilder();

        // Create dataset name.
        sb1.Append(ds.DataSetName);
        sb1.AppendFormat("\n");
        foreach(DataTable tbl in ds.Tables) {
            // Create table name.
            sb1.AppendFormat(tbl.TableName);
            sb1.AppendFormat("\n");
            // Create header
            foreach(DataColumn col in tbl.Columns) {
                sb1.AppendFormat("{0}, ", col.ColumnName);
            }
            sb1.AppendFormat("\n");
            // Create body
            foreach(DataRow row in tbl.Rows) {
                foreach(DataColumn col in tbl.Columns) {
                    sb1.AppendFormat("{0}, ", row[col].ToString());
                }
                sb1.AppendFormat("\n");
            }
        }
        // Create statistics.
        sb1.AppendFormat("totalTables: {0}", ds.Tables.Count);
        sb1.AppendFormat("\n");
        sb1.AppendFormat("totalTableRows: {0}", ds.Tables[0].Rows.Count);
        sb1.AppendFormat("\n");
        sb1.AppendFormat("totalTableColumns: {0}", ds.Tables[0].Columns.Count);
        sb1.AppendFormat("\n");
        return sb1.ToString();
    }

    /// <summary>To string of System.Data.DateTable.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToStringDebug(this DataTable dt)
    {
        var ds = new DataSet();
        ds.Tables.Add(dt);
        return ToStringDebug(ds);
    }

    /// <summary>To string column of System.Data.DateTable.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToStringSpecial(this DataTable dt, string columnName)
    {
        return ToStringSpecial(dt, columnName, ", ");
    }

    /// <summary>To string column of System.Data.DateTable.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToStringSpecial(this DataTable dt, string columnName, string separator) {
        var sb1 = new System.Text.StringBuilder();
        var hasSeparator = false;
        
        foreach(DataRow row in dt.Rows) {
            if(row[columnName] != null && !String.IsNullOrEmpty(row[columnName].ToString())) {
                if(hasSeparator) {sb1.Append(separator);}
                sb1.Append(row[columnName].ToString());
                hasSeparator = true;
            }
        }
        return sb1.ToString();
    }
}

} // END namespace UmbracoLabs.Data