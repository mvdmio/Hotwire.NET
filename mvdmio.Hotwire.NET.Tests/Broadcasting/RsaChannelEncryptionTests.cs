using FluentAssertions;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class RsaChannelEncryptionTests : IDisposable
{
   private readonly RsaChannelEncryption _sut;

   public RsaChannelEncryptionTests()
   {
      _sut = new RsaChannelEncryption();
   }

   [Fact]
   public void EncryptAndDecrypt()
   {
      var channelName = "test";
      var encryptedChannelName = _sut.Encrypt(channelName);
      var decryptedChannelName = _sut.Decrypt(encryptedChannelName);
      
      decryptedChannelName.Should().Be(channelName);
   }

   [Fact]
   public async Task MultipleThreads()
   {
      var tasks = new List<Task>();
      for(var i = 0; i < 100; i++)
      {
         var channelName = "test" + i; // Unique per task to avoid overlap
         
         tasks.Add(
            Task.Run(
               () => {
                  var encrypted = _sut.Encrypt(channelName);
                  var decrypted = _sut.Decrypt(encrypted);
                  decrypted.Should().Be(channelName);
               }
            )
         );
      }
      
      await Task.WhenAll(tasks);
   }

   public void Dispose()
   {
      _sut.Dispose();
   }
}