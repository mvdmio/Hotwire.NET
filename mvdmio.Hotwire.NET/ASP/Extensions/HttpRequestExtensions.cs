using System.Linq;
using Microsoft.AspNetCore.Http;
using mvdmio.Hotwire.NET.Utilities;

namespace mvdmio.Hotwire.NET.ASP.Extensions;

/// <summary>
/// Extensions for <see cref="HttpRequest"/>.
/// </summary>
public static class HttpRequestExtensions
{
   /// <summary>
   /// Check if the request is a Turbo request. Returns true if the turbo request header is set on the request.
   /// </summary>
   public static bool IsTurboRequest(this HttpRequest request)
   {
      // For more info, see: https://turbo.hotwired.dev/handbook/streams#streaming-from-http-responses
      
      var acceptHeader = request.Headers["Accept"];
      var values = acceptHeader.Where(x => x is not null).SelectMany(x => x!.Split(",")).Select(x => x.Trim());
      return values.Contains("text/vnd.turbo-stream.html");
   }
   
   /// <summary>
   /// Retrieve the Referer Url from the request headers.
   /// Uses the turbo-referrer header if the HTTP referer header is not set; this happens when turbo sends the request from a form-interaction.
   /// </summary>
   public static Url? Referer(this HttpRequest request)
   {
      request.Headers.TryGetValue("Referer", out var referer);
      request.Headers.TryGetValue("turbo-referrer", out var turboReferer);

      if (referer.Count > 0 && referer[0] is not null)
      {
         var refererValue = referer[0];
         if (refererValue is not null)
            return new Url(refererValue);
      }

      if (turboReferer.Count > 0)
      {
         var turboRefererValue = turboReferer[0];
         if (turboRefererValue is not null)
            return new Url(turboRefererValue);
      }

      return null;
   }
}