using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using mvdmio.Hotwire.NET.ASP.Extensions;
using mvdmio.Hotwire.NET.ASP.Interfaces;
using mvdmio.Hotwire.NET.Utilities;

namespace mvdmio.Hotwire.NET.ASP.RazorPages;

/// <summary>
/// Extensions for <see cref="PageModel"/>.
/// </summary>
[PublicAPI]
public static class PageModelExtensions
{
   /// <summary>
   ///    Create a <see cref="TurboStreamActionResult" /> object that renders a turbo stream to the response.
   /// </summary>
   public static TurboStreamActionResult TurboStream(this PageModel pageModel, params ITurboAction[] actions)
   {
      return new TurboStreamActionResult(actions);
   }

   /// <summary>
   ///    Render a partial view to string.
   /// </summary>
   public static async Task<HtmlString> RenderView<TModel>(this PageModel pageModel, string viewNamePath, TModel model)
   {
      var viewResult = LoadView(pageModel, viewNamePath);

      if (viewResult.View is null)
         throw new InvalidOperationException("View is not loaded");

      await using var writer = new StringWriter();

      var viewContext = new ViewContext(
         pageModel.PageContext,
         viewResult.View,
         new ViewDataDictionary<TModel>(pageModel.ViewData, model),
         pageModel.TempData,
         writer,
         new HtmlHelperOptions()
      );

      await viewResult.View.RenderAsync(viewContext);
      return new HtmlString(writer.GetStringBuilder().ToString());
   }

   /// <summary>
   ///    Render a partial view to string, without a model present.
   /// </summary>
   public static async Task<HtmlString> RenderView(this PageModel pageModel, string viewNamePath)
   {
      var viewResult = LoadView(pageModel, viewNamePath);

      if (viewResult.View is null)
         throw new InvalidOperationException("View is not loaded");

      await using var writer = new StringWriter();

      var viewContext = new ViewContext(
         pageModel.PageContext,
         viewResult.View,
         pageModel.ViewData,
         pageModel.TempData,
         writer,
         new HtmlHelperOptions()
      );

      await viewResult.View.RenderAsync(viewContext);
      return new HtmlString(writer.GetStringBuilder().ToString());
   }

   /// <summary>
   /// Redirect back to the referer URL.
   /// </summary>
   public static IActionResult RedirectToReferer(this PageModel pageModel)
   {
      var referer = pageModel.Request.Referer();

      if(referer is null)
         throw new InvalidOperationException("No referer found.");

      return new RedirectResult(referer.Value);
   }

   /// <summary>
   /// Redirect back to the referer URL, with modifications.
   /// </summary>
   public static IActionResult RedirectToReferer(this PageModel pageModel, Action<Url> urlModificationAction)
   {
      var referer = pageModel.Request.Referer();

      if(referer is null)
         throw new InvalidOperationException("No referer found.");

      urlModificationAction.Invoke(referer.Value);

      return new RedirectResult(referer.Value);
   }

   private static ViewEngineResult LoadView(PageModel pageModel, string viewNamePath)
   {
      var viewEngine = pageModel.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

      if (viewEngine is null)
         throw new InvalidOperationException("Could not load view engine from request.");

      var viewResult = viewEngine.GetView(executingFilePath: null, viewPath: viewNamePath, isMainPage: false);
      if (!viewResult.Success)
         viewResult = viewEngine.FindView(pageModel.PageContext, viewNamePath, isMainPage: false);
      
      if (!viewResult.Success)
         throw new InvalidOperationException($"A view with the name '{viewNamePath}' could not be found");

      return viewResult;
   }
}