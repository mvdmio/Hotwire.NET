using System;

namespace mvdmio.Hotwire.NET.Utilities;

internal static class Base64Url
{
   public static string Encode(byte[] data)
   {
      return Convert.ToBase64String(data)
         .Replace('+', '-')
         .Replace('/', '_')
         .Replace("=", "");
   }

   public static byte[] Decode(string base64Url)
   {
      var base64 = base64Url
         .Replace('-', '+')
         .Replace('_', '/')
         .PadRight(base64Url.Length + (4 - base64Url.Length % 4) % 4, '=');

      return Convert.FromBase64String(base64);
   }
}