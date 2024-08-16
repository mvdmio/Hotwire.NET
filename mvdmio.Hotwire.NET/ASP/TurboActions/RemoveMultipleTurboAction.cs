using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Removes the elements designated by the targets CSS selector.
///    https://turbo.hotwired.dev/reference/streams#remove
/// </summary>
[PublicAPI]
public sealed class RemoveMultipleTurboAction : ITurboAction
{
   private readonly string _targetsCssSelector;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="targetsCssSelector">The CSS selector for the dom elements that should be targeted.</param>
   public RemoveMultipleTurboAction(string targetsCssSelector)
   {
      _targetsCssSelector = targetsCssSelector;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      return Task.FromResult(new HtmlString($"<turbo-stream action=\"remove\" targets=\"{_targetsCssSelector}\"></turbo-stream>"));
   }
}