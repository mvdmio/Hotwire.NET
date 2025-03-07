using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Middleware for accepting Turbo Streams websocket connections.
/// </summary>
public class TurboStreamsWebsocketMiddleware : IMiddleware
{
   private readonly ITurboBroadcaster _broadcaster;
   private readonly IChannelEncryption _channelEncryption;
   private readonly ILogger<TurboStreamsWebsocketMiddleware> _logger;

   /// <summary>
   ///   Constructor.
   /// </summary>
   public TurboStreamsWebsocketMiddleware(ITurboBroadcaster broadcaster, IChannelEncryption channelEncryption, ILogger<TurboStreamsWebsocketMiddleware> logger)
   {
      _broadcaster = broadcaster;
      _channelEncryption = channelEncryption;
      _logger = logger;
   }

   /// <inheritdoc />
   public async Task InvokeAsync(HttpContext context, RequestDelegate next)
   {
      if (!context.WebSockets.IsWebSocketRequest)
      {
         await next(context);
         return;
      }

      try
      {
         if (!context.Request.Path.StartsWithSegments("/turbo/ws"))
         {
            _logger.LogWarning("Received websocket connection request on the wrong path. Expected /turbo/ws, but was {Path}", context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
         }

         var signedChannelName = context.Request.Path.Value?.Substring("/turbo/ws/".Length);
         if (string.IsNullOrWhiteSpace(signedChannelName))
         {
            _logger.LogWarning("Received websocket connection request without signature.");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
         }

         try
         {
            var channelName = _channelEncryption.Decrypt(signedChannelName);

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var tcs = new TaskCompletionSource();
            await _broadcaster.AddConnection(channelName, webSocket, tcs);

            _logger.LogInformation("Accepted websocket connection for channel {ChannelName}", channelName);

            await tcs.Task; // Block until the application shuts down or the client closes the connection.
         }
         catch (ArgumentException) // Thrown by the channel encryption when signature is not valid.
         {
            _logger.LogWarning("Received websocket connection request with invalid signature.");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
         }
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error in TurboStreamsWebsocketMiddleware");
      }
   }
}