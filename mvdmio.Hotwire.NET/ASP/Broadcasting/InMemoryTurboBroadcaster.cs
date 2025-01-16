using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Implements the Turbo Streams broadcaster with an in-memory storage of connections.
/// </summary>
public class InMemoryTurboBroadcaster : ITurboBroadcaster
{
   private readonly IList<(string channel, WebSocket socket)> _connections;

   /// <summary>
   ///   Constructor.
   /// </summary>
   public InMemoryTurboBroadcaster()
   {
      _connections = new List<(string channel, WebSocket socket)>();
   }

   /// <inheritdoc />
   public Task AddConnection(string channel, WebSocket webSocket)
   {
      _connections.Add((channel, webSocket));
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
}