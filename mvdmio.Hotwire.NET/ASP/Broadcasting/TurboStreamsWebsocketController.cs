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
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
         }

         var signedChannelName = context.Request.Path.Value?.Substring("/turbo/ws/".Length);
         if (string.IsNullOrWhiteSpace(signedChannelName))
         {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
         }

         try
         {
            var channelName = _channelEncryption.Decrypt(signedChannelName);

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var tcs = new TaskCompletionSource();
            await _broadcaster.AddConnection(channelName, webSocket, tcs);

            await tcs.Task;
         }
         catch (ArgumentException) // Thrown by the channel encryption when signature is not valid.
         {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
         }
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error in TurboStreamsWebsocketMiddleware");
      }
   }
}