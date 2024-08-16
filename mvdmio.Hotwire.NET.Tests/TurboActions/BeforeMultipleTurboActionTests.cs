﻿using FluentAssertions;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.TurboActions;

public class BeforeMultipleTurboActionTests
{
   [Fact]
   public async Task WithRequiredParameters()
   {
      const string targetsSelector = ".selector";
      const string template = "<test>template</test>";
      
      var action = new BeforeMultipleTurboAction(targetsSelector, new HtmlString(template));

      var result = await action.Render();

      result.Value.Should().Be(
          """
          <turbo-stream action="before" targets=".selector">
             <template>
                <test>template</test>
             </template>
          </turbo-stream>
          """
      );
   }
}