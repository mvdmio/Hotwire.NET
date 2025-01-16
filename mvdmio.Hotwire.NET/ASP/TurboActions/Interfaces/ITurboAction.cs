using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;

/// <summary>
///    Interface for turbo actions.
///    For more information, see: https://turbo.hotwired.dev/reference/streams
/// </summary>
public interface ITurboAction
{
   /// <summary>
   ///    Render the turbo action.
   /// </summary>
   public Task<HtmlString> Render();
}