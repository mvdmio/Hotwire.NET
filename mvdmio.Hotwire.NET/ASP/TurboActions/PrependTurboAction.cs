using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Prepends the content within the template tag to the container designated by the target dom id.
///    If the template’s first element has an id that is already used by a direct child inside the container targeted by dom_id, it is replaced instead of prepended.
///    https://turbo.hotwired.dev/reference/streams#prepend
/// </summary>
[PublicAPI]
public sealed class PrependTurboAction : ITurboAction
{
   private readonly string _targetDomId;
   private readonly HtmlString _template;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetDomId">The DOM ID of the element that should be targeted.</param>
   /// <param name="template">The HTML template that should be used.</param>
   public PrependTurboAction(string targetDomId, HtmlString template)
   {
      _targetDomId = targetDomId;
      _template = template;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      return Task.FromResult(new HtmlString(
         $"""
          <turbo-stream action="prepend" target="{_targetDomId}">
             <template>
                {_template}
             </template>
          </turbo-stream>
          """
      ));
   }
}