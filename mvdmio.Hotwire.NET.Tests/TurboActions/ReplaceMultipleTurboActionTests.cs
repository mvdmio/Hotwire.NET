using FluentAssertions;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class ReplaceMultipleTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string targetsSelector = ".selector";
      const string template = "<test>template</test>";
      
      var action = new ReplaceMultipleTurboAction(targetsSelector, new HtmlString(template));

      var result = await action.Render();

      result.Value.Should().Be(
          """
          <turbo-stream action="replace" targets=".selector">
             <template>
                <test>template</test>
             </template>
          </turbo-stream>
          """
      );
   }
   
   [Fact]
   public async Task WithAdditionalMorphParameter()
   {
      const string targetsSelector = ".selector";
      const string template = "<test>template</test>";
      
      var action = new ReplaceMultipleTurboAction(targetsSelector, new HtmlString(template), TurboReplaceMethod.Morph);

      var result = await action.Render();

      result.Value.Should().Be(
         """
         <turbo-stream action="replace" method="morph" targets=".selector">
            <template>
               <test>template</test>
            </template>
         </turbo-stream>
         """
      );
   }
}