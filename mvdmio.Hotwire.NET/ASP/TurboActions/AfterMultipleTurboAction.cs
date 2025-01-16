using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Inserts the content within the template tag after the element designated by the target dom id.
///    https://turbo.hotwired.dev/reference/streams#after
/// </summary>
[PublicAPI]
public sealed class AfterMultipleTurboAction : ITurboAction
{
   private readonly string _targetsCssSelector;
   private readonly HtmlString _template;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetsCssSelector">The CSS selector for the dom elements that should be targeted.</param>
   /// <param name="template">The HTML template that should be used.</param>
   public AfterMultipleTurboAction(string targetsCssSelector, HtmlString template)
   {
      _targetsCssSelector = targetsCssSelector;
      _template = template;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      return Task.FromResult(new HtmlString(
         $"""
          <turbo-stream action="after" targets="{_targetsCssSelector}">
             <template>
                {_template}
             </template>
          </turbo-stream>
          """
      ));
   }
}