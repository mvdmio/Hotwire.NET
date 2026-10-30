using Microsoft.AspNetCore.Http;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Builds the websocket address a Turbo broadcast channel connects to.
/// </summary>
internal static class TurboBroadcastChannelAddress
{
   /// <summary>
   ///   Builds the websocket address for the given signed channel name, including the request's path base.
   /// </summary>
   public static string Create(HttpRequest request, string signedChannelName)
   {
      var protocol = request.IsHttps ? "wss" : "ws";
      return $"{protocol}://{request.Host}{request.PathBase}/turbo/ws/{signedChannelName}";
   }
}
