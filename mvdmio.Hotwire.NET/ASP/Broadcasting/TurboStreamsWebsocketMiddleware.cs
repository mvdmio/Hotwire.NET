using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Broadcasting.ValueObjects;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Middleware for accepting Turbo Streams websocket connections.
/// </summary>
public sealed class TurboStreamsWebsocketMiddleware : IMiddleware
{
   private readonly CancellationToken _shutdownCt;
   
   private readonly ITurboBroadcaster _broadcaster;
   private readonly IChannelEncryption _channelEncryption;
   private readonly ILogger<TurboStreamsWebsocketMiddleware> _logger;
   
   /// <summary>
   ///   Constructor.
   /// </summary>
   public TurboStreamsWebsocketMiddleware(IHostApplicationLifetime applicationLifetime, ITurboBroadcaster broadcaster, IChannelEncryption channelEncryption, ILogger<TurboStreamsWebsocketMiddleware> logger)
   {
      _broadcaster = broadcaster;
      _channelEncryption = channelEncryption;
      _logger = logger;
      
      _shutdownCt = applicationLifetime.ApplicationStopping;
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

            var tcs = new TaskCompletionSource();
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            var connectionId = await _broadcaster.AddConnection(channelName, webSocket);
            _ = HandleConnectionAsync(connectionId, channelName, webSocket, tcs);
            
            _logger.LogInformation("Accepted websocket connection {Id} for channel {ChannelName}", connectionId.Value, channelName);

            await tcs.Task; // Block until the application shuts down or the client closes the connection.
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
   
   private async Task HandleConnectionAsync(ConnectionId connectionId, string channel, WebSocket webSocket, TaskCompletionSource tcs)
   {
      var buffer = new byte[4096]; // For receive; size doesn't matter if ignoring data
      try
      {
         while (webSocket.State == WebSocketState.Open && !_shutdownCt.IsCancellationRequested)
         {
            var result = await webSocket.ReceiveAsync(buffer, _shutdownCt);
            if (result.MessageType == WebSocketMessageType.Close)
            {
               _logger.LogInformation("Client closed WebSocket {Id} for channel {Channel}", connectionId.Value, channel);
               break;
            }
            // Ignore any received data (or handle if needed, e.g., protocol messages)
         }
      }
      catch (OperationCanceledException) { } // Shutdown
      catch (WebSocketException)
      {
         // This happens when the browser closes the websocket connection without the close handshake. Occurs often when the browser is refreshed.
         // We can just remove the connection when this happens.
         _logger.LogInformation("WebSocket {Id} closed by client", connectionId.Value);
      }
      finally
      {
         await _broadcaster.RemoveConnection(connectionId, _shutdownCt); // This also closes and disposes the socket.
         tcs.TrySetResult(); // Unblock middleware
      }
   }
}