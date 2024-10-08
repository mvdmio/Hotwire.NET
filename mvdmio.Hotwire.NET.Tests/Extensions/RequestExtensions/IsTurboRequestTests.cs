﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using mvdmio.Hotwire.NET.ASP.Extensions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Extensions.RequestExtensions;

public class IsTurboRequestTests
{
   private readonly HttpRequest _request;
   
   public IsTurboRequestTests()
   {
      _request = new DefaultHttpContext().Request;
   }
   
   [Theory]
   [InlineData("text/plain")]
   [InlineData("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/png,image/svg+xml,*/*;q=0.8")]
   public void IsTurboRequest_ShouldReturnFalse(string acceptHeaderValue)
   {
      _request.Headers.Append("Accept", acceptHeaderValue);

      var result = _request.IsTurboRequest();

      result.Should().BeFalse();
   }
   
   [Theory]
   [InlineData("text/vnd.turbo-stream.html")]
   [InlineData("text/vnd.turbo-stream.html, text/html, application/xhtml+xml")]
   public void IsTurboRequest_ShouldReturnTrue(string acceptHeaderValue)
   {
      _request.Headers.Append("Accept", acceptHeaderValue);

      var result = _request.IsTurboRequest();

      result.Should().BeTrue();
   }
}