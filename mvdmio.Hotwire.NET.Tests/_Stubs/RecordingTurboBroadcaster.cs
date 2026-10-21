using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

/// <summary>
///   An <see cref="ITurboBroadcaster" /> written the way an app developer would write one: it records every broadcast and forwards it to the wrapped broadcaster.
/// </summary>
public sealed class RecordingTurboBroadcaster(ITurboBroadcaster inner) : ITurboBroadcaster
{
   public List<(string Channel, ITurboAction Action)> Broadcasts { get; } = [];

   public Task BroadcastAsync(string channel, ITurboAction turboAction, CancellationToken ct = default)
   {
      Broadcasts.Add((channel, turboAction));
      return inner.BroadcastAsync(channel, turboAction, ct);
   }
}
