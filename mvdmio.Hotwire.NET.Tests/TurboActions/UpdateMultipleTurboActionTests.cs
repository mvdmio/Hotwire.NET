using FluentAssertions;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.Enums;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class UpdateMultipleTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string targetsSelector = ".selector";
      const string template = "<test>template</test>";
      
      var action = new UpdateMultipleTurboAction(targetsSelector, new HtmlString(template));

      var result = await action.Render();

      result.Value.Should().Be(
          """
          <turbo-stream action="update" targets=".selector">
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
      
      var action = new UpdateMultipleTurboAction(targetsSelector, new HtmlString(template), TurboReplaceMethod.Morph);

      var result = await action.Render();

      result.Value.Should().Be(
         """
         <turbo-stream action="update" method="morph" targets=".selector">
            <template>
               <test>template</test>
            </template>
         </turbo-stream>
         """
      );
   }
}