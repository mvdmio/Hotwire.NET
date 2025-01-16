using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Inserts the content within the template tag before the element designated by the target dom id.
///    https://turbo.hotwired.dev/reference/streams#before
/// </summary>
[PublicAPI]
public sealed class BeforeTurboAction : ITurboAction
{
   private readonly string _targetDomId;
   private readonly HtmlString _template;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetDomId">The DOM ID of the element that should be targeted.</param>
   /// <param name="template">The HTML template that should be used.</param>
   public BeforeTurboAction(string targetDomId, HtmlString template)
   {
      _targetDomId = targetDomId;
      _template = template;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      return Task.FromResult(new HtmlString(
         $"""
          <turbo-stream action="before" target="{_targetDomId}">
             <template>
                {_template}
             </template>
          </turbo-stream>
          """
      ));
   }
}