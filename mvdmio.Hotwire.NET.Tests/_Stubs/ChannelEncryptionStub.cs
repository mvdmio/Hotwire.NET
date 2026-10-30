using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

/// <summary>
///   Channel encryption with a known signed value: the channel name prefixed with <c>signed-</c>.
/// </summary>
public sealed class ChannelEncryptionStub : IChannelEncryption
{
   private const string _prefix = "signed-";

   public string Encrypt(string channelName)
   {
      return _prefix + channelName;
   }

   public string Decrypt(string signedChannelName)
   {
      if (!signedChannelName.StartsWith(_prefix, StringComparison.Ordinal))
         throw new ArgumentException("Invalid signature", nameof(signedChannelName));

      return signedChannelName[_prefix.Length..];
   }
}
