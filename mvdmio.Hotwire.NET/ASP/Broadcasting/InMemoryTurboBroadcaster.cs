using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Broadcasting.ValueObjects;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Implements the Turbo Streams broadcaster with an in-memory storage of connections.
/// </summary>
public sealed class InMemoryTurboBroadcaster : ITurboBroadcaster
{
   private readonly ILogger<InMemoryTurboBroadcaster> _logger;
   private readonly ConcurrentDictionary<ConnectionId, (string channel, WebSocket socket)> _connections;

   /// <summary>
   ///   Constructor.
   /// </summary>
   public InMemoryTurboBroadcaster(ILogger<InMemoryTurboBroadcaster> logger)
   {
      _logger = logger;
      _connections = new ConcurrentDictionary<ConnectionId, (string channel, WebSocket socket)>();
   }

   /// <inheritdoc />
   public Task<ConnectionId> AddConnection(string channel, WebSocket webSocket, CancellationToken ct = default)
   {
      var connectionId = new ConnectionId();

      if (_connections.TryAdd(connectionId, (channel, webSocket)))
         return Task.FromResult(connectionId);
      
      throw new InvalidOperationException("Failed to add connection to the broadcaster. This should not happen.");
   }

   /// <inheritdoc />
   public async Task RemoveConnection(ConnectionId connectionId, CancellationToken ct = default)
   {
      if (!_connections.TryRemove(connectionId, out var connection))
         return; // Connection already removed.

      try
      {
         if (connection.socket.State == WebSocketState.Open)
            await connection.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closing connection", ct);
         
         connection.socket.Dispose();
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Failed to close websocket connection.");
      }
   }

   /// <inheritdoc />
   public async Task BroadcastAsync(string channel, ITurboAction turboAction, CancellationToken ct = default)
   {
      var connections = _connections.Where(x => x.Value.channel == channel);
      var message = await turboAction.Render();
      var buffer = Encoding.UTF8.GetBytes(message.Value!);

      foreach (var connection in connections)
      {
         _logger.LogInformation("Sending {Action} to channel '{Channel}' with connection ID {ConnectionId}", turboAction.GetType().Name, channel, connection.Key.Value);
         
         var socket = connection.Value.socket;
         try
         {
            await socket.SendAsync(buffer, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.DisableCompression, ct);
         }
         catch (WebSocketException)
         {
            _logger.LogWarning("Error while sending {Action} to channel '{Channel} with connection ID {ConnectionId}. Closing connection.", turboAction.GetType().Name, channel, connection.Key.Value);
            await RemoveConnection(connection.Key, ct);
         }
      }
   }
}