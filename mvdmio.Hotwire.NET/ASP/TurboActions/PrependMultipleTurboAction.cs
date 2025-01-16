using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Prepends the content within the template tag to the container designated by the target dom id.
///    If the template’s first element has an id that is already used by a direct child inside the container targeted by dom_id, it is replaced instead of prepended.
///    https://turbo.hotwired.dev/reference/streams#prepend
/// </summary>
[PublicAPI]
public sealed class PrependMultipleTurboAction : ITurboAction
{
   private readonly string _targetsCssSelector;
   private readonly HtmlString _template;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetsCssSelector">The CSS selector for the dom elements that should be targeted.</param>
   /// <param name="template">The HTML template that should be used.</param>
   public PrependMultipleTurboAction(string targetsCssSelector, HtmlString template)
   {
      _targetsCssSelector = targetsCssSelector;
      _template = template;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      return Task.FromResult(new HtmlString(
         $"""
          <turbo-stream action="prepend" targets="{_targetsCssSelector}">
             <template>
                {_template}
             </template>
          </turbo-stream>
          """
      ));
   }
}