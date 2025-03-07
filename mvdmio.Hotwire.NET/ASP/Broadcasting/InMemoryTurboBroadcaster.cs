using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Implements the Turbo Streams broadcaster with an in-memory storage of connections.
/// </summary>
public class InMemoryTurboBroadcaster : ITurboBroadcaster, IDisposable
{
   private readonly ILogger<InMemoryTurboBroadcaster> _logger;
   private readonly IList<(string channel, WebSocket socket, TaskCompletionSource tcs)> _connections;

   /// <summary>
   ///   Constructor.
   /// </summary>
   public InMemoryTurboBroadcaster(ILogger<InMemoryTurboBroadcaster> logger)
   {
      _logger = logger;
      _connections = new List<(string channel, WebSocket socket, TaskCompletionSource tcs)>();
   }

   /// <inheritdoc />
   public Task AddConnection(string channel, WebSocket webSocket, TaskCompletionSource tcs)
   {
      _connections.Add((channel, webSocket, tcs));
      return Task.CompletedTask;
   }

   /// <inheritdoc />
   public async Task BroadcastAsync(string channel, ITurboAction turboAction, CancellationToken ct = default)
   {
      var sockets = _connections.Where(x => x.channel == channel).Select(x => x.socket);
      var message = await turboAction.Render();
      var buffer = Encoding.UTF8.GetBytes(message.Value!);
      
      foreach (var socket in sockets)         
         await socket.SendAsync(buffer, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.DisableCompression, ct);
   }

   /// <inheritdoc />
   public void Dispose()
   {
      GC.SuppressFinalize(this);

      foreach (var connection in _connections)
      {
         _logger.LogDebug("Closing websocket connection: {ChannelName}", connection.channel);

         connection.tcs.TrySetResult(); // This closes the websocket connection.
         connection.socket.Dispose();
      }
   }
}