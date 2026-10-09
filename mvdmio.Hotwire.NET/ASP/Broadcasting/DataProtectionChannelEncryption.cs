using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
///   Channel encryption backed by ASP.NET Core Data Protection.
///   Signed channel names stay valid for as long as the application's Data Protection key ring does,
///   so they survive restarts and work across instances that share a key ring.
/// </summary>
public sealed class DataProtectionChannelEncryption : IChannelEncryption
{
   // Never change this value: changing it invalidates every signed channel name already issued.
   private const string Purpose = "mvdmio.Hotwire.NET.Broadcasting.Channel";

   private readonly IDataProtector _protector;

   /// <summary>
   ///   Constructor.
   /// </summary>
   public DataProtectionChannelEncryption(IDataProtectionProvider dataProtectionProvider)
   {
      ArgumentNullException.ThrowIfNull(dataProtectionProvider);

      _protector = dataProtectionProvider.CreateProtector(Purpose);
   }

   /// <inheritdoc />
   public string Encrypt(string channelName)
   {
      return _protector.Protect(channelName);
   }

   /// <inheritdoc />
   public string Decrypt(string signedChannelName)
   {
      try
      {
         return _protector.Unprotect(signedChannelName);
      }
      catch (CryptographicException ex)
      {
         throw new ArgumentException("Invalid channel name", nameof(signedChannelName), ex);
      }
      catch (FormatException ex)
      {
         throw new ArgumentException("Invalid channel name", nameof(signedChannelName), ex);
      }
   }
}
