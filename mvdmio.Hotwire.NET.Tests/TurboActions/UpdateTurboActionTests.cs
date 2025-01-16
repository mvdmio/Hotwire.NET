using FluentAssertions;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class UpdateTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string target = "test_id";
      const string template = "<test>template</test>";
      
      var action = new UpdateTurboAction(target, new HtmlString(template));

      var result = await action.Render();

      result.Value.Should().Be(
          """
          <turbo-stream action="update" target="test_id">
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
      
      var action = new UpdateTurboAction(target, new HtmlString(template), TurboReplaceMethod.Morph);

      var result = await action.Render();

      result.Value.Should().Be(
         """
         <turbo-stream action="update" method="morph" target="test_id">
            <template>
               <test>template</test>
            </template>
         </turbo-stream>
         """
      );
   }
}