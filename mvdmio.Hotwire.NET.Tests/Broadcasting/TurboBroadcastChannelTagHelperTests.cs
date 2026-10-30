using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using mvdmio.Hotwire.NET.ASP.Broadcasting.TagHelpers;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboBroadcastChannelTagHelperTests
{
   [Fact]
   public void Process_ShouldIncludePathBaseInSrc_WhenRequestHasPathBase()
   {
      var httpContext = new DefaultHttpContext();
      httpContext.Request.Scheme = "https";
      httpContext.Request.Host = new HostString("demo.mvdm.io");
      httpContext.Request.PathBase = "/compliance";

      var output = Render(httpContext);

      output.Content.GetContent().Should().Be("""<turbo-stream-source src="wss://demo.mvdm.io/compliance/turbo/ws/signed-channel-name"></turbo-stream-source>""");
   }

   [Fact]
   public void Process_ShouldRenderSrcWithoutPathBase_WhenRequestHasNoPathBase()
   {
      var httpContext = new DefaultHttpContext();
      httpContext.Request.Scheme = "http";
      httpContext.Request.Host = new HostString("localhost", 5000);

      var output = Render(httpContext);

      output.Content.GetContent().Should().Be("""<turbo-stream-source src="ws://localhost:5000/turbo/ws/signed-channel-name"></turbo-stream-source>""");
   }

   private static TagHelperOutput Render(HttpContext httpContext)
   {
      var sut = new TurboBroadcastChannelTagHelper(new HttpContextAccessor { HttpContext = httpContext }, new ChannelEncryptionStub()) { Name = "channel-name" };
      var context = new TagHelperContext([], new Dictionary<object, object>(), "test");
      var output = new TagHelperOutput("turbo-broadcast-channel", [], (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

      sut.Process(context, output);

      return output;
   }
}
