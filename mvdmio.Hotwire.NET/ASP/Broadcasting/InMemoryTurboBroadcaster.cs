using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Implements the Turbo Streams broadcaster with an in-memory storage of connections.
/// </summary>
public class InMemoryTurboBroadcaster : ITurboBroadcaster
{
   private readonly Dictionary<string, WebSocket> _connections;

   /// <summary>
   ///   Constructor.
   /// </summary>
   public InMemoryTurboBroadcaster()
   {
      _connections = new Dictionary<string, WebSocket>();
   }

   /// <inheritdoc />
   public Task AddConnection(string channel, WebSocket webSocket)
   {
      _connections.Add(channel, webSocket);
      return Task.CompletedTask;
   }
}