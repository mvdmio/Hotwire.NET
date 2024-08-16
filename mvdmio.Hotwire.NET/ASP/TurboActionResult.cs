using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mvdmio.Hotwire.NET.ASP.Interfaces;

namespace mvdmio.Hotwire.NET.ASP;

/// <summary>
///    Class for returning Turbo Streams action results.
///    For more information, see: https://turbo.hotwired.dev/reference/streams
/// </summary>
[PublicAPI]
public class TurboActionResult : IActionResult
{
   /// <summary>
   ///    The actions that should be rendered in the HTTP response.
   /// </summary>
   public IEnumerable<ITurboAction> Actions { get; }

   /// <summary>
   ///    Constructor.
   /// </summary>
   public TurboActionResult(IEnumerable<ITurboAction> actions)
   {
      Actions = actions;
   }

   /// <inheritdoc />
   public async Task ExecuteResultAsync(ActionContext context)
   {
      // For more info, see: https://turbo.hotwired.dev/handbook/streams#streaming-from-http-responses
      context.HttpContext.Response.Headers["Content-Type"] = "text/vnd.turbo-stream.html";
      
      foreach (var action in Actions)
      {
         var renderedAction = await action.Render();

         if (renderedAction.Value is not null)
            await context.HttpContext.Response.WriteAsync(renderedAction.Value);
      }
   }
}