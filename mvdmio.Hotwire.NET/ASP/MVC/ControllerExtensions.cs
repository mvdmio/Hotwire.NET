using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using mvdmio.Hotwire.NET.ASP.Extensions;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;
using mvdmio.Hotwire.NET.Utilities;

namespace mvdmio.Hotwire.NET.ASP.MVC;

/// <summary>
///    Extensions for <see cref="Controller" />.
/// </summary>
[PublicAPI]
public static class ControllerExtensions
{
   /// <summary>
   ///    Create a <see cref="TurboStreamActionResult" /> object that renders a turbo stream to the response.
   /// </summary>
   public static TurboStreamActionResult TurboStream(this Controller controller, params ITurboAction[] actions)
   {
      return new TurboStreamActionResult(actions);
   }
   
   /// <summary>
   ///    Create a <see cref="TurboStreamActionResult" /> with a <see cref="RefreshTurboAction"/>.
   /// </summary>
   public static TurboStreamActionResult TurboStreamRefresh(this Controller pageModel)
   {
      return TurboStream(pageModel, new RefreshTurboAction());
   }

   /// <summary>
   ///    Render a partial view to string.
   /// </summary>
   public static async Task<HtmlString> RenderView<TModel>(this Controller controller, string viewNamePath, TModel model)
   {
      var viewResult = LoadView(controller, viewNamePath);

      if (viewResult.View is null)
         throw new InvalidOperationException("View is not loaded");

      await using var writer = new StringWriter();

      var viewContext = new ViewContext(
         controller.ControllerContext,
         viewResult.View,
         new ViewDataDictionary<TModel>(controller.ViewData, model),
         controller.TempData,
         writer,
         new HtmlHelperOptions()
      );

      await viewResult.View.RenderAsync(viewContext);
      return new HtmlString(writer.GetStringBuilder().ToString());
   }

   /// <summary>
   ///    Render a partial view to string, without a model present.
   /// </summary>
   public static async Task<HtmlString> RenderView(this Controller controller, string viewNamePath)
   {
      var viewResult = LoadView(controller, viewNamePath);

      if (viewResult.View is null)
         throw new InvalidOperationException("View is not loaded");

      await using var writer = new StringWriter();

      var viewContext = new ViewContext(
         controller.ControllerContext,
         viewResult.View,
         controller.ViewData,
         controller.TempData,
         writer,
         new HtmlHelperOptions()
      );

      await viewResult.View.RenderAsync(viewContext);
      return new HtmlString(writer.GetStringBuilder().ToString());
   }

   /// <summary>
   /// Redirect back to the referer URL.
   /// </summary>
   public static IActionResult RedirectToReferer(this Controller controller)
   {
      var referer = controller.Request.Referer();

      if(referer is null)
         throw new InvalidOperationException("No referer found.");

      return new RedirectResult(referer.Value);
   }

   /// <summary>
   /// Redirect back to the referer URL, with modifications.
   /// </summary>
   public static IActionResult RedirectToReferer(this Controller controller, Func<Url, Url> urlModificationAction)
   {
      var referer = controller.Request.Referer();

      if(referer is null)
         throw new InvalidOperationException("No referer found.");

      var modified = urlModificationAction.Invoke(referer.Value);
      return new RedirectResult(modified);
   }
   
   private static ViewEngineResult LoadView(Controller controller, string viewNamePath)
   {
      var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

      if (viewEngine is null)
         throw new InvalidOperationException("Could not load view engine from request.");

      var viewResult = viewEngine.GetView(executingFilePath: null, viewPath: viewNamePath, isMainPage: false);
      if (!viewResult.Success)
         viewResult = viewEngine.FindView(controller.ControllerContext, viewNamePath, isMainPage: false);
      
      if (!viewResult.Success)
         throw new InvalidOperationException($"A view with the name '{viewNamePath}' could not be found");

      return viewResult;
   }
}