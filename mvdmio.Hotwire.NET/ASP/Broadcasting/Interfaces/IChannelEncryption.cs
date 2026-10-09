namespace mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

/// <summary>
///   Interface for channel encryption.
/// </summary>
public interface IChannelEncryption
{
   /// <summary>
   /// Encrypt the channel name.
   /// </summary>
   string Encrypt(string channelName);
   
   /// <summary>
   /// Decrypt the channel name.
   /// </summary>
   /// <exception cref="System.ArgumentException">Thrown when <paramref name="signedChannelName"/> fails verification.</exception>
   string Decrypt(string signedChannelName);
}