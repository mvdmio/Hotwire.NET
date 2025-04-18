﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions.Enums;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Replaces the element designated by the target dom id.
///    The [method="morph"] attribute can be added to the turbo-stream element to replace the element designated by the target dom id via morph.
///    https://turbo.hotwired.dev/reference/streams#replace
/// </summary>
[PublicAPI]
public sealed class ReplaceTurboAction : ITurboAction
{
   private readonly string _targetDomId;
   private readonly HtmlString _template;
   private readonly TurboReplaceMethod _method;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetDomId">The DOM ID of the element that should be targeted.</param>
   /// <param name="template">The HTML template that should be used.</param>
   /// <param name="method">The replacement method that should be used.</param>
   public ReplaceTurboAction(string targetDomId, HtmlString template, TurboReplaceMethod method = TurboReplaceMethod.None)
   {
      _targetDomId = targetDomId;
      _template = template;
      _method = method;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      var template = $"""
                      <turbo-stream action="replace" method_placeholder target="{_targetDomId}">
                         <template>
                            {_template}
                         </template>
                      </turbo-stream>
                      """;

      template = template.Replace("method_placeholder ", _method == TurboReplaceMethod.None ? string.Empty : "method=\"morph\" ");

      return Task.FromResult(new HtmlString(template));
   }
}