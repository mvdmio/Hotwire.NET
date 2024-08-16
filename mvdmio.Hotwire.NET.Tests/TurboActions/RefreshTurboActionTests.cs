using FluentAssertions;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class RefreshTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      var action = new RefreshTurboAction();

      var result = await action.Render();

      result.Value.Should().Be("<turbo-stream action=\"refresh\"></turbo-stream>");
   }
   
   [Fact]
   public async Task WithAdditionalRequestIdParameter()
   {
      const string requestId = "some-unique-id";
      var action = new RefreshTurboAction(requestId);

      var result = await action.Render();

      result.Value.Should().Be("<turbo-stream action=\"refresh\" request-id=\"some-unique-id\"></turbo-stream>");
   }
}