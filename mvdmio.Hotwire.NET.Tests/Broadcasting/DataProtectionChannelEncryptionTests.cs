using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class DataProtectionChannelEncryptionTests : IDisposable
{
   private readonly TemporaryKeyDirectory _keyDirectory = new();
   private readonly TemporaryKeyDirectory _foreignKeyDirectory = new();
   private readonly IDataProtectionProvider _provider;
   private readonly DataProtectionChannelEncryption _sut;

   public DataProtectionChannelEncryptionTests()
   {
      _provider = DataProtectionProvider.Create(_keyDirectory.Directory);
      _sut = new DataProtectionChannelEncryption(_provider);
   }

   [Theory]
   [InlineData("test")]
   [InlineData("record-page:2:Requirement:iso-27001-2022|5.2")]
   [InlineData("eu-gdpr-2016|Art. 5(1)(a)")]
   [InlineData("überprüfung-ærø-日本語")]
   public void Decrypt_ShouldReturnOriginalChannelName(string channelName)
   {
      var token = _sut.Encrypt(channelName);

      _sut.Decrypt(token).Should().Be(channelName);
   }

   [Theory]
   [InlineData("test")]
   [InlineData("record-page:2:Requirement:iso-27001-2022|5.2")]
   [InlineData("eu-gdpr-2016|Art. 5(1)(a)")]
   [InlineData("überprüfung-ærø-日本語")]
   public void Encrypt_ShouldReturnPathSafeToken(string channelName)
   {
      var token = _sut.Encrypt(channelName);

      Uri.EscapeDataString(token).Should().Be(token);
   }

   [Fact]
   public void Decrypt_ShouldThrowArgumentException_WhenTokenComesFromForeignKeyRing()
   {
      var foreignSut = new DataProtectionChannelEncryption(DataProtectionProvider.Create(_foreignKeyDirectory.Directory));
      var token = foreignSut.Encrypt("test");

      var act = () => _sut.Decrypt(token);

      act.Should().ThrowExactly<ArgumentException>();
   }

   [Fact]
   public void Decrypt_ShouldThrowArgumentException_WhenTokenIsEdited()
   {
      var token = _sut.Encrypt("test");
      var index = token.Length / 2; // Not the last character: its low bits can be unused.
      var replacement = token[index] == 'A' ? 'B' : 'A';
      var editedToken = token[..index] + replacement + token[(index + 1)..];

      var act = () => _sut.Decrypt(editedToken);

      act.Should().ThrowExactly<ArgumentException>();
   }

   [Theory]
   [InlineData("abc.def")]
   [InlineData("abc def")]
   [InlineData("test.c2lnbmF0dXJl")]
   public void Decrypt_ShouldThrowArgumentException_WhenInputIsNotBase64Url(string input)
   {
      var act = () => _sut.Decrypt(input);

      act.Should().ThrowExactly<ArgumentException>();
   }

   [Fact]
   public void Decrypt_ShouldThrowArgumentException_WhenPayloadWasProtectedForOtherPurpose()
   {
      var otherPurposeToken = _provider.CreateProtector("some-other-purpose").Protect("test");

      var act = () => _sut.Decrypt(otherPurposeToken);

      act.Should().ThrowExactly<ArgumentException>();
   }

   [Fact]
   public async Task EncryptAndDecrypt_ShouldRoundTrip_WhenCalledConcurrently()
   {
      var tasks = new List<Task>();
      for (var i = 0; i < 100; i++)
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
      _keyDirectory.Dispose();
      _foreignKeyDirectory.Dispose();
   }
}
