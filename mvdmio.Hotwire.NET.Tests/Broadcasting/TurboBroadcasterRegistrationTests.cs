using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Extensions;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboBroadcasterRegistrationTests
{
   [Fact]
   public void AddTurboStreamsBroadcasting_ShouldResolveOneInstanceForBothBroadcasterTypes()
   {
      using var provider = CreateServices().BuildServiceProvider();

      provider.GetRequiredService<ITurboBroadcaster>().Should().BeSameAs(provider.GetRequiredService<InMemoryTurboBroadcaster>());
   }

   [Fact]
   public void AddTurboStreamsBroadcasting_ShouldKeepRegistryShared_WhenAppRegistersDecoratorAfterwards()
   {
      var services = CreateServices();
      services.AddSingleton<ITurboBroadcaster>(sp => new RecordingTurboBroadcaster(sp.GetRequiredService<InMemoryTurboBroadcaster>()));
      using var provider = services.BuildServiceProvider();
      using var scope = provider.CreateScope();

      scope.ServiceProvider.GetRequiredService<ITurboBroadcaster>().Should().BeOfType<RecordingTurboBroadcaster>();
      scope.ServiceProvider.GetRequiredService<TurboStreamsWebsocketMiddleware>().Should().NotBeNull();
      scope.ServiceProvider.GetRequiredService<InMemoryTurboBroadcaster>().Should().BeSameAs(provider.GetRequiredService<InMemoryTurboBroadcaster>());
   }

   private static ServiceCollection CreateServices()
   {
      var services = new ServiceCollection();
      services.AddLogging();
      services.AddSingleton<IHostApplicationLifetime, HostApplicationLifetimeStub>();
      services.AddTurboStreamsBroadcasting();
      services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());

      return services;
   }
}
