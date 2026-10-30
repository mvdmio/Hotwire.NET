using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Broadcasting.TagHelpers;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboBroadcastChannelHtmlHelperTests
{
   [Fact]
   public void TurboBroadcastChannel_ShouldIncludePathBaseInSrc_WhenRequestHasPathBase()
   {
      var httpContext = CreateHttpContext();
      httpContext.Request.Scheme = "https";
      httpContext.Request.Host = new HostString("demo.mvdm.io");
      httpContext.Request.PathBase = "/compliance";

      var result = CreateHtmlHelper(httpContext).TurboBroadcastChannel("channel-name");

      result.Should().Be("""<turbo-stream-source src="wss://demo.mvdm.io/compliance/turbo/ws/signed-channel-name"></turbo-stream-source>""");
   }

   [Fact]
   public void TurboBroadcastChannel_ShouldRenderSrcWithoutPathBase_WhenRequestHasNoPathBase()
   {
      var httpContext = CreateHttpContext();
      httpContext.Request.Scheme = "http";
      httpContext.Request.Host = new HostString("localhost", 5000);

      var result = CreateHtmlHelper(httpContext).TurboBroadcastChannel("channel-name");

      result.Should().Be("""<turbo-stream-source src="ws://localhost:5000/turbo/ws/signed-channel-name"></turbo-stream-source>""");
   }

   private static DefaultHttpContext CreateHttpContext()
   {
      var services = new ServiceCollection();
      services.AddSingleton<IChannelEncryption, ChannelEncryptionStub>();

      return new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
   }

   private static IHtmlHelper CreateHtmlHelper(HttpContext httpContext)
   {
      return HtmlHelperStub.Create(new ViewContext { HttpContext = httpContext });
   }
}
