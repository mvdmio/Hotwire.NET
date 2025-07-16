using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

/// <summary>
///   Interface for Turbo Streams broadcasters.
/// </summary>
public interface ITurboBroadcaster
{
   /// <summary>
   ///   Adds a connection to broadcast to. Normally handled by the library internally.
   /// </summary>
   internal Task<Guid> AddConnection(string channel, WebSocket webSocket);
   
   /// <summary>
   ///   Removes a connection from the broadcaster. This is typically called when the WebSocket connection is closed.
   /// </summary>
   internal Task RemoveConnection(Guid connectionId);
   
   /// <summary>
   ///   Broadcasts a Turbo Action to a channel.
   /// </summary>
   Task BroadcastAsync(string channel, ITurboAction turboAction, CancellationToken ct = default);
   
   /// <summary>
   ///   Broadcast a <see cref="RefreshTurboAction"/> to the channel.
   /// </summary>
   async Task BroadcastRefreshAsync(string channel, CancellationToken ct = default)
   {
      await BroadcastAsync(channel, new RefreshTurboAction(), ct);
   }
}