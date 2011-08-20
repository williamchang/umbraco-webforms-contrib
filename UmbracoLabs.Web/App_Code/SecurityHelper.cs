/**
@file
    SecurityHelper.cs
@author
    William Chang
@version
    0.1
@date
    - Created: 2009-08-11
    - Modified: 2010-09-22
    .
@note
    References:
    - General:
        - http://api.wordpress.org/secret-key/1.1/
        - http://markjaquith.wordpress.com/2006/06/02/wordpress-203-nonces/
        .
    - Conversion:
        - http://ostermiller.org/calc/encode.html
        .
    - Hashing (one way):
        - http://msdn.microsoft.com/en-us/library/aa302398.aspx
        .
    .
*/

using System;

namespace UmbracoLabs.Web.Helpers {

public static class SecurityHelper
{
    /// <summary>Static constructor.</summary>
    static SecurityHelper() {}

    /// <summary>Create hash using MD5 cryptography.</summary>
    public static string CreateHashMd5(string s)
    {
        /*using(var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider()) {
            byte[] originalBytes = System.Text.Encoding.Unicode.GetBytes(s);
            byte[] hashBytes = md5.ComputeHash(originalBytes);
            return hashBytes.ToStringFromBytes();
        }*/
        return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5");

    }

    /// <summary>Create hash using SHA-256 cryptography.</summary>
    public static string CreateHashSha256(string s)
    {
        using(var sha = new System.Security.Cryptography.SHA256Managed()) {
            byte[] originalBytes = System.Text.Encoding.Unicode.GetBytes(s);
            byte[] hashBytes = sha.ComputeHash(originalBytes);
            return hashBytes.ToStringFromBytes();
        }
    }

    /// <summary>Create hash from salt, for user password.</summary>
    public static string CreateHashPassword(string password, string salt)
    {
        string raw = String.Concat(password, salt);
        string hash = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(raw, "SHA1");
        hash = String.Concat(hash, salt);
        return hash;
    }
    
    /// <summary>Create salt with Base64.</summary>
    public static string CreateSalt(int size)
    {
        // Generate a cryptographic random number using the cryptographic service provider
        var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        byte[] saltBytes = new byte[size];
        rng.GetBytes(saltBytes);
        // Return a base64 string representation of the random number.
        return saltBytes.ToBase64();
    }

    /// <summary>Create random key using guid (maximum length of 32 characters).</summary>
    /// <remarks>http://aspnet.4guysfromrolla.com/articles/101205-1.aspx</remarks>
    public static string CreateRandomKeyUsingGuid(int length)
    {
        // Create guid.
        string guidResult = System.Guid.NewGuid().ToString();
        // Remove the hyphens.
        guidResult = guidResult.Replace("-", String.Empty);
        // Make sure length is valid.
        if(length <= 0 || length > guidResult.Length) {
            throw new ArgumentException("Length must be between 1 and " + guidResult.Length + ".");
        }
        // Return the first length bytes.
        return guidResult.Substring(0, length);
    }

    /// <summary>Encrypt query string. (Replacing plus signs (+), forward slashes (/), and equal signs (=) from base64.)</summary>
    /// <remarks>http://kennyshu.blogspot.com/2008/11/encrypt-query-string-by-using-base64.html</remarks>
    public static string EncryptQueryString(string s)
    {
        byte[] b = System.Text.Encoding.UTF8.GetBytes(s);
        return Convert.ToBase64String(b)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace('=', '!');
    }

    /// <summary>Decrypt query string.</summary>
    /// <remarks>http://kennyshu.blogspot.com/2008/11/encrypt-query-string-by-using-base64.html</remarks>
    public static string DecryptQueryString(string s)
    {
        byte[] b = Convert.FromBase64String(s.Replace('-', '+').Replace('_', '/').Replace('!', '='));
        return System.Text.Encoding.UTF8.GetString(b);
    }

    /// <summary>Converts an array of 8-bit unsigned integers to string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToBase64(this byte[] b)
    {
        return Convert.ToBase64String(b);
    }

    /// <summary>Converts an array of 8-bit unsigned integers to string.</summary>
    /// <remarks>Extension method.</remarks>
    public static string ToStringFromBytes(this byte[] b)
    {
        return BitConverter.ToString(b);
    }
}

} // END namespace UmbracoLabs.Web.Helpers