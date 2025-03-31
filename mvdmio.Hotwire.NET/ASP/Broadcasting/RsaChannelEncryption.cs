using System;
using System.Security.Cryptography;
using System.Text;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.Utilities;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   In-memory implementation of channel encryption.
///   This implementation recycles the encryption key-pair every time the application is restarted.
/// </summary>
public sealed class RsaChannelEncryption : IChannelEncryption, IDisposable
{
   // RSA SignHash is NOT thread-safe. You must lock around it.
   // You mush also use the same instance of RSA for both signing and verifying, otherwise the signature will not match.
   
   private readonly object _lock = new();
   private readonly RSA _rsa;
   
   /// <summary>
   ///   Constructor.
   /// </summary>
   public RsaChannelEncryption()
   {
      _rsa = RSA.Create();
   }
   
   /// <inheritdoc />
   public string Encrypt(string channelName)
   {
      lock (_lock)
      {
         var data = Encoding.UTF8.GetBytes(channelName);
         var hash = SHA512.HashData(data);
         var signatureBytes = _rsa.SignHash(hash, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
      
         var signature = Base64Url.Encode(signatureBytes);

         return $"{channelName}.{signature}";   
      }
   }

   /// <inheritdoc />
   public string Decrypt(string signedChannelName)
   {
      var parts = signedChannelName.Split('.');
      if (parts.Length != 2)
         throw new ArgumentException("Invalid channel name", nameof(signedChannelName));

      var channelName = parts[0];
      var signature = parts[1];
      var data = Encoding.UTF8.GetBytes(channelName);
      
      var hash = SHA512.HashData(data);
      var signatureBytes = Base64Url.Decode(signature);
      
      // ReSharper disable once InconsistentlySynchronizedField -- VerifyHash is thread-safe, SignHash is not
      var isValid = _rsa.VerifyHash(hash, signatureBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

      if (!isValid)
         throw new ArgumentException("Invalid channel name", nameof(signedChannelName));

      return channelName;
   }

   /// <inheritdoc />
   public void Dispose()
   {
      _rsa.Dispose();
   }
}