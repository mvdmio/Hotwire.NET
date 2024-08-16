using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using mvdmio.Hotwire.NET.ASP.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Extensions;

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
   ///    Render a partial view to string.
   /// </summary>
   public static async Task<HtmlString> RenderView<TModel>(this Controller controller, string viewNamePath, TModel model)
   {
      var viewData = new ViewDataDictionary<TModel>(controller.ViewData) {
         Model = model
      };

      var viewResult = LoadView(controller, viewNamePath, viewData);

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
   ///    Render a partial view to string, without a model present.
   /// </summary>
   public static async Task<HtmlString> RenderView(this Controller controller, string viewNamePath)
   {
      var viewData = new ViewDataDictionary(controller.ViewData);
      var viewResult = LoadView(controller, viewNamePath, viewData);

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

   private static ViewEngineResult LoadView(Controller controller, string viewNamePath, ViewDataDictionary viewDataDictionary)
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