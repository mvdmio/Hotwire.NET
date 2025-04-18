﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions.Enums;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Updates the content within the template tag to the container designated by the target dom id.
///    The [method="morph"] attribute can be added to the turbo-stream element to morph only the children of the element designated by the target dom id.
///    https://turbo.hotwired.dev/reference/streams#update
/// </summary>
[PublicAPI]
public sealed class UpdateMultipleTurboAction : ITurboAction
{
   private readonly string _targetsCssSelector;
   private readonly HtmlString _template;
   private readonly TurboReplaceMethod _method;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetsCssSelector">The CSS selector for the dom elements that should be targeted.</param>
   /// <param name="template">The HTML template that should be used.</param>
   /// <param name="method">The replacement method that should be used.</param>
   public UpdateMultipleTurboAction(string targetsCssSelector, HtmlString template, TurboReplaceMethod method = TurboReplaceMethod.None)
   {
      _targetsCssSelector = targetsCssSelector;
      _template = template;
      _method = method;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      var template = $"""
                      <turbo-stream action="update" method_placeholder targets="{_targetsCssSelector}">
                         <template>
                            {_template}
                         </template>
                      </turbo-stream>
                      """;

      template = template.Replace("method_placeholder ", _method == TurboReplaceMethod.None ? string.Empty : "method=\"morph\" ");

      return Task.FromResult(new HtmlString(template));
   }
}