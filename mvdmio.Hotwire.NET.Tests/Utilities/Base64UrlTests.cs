using System.Net;
using System.Text;
using FluentAssertions;
using mvdmio.Hotwire.NET.Utilities;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Utilities;

public class Base64UrlTests
{
   [Fact]
   public void EncodeAndDecode()
   {
      const string data = "Hello, World!";
      var encoded = Base64Url.Encode(Encoding.UTF8.GetBytes(data));
      var decoded = Encoding.UTF8.GetString(Base64Url.Decode(encoded));

      decoded.Should().Be(data);
   }

   [Fact]
   public void ShouldNotNeedUrlEncoding()
   {
      const string data = "Hello, World!";
      var encoded = Base64Url.Encode(Encoding.UTF8.GetBytes(data));
      var urlEncoded = WebUtility.UrlEncode(encoded);

      encoded.Should().Be(urlEncoded);
   }
}