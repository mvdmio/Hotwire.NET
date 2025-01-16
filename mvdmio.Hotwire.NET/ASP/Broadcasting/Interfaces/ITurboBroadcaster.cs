using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

/// <summary>
/// Interface for Turbo Streams broadcasters.
/// </summary>
public interface ITurboBroadcaster
{
   internal Task AddConnection(string channel, WebSocket webSocket);
   
   /// <summary>
   ///   Broadcasts a Turbo Action to a channel.
   /// </summary>
   public Task BroadcastAsync(string channel, ITurboAction turboAction, CancellationToken ct = default);
   
   /// <summary>
   ///   Broadcast a <see cref="RefreshTurboAction"/> to the channel.
   /// </summary>
   public async Task BroadcastRefreshAsync(string channel, CancellationToken ct = default)
   {
      await BroadcastAsync(channel, new RefreshTurboAction(), ct);
   }
}