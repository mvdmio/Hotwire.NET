using FluentAssertions;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class AppendTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string target = "test_id";
      const string template = "<test>template</test>";
      
      var action = new AppendTurboAction(target, new HtmlString(template));

      var result = await action.Render();

      result.Value.Should().Be(
          """
          <turbo-stream action="append" target="test_id">
             <template>
                <test>template</test>
             </template>
          </turbo-stream>
          """
      );
   }
}