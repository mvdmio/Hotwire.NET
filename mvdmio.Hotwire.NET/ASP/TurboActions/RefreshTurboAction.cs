using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using mvdmio.Hotwire.NET.ASP.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.TurboActions;

/// <summary>
///    Initiates a Page Refresh to render new content with morphing.
///    Can optionally use a request-id to debounce multiple refreshes.
///    https://turbo.hotwired.dev/reference/streams#refresh
/// </summary>
[PublicAPI]
public sealed class RefreshTurboAction : ITurboAction
{
   private readonly string? _requestId;

   /// <summary>
   ///    Constructor.
   /// </summary>
   /// <param name="requestId">The request ID to use to debounce multiple refreshes.</param>
   public RefreshTurboAction(string? requestId = null)
   {
      _requestId = requestId;
   }

   /// <inheritdoc />
   public Task<HtmlString> Render()
   {
      var turboStreamTag = "<turbo-stream action=\"refresh\" request-id-placeholder></turbo-stream>";
      turboStreamTag = turboStreamTag.Replace(" request-id-placeholder", _requestId is null ? string.Empty : $" request-id=\"{_requestId}\"");
      
      return Task.FromResult(new HtmlString(turboStreamTag));
   }
}