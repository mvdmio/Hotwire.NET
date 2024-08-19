using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using mvdmio.Hotwire.NET.ASP.MVC;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.RazorPages.ExtendedPageModel;

public class RedirectToReferrerTests
{
   private readonly PageModelStub _pageModelStub;

   public RedirectToReferrerTests()
   {
      _pageModelStub = new PageModelStub();
   }

   [Fact]
   public void WhenReferrerNotSet()
   {
      _pageModelStub.Invoking(x => x.TestRedirectToReferer()).Should().Throw<InvalidOperationException>().WithMessage("No referer found.");
   }

   [Fact]
   public void WithHttpReferrer()
   {
      _pageModelStub.Request.Headers["Referer"] = "https://jewelsoftware.com";

      var result = _pageModelStub.TestRedirectToReferer();
      result.Should().BeOfType<RedirectResult>();
      
      var redirectResult = (RedirectResult)result;
      redirectResult.Url.Should().Be("https://jewelsoftware.com/");
   }

   [Fact]
   public void WithTurboReferrer()
   {
      _pageModelStub.Request.Headers["turbo-referrer"] = "https://jewelsoftware.com";

      var result = _pageModelStub.TestRedirectToReferer();
      result.Should().BeOfType<RedirectResult>();
      
      var redirectResult = (RedirectResult)result;
      redirectResult.Url.Should().Be("https://jewelsoftware.com/");
   }

   [Fact]
   public void WithUrlModification()
   {
      _pageModelStub.Request.Headers["Referer"] = "https://jewelsoftware.com?q1=value1&q2=value2";

      var result = _pageModelStub.TestRedirectToReferer(url => url.WithoutQueryParam("q1"));
      result.Should().BeOfType<RedirectResult>();
      
      var redirectResult = (RedirectResult)result;
      redirectResult.Url.Should().Be("https://jewelsoftware.com/?q2=value2");
   }
}