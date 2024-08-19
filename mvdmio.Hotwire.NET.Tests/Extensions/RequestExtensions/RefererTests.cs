using FluentAssertions;
using Microsoft.AspNetCore.Http;
using mvdmio.Hotwire.NET.ASP.Extensions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Extensions.RequestExtensions;

public class RefererTests
{
   private readonly HttpRequest _request;
   
   public RefererTests()
   {
      _request = new DefaultHttpContext().Request;
   }
   
   [Fact]
   public void NoHeadersSet()
   {
      var result = _request.Referer();
      result.Should().BeNull();
   }
   
   [Fact]
   public void RefererHeaderSet()
   {
      _request.Headers.Append("Referer", "https://jewelsoftware.com");
      
      var result = _request.Referer();
      result.Should().NotBeNull();
      result!.Value.AbsoluteUrl.Should().Be("https://jewelsoftware.com/");
   }
   
   [Fact]
   public void TurboRefererHeaderSet()
   {
      _request.Headers.Append("turbo-referrer", "https://jewelsoftware.com/turbo");
      
      var result = _request.Referer();
      result.Should().NotBeNull();
      result!.Value.AbsoluteUrl.Should().Be("https://jewelsoftware.com/turbo");
   }
   
   [Fact]
   public void BothHeadersSet()
   {
      _request.Headers.Append("Referer", "https://jewelsoftware.com");
      _request.Headers.Append("turbo-referrer", "https://jewelsoftware.com/turbo");
      
      var result = _request.Referer();
      result.Should().NotBeNull();
      result!.Value.AbsoluteUrl.Should().Be("https://jewelsoftware.com/");
   }
}