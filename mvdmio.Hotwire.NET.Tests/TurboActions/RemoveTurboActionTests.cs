using FluentAssertions;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class RemoveTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string target = "test_id";
      var action = new RemoveTurboAction(target);

      var result = await action.Render();

      result.Value.Should().Be("<turbo-stream action=\"remove\" target=\"test_id\"></turbo-stream>");
   }
}