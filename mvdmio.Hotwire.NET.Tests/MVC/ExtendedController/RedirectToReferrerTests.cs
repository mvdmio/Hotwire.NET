using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using mvdmio.Hotwire.NET.ASP.MVC;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.MVC.ExtendedController;

public class RedirectToReferrerTests
{
   private readonly ControllerStub _controller;

   public RedirectToReferrerTests()
   {
      _controller = new ControllerStub();
   }

   [Fact]
   public void WhenReferrerNotSet()
   {
      _controller.Invoking(x => x.RedirectToReferer()).Should().Throw<InvalidOperationException>().WithMessage("No referer found.");
   }

   [Fact]
   public void WithHttpReferrer()
   {
      _controller.Request.Headers["Referer"] = "https://jewelsoftware.com";

      var result = _controller.RedirectToReferer();
      result.Should().BeOfType<RedirectResult>();
      
      var redirectResult = (RedirectResult)result;
      redirectResult.Url.Should().Be("https://jewelsoftware.com/");
   }

   [Fact]
   public void WithTurboReferrer()
   {
      _controller.Request.Headers["turbo-referrer"] = "https://jewelsoftware.com";

      var result = _controller.RedirectToReferer();
      result.Should().BeOfType<RedirectResult>();
      
      var redirectResult = (RedirectResult)result;
      redirectResult.Url.Should().Be("https://jewelsoftware.com/");
   }

   [Fact]
   public void WithUrlModification()
   {
      _controller.Request.Headers["Referer"] = "https://jewelsoftware.com?q1=value1&q2=value2";

      var result = _controller.RedirectToReferer(url => url.WithoutQueryParam("q1"));
      result.Should().BeOfType<RedirectResult>();
      
      var redirectResult = (RedirectResult)result;
      redirectResult.Url.Should().Be("https://jewelsoftware.com/?q2=value2");
   }
}