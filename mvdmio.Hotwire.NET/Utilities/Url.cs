using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace mvdmio.Hotwire.NET.Utilities;

/// <summary>
/// Class for working with URLs.
/// </summary>
[PublicAPI]
public struct Url
{
   /// <summary>
   /// The URL scheme (http or https).
   /// </summary>
   public string Scheme { get; set; }
   
   /// <summary>
   /// The Host of the URL.
   /// </summary>
   public string Host { get; set; }
   
   /// <summary>
   /// The Port of the URL.
   /// </summary>
   public int Port { get; set; }
   
   /// <summary>
   /// The path of the URL.
   /// </summary>
   public string Path { get; set; }
   
   /// <summary>
   /// The Query parameters of the URL.
   /// </summary>
   public Dictionary<string, StringValues> Query { get; }

   /// <summary>
   /// The URL as a URI.
   /// </summary>
   public Uri Uri => new(ToString());
   
   /// <summary>
   /// The absolute URL. e.g. https://some-host.com/path?query=value
   /// </summary>
   public string AbsoluteUrl => Uri.AbsoluteUri;

   /// <summary>
   /// Constructor.
   /// </summary>
   public Url(string url)
      : this(new Uri(url))
   {
   }

   /// <summary>
   /// Constructor.
   /// </summary>
   public Url(Uri uri)
   {
      Scheme = uri.Scheme;
      Host = uri.Host;
      Port = uri.Port;
      Path = uri.AbsolutePath;
      Query = QueryHelpers.ParseQuery(uri.Query);
   }

   /// <inheritdoc />
   public override string ToString()
   {
      string url;
      if((Scheme is "http" && Port is 80) || (Scheme is "https" && Port is 443))
         url = $"{Scheme}://{Host}{Path}";
      else
         url = $"{Scheme}://{Host}:{Port}{Path}";
      
      return QueryHelpers.AddQueryString(url, Query);
   }

   /// <summary>
   /// Create a new Url without the given query parameter.
   /// </summary>
   public Url WithoutQueryParam(string key)
   {
      var clone = new Url(Uri);
      clone.Query.Remove(key);

      return clone;
   }

   /// <summary>
   /// Create a new Url with the given query parameter and value.
   /// </summary>
   public Url WithQueryParam(string key, string value)
   {
      var clone = new Url(Uri);
      clone.Query[key] = value;
      return clone;
   }

   /// <summary>
   /// Makes the Url usable as if it was a string.
   /// </summary>
   /// <param name="url"></param>
   /// <returns></returns>
   public static implicit operator string(Url url)
   {
      return url.AbsoluteUrl;
   }
}