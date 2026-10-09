using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Extensions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Extensions.AspWebExtensions;

public sealed class AddTurboStreamsBroadcastingTests : IDisposable
{
   private readonly DirectoryInfo _keyDirectory;

   public AddTurboStreamsBroadcastingTests()
   {
      _keyDirectory = Directory.CreateTempSubdirectory("hotwire-dp-keys-");
   }

   [Fact]
   public void Decrypt_ShouldAcceptTokenSignedBeforeRestart()
   {
      const string channelName = "record-page:2:Requirement:iso-27001-2022|5.2";
      string token;

      using (var providerBeforeRestart = BuildServiceProvider())
      {
         token = providerBeforeRestart.GetRequiredService<IChannelEncryption>().Encrypt(channelName);
      }

      using var providerAfterRestart = BuildServiceProvider();
      var decrypted = providerAfterRestart.GetRequiredService<IChannelEncryption>().Decrypt(token);

      decrypted.Should().Be(channelName);
   }

   [Fact]
   public void ShouldRegisterDataProtectionChannelEncryption()
   {
      var services = new ServiceCollection();
      services.AddTurboStreamsBroadcasting();

      using var provider = services.BuildServiceProvider();

      provider.GetRequiredService<IChannelEncryption>().Should().BeOfType<DataProtectionChannelEncryption>();
   }

   [Fact]
   public void ShouldLetLaterChannelEncryptionRegistrationWin()
   {
      var services = new ServiceCollection();
      services.AddTurboStreamsBroadcasting();
      services.AddSingleton<IChannelEncryption, RsaChannelEncryption>();

      using var provider = services.BuildServiceProvider();

      provider.GetRequiredService<IChannelEncryption>().Should().BeOfType<RsaChannelEncryption>();
   }

   public void Dispose()
   {
      _keyDirectory.Delete(recursive: true);
   }

   private ServiceProvider BuildServiceProvider()
   {
      var services = new ServiceCollection();
      services.AddTurboStreamsBroadcasting();
      services.AddDataProtection().PersistKeysToFileSystem(_keyDirectory);

      return services.BuildServiceProvider();
   }
}
