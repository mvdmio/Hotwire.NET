using System;
using System.Security.Cryptography;
using System.Text;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   In-memory implementation of channel encryption.
///   This implementation recycles the encryption key-pair every time the application is restarted.
/// </summary>
public class InMemoryChannelEncryption : IChannelEncryption, IDisposable
{
   private readonly SHA512 _algorithm;
   private readonly RSA _rsa;
   private readonly RSAPKCS1SignatureFormatter _rsaFormatter;
   
   /// <summary>
   ///   Constructor.
   /// </summary>
   public InMemoryChannelEncryption()
   {
      _algorithm = SHA512.Create();
      _rsa = RSA.Create();
      
      _rsaFormatter = new RSAPKCS1SignatureFormatter(_rsa);
      _rsaFormatter.SetHashAlgorithm(nameof(SHA512));
   }
   
   /// <inheritdoc />
   public string Encrypt(string channelName)
   {
      var data = Encoding.UTF8.GetBytes(channelName);
      var hash = _algorithm.ComputeHash(data);
      var signatureBytes = _rsaFormatter.CreateSignature(hash);
      var signature = Convert.ToBase64String(signatureBytes);

      return $"{channelName}.{signature}";
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
      var hash = _algorithm.ComputeHash(data);
      var signatureBytes = Convert.FromBase64String(signature);
      var isValid = _rsa.VerifyHash(hash, signatureBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

      if (!isValid)
         throw new ArgumentException("Invalid channel name", nameof(signedChannelName));

      return channelName;
   }

   /// <inheritdoc />
   public void Dispose()
   {
      _rsa.Dispose();
      _algorithm.Dispose();
   }
}