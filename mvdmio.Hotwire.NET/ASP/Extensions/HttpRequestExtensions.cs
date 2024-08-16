using System.Linq;
using Microsoft.AspNetCore.Http;

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
      return acceptHeader.Contains("text/vnd.turbo-stream.html");
   }
}