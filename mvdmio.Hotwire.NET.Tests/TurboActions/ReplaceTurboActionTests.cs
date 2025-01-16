using FluentAssertions;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.ASP.TurboActions.Enums;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class ReplaceTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string target = "test_id";
      const string template = "<test>template</test>";
      
      var action = new ReplaceTurboAction(target, new HtmlString(template));

      var result = await action.Render();

      result.Value.Should().Be(
          """
          <turbo-stream action="replace" target="test_id">
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
      const string target = "test_id";
      const string template = "<test>template</test>";
      
      var action = new ReplaceTurboAction(target, new HtmlString(template), TurboReplaceMethod.Morph);

      var result = await action.Render();

      result.Value.Should().Be(
         """
         <turbo-stream action="replace" method="morph" target="test_id">
            <template>
               <test>template</test>
            </template>
         </turbo-stream>
         """
      );
   }
}