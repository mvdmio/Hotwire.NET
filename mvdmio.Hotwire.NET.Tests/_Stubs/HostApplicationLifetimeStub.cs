using Microsoft.Extensions.Hosting;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

public sealed class HostApplicationLifetimeStub : IHostApplicationLifetime
{
   public CancellationToken ApplicationStarted => CancellationToken.None;

   public CancellationToken ApplicationStopping => CancellationToken.None;

   public CancellationToken ApplicationStopped => CancellationToken.None;

   public void StopApplication()
   {
   }
}
