using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Removes the element designated by the target dom id.
///    https://turbo.hotwired.dev/reference/streams#remove
/// </summary>
[PublicAPI]
public sealed class RemoveTurboAction : ITurboAction
{
   private readonly string _targetDomId;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetDomId">The DOM ID of the element that should be targeted.</param>
   public RemoveTurboAction(string targetDomId)
   {
      _targetDomId = targetDomId;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      return Task.FromResult(new HtmlString($"<turbo-stream action=\"remove\" target=\"{_targetDomId}\"></turbo-stream>"));
   }
}