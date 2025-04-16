using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
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
   private readonly IHostApplicationLifetime _applicationLifetime;
   private readonly ILogger<TurboStreamsWebsocketMiddleware> _logger;
   
   /// <summary>
   ///   Constructor.
   /// </summary>
   public TurboStreamsWebsocketMiddleware(ITurboBroadcaster broadcaster, IChannelEncryption channelEncryption, IHostApplicationLifetime applicationLifetime, ILogger<TurboStreamsWebsocketMiddleware> logger)
   {
      _broadcaster = broadcaster;
      _channelEncryption = channelEncryption;
      _applicationLifetime = applicationLifetime;
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

      _logger.LogDebug("Received websocket connection request");

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
            _logger.LogWarning("Received websocket connection request without channel name");
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

            // Make sure the task is cancelled when the application shuts down.
            _applicationLifetime.ApplicationStopping.Register(() => { tcs.TrySetCanceled(); });
            
            await tcs.Task; // Block until the application shuts down or the client closes the connection.
            
            if(webSocket.State == WebSocketState.Open)
               await webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Application shutting down", CancellationToken.None);
         }
         catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException) 
         {
            // Ignore the exception, it is expected when the application is shutting down.
         }
         catch (ArgumentException) // Thrown by the channel encryption when signature is not valid.
         {
            _logger.LogWarning("Received websocket connection request with invalid signature");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
         }
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error in TurboStreamsWebsocketMiddleware");
      }
   }
}