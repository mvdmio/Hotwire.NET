using FluentAssertions;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class RemoveMultipleTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string targetsSelector = ".selector";
      var action = new RemoveMultipleTurboAction(targetsSelector);

      var result = await action.Render();

      result.Value.Should().Be("<turbo-stream action=\"remove\" targets=\".selector\"></turbo-stream>");
   }
}