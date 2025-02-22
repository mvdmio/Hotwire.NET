using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;
using mvdmio.Hotwire.NET.Utilities;

namespace mvdmio.Hotwire.NET.ASP.RazorPages;

/// <summary>
/// Extended base class for Page Models that contains Turbo-related methods.
/// </summary>
[PublicAPI]
public class ExtendedPageModel : PageModel
{
   /// <summary>
   ///    Create a <see cref="TurboStreamActionResult" /> object that renders a turbo stream to the response.
   /// </summary>
   protected TurboStreamActionResult TurboStream(params ITurboAction[] actions)
   {
      return PageModelExtensions.TurboStream(this, actions);
   }

   /// <summary>
   ///    Create a <see cref="TurboStreamActionResult" /> with a <see cref="RefreshTurboAction"/>.
   /// </summary>
   protected TurboStreamActionResult TurboStreamRefresh()
   {
      return PageModelExtensions.TurboStreamRefresh(this);
   }
   
   /// <summary>
   ///    Render a partial view to string.
   /// </summary>
   protected async Task<HtmlString> RenderView<TModel>(string viewNamePath, [DisallowNull] TModel model)
   {
      return await PageModelExtensions.RenderView(this, viewNamePath, model);
   }

   /// <summary>
   ///    Render a partial view to string, without a model present.
   /// </summary>
   protected async Task<HtmlString> RenderView(string viewNamePath)
   {
      return await PageModelExtensions.RenderView(this, viewNamePath);
   }
   
   /// <summary>
   /// Redirect back to the referer URL.
   /// </summary>
   protected IActionResult RedirectToReferer()
   {
      return PageModelExtensions.RedirectToReferer(this);
   }

   /// <summary>
   /// Redirect back to the referer URL, with modifications.
   /// </summary>
   protected IActionResult RedirectToReferer(Func<Url, Url> urlModificationAction)
   {
      return PageModelExtensions.RedirectToReferer(this, urlModificationAction);
   }
}