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
   ///   Broadcasts a Turbo Action to a channel.
   /// </summary>
   Task BroadcastAsync(string channel, ITurboAction turboAction, CancellationToken ct = default);
   
   /// <summary>
   ///   Broadcast a <see cref="RefreshTurboAction"/> to the channel.
   /// </summary>
   async Task BroadcastRefreshAsync(string channel, string? requestId = null, CancellationToken ct = default)
   {
      await BroadcastAsync(channel, new RefreshTurboAction(requestId), ct);
   }
}