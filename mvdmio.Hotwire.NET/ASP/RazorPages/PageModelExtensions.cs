using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using mvdmio.Hotwire.NET.ASP.Extensions;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;
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
   ///    Create a <see cref="TurboStreamActionResult" /> with a <see cref="RefreshTurboAction"/>.
   /// </summary>
   public static TurboStreamActionResult TurboStreamRefresh(this PageModel pageModel)
   {
      return TurboStream(pageModel, new RefreshTurboAction());
   }

   /// <summary>
   ///    Render a partial view to string.
   /// </summary>
   public static async Task<HtmlString> RenderView<TModel>(this PageModel pageModel, string viewNamePath, [DisallowNull] TModel model)
   {
      var viewResult = LoadView(pageModel, viewNamePath);

      if (viewResult.View is null)
         throw new InvalidOperationException("View is not loaded");

      await using var writer = new StringWriter();

      // Create view context with the model.
      var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
         Model = model
      };

      var viewContext = new ViewContext(
         pageModel.PageContext,
         viewResult.View,
         viewData,
         pageModel.TempData,
         writer,
         new HtmlHelperOptions()
      );

      // Set ViewContext property value on the model, if it exists.
      var properties = model.GetType().GetProperties();
      foreach (var property in properties)
      {
         if(property.GetCustomAttribute<ViewContextAttribute>() is not null)
            property.SetValue(model, viewContext);
      }

      // Render the view.
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

      // Create view context with the model.
      var viewData = new ViewDataDictionary(pageModel.ViewData);
      var viewContext = new ViewContext(
         pageModel.PageContext,
         viewResult.View,
         viewData,
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
   public static IActionResult RedirectToReferer(this PageModel pageModel, Func<Url, Url> urlModificationAction)
   {
      var referer = pageModel.Request.Referer();

      if(referer is null)
         throw new InvalidOperationException("No referer found.");

      var modified = urlModificationAction.Invoke(referer.Value);
        return new RedirectResult(modified);
   }

   private static ViewEngineResult LoadView(PageModel pageModel, string viewNamePath)
   {
      var viewEngine = pageModel.HttpContext.RequestServices.GetRequiredService<IRazorViewEngine>();

      if (viewEngine is null)
         throw new InvalidOperationException("Could not load view engine from request.");

      List<Func<ViewEngineResult>> viewRetrievers = [
         () => viewEngine.GetView(executingFilePath: null, viewPath: viewNamePath, isMainPage: true),
         () => viewEngine.GetView(executingFilePath: null, viewPath: viewNamePath, isMainPage: false),
         () => viewEngine.GetView(executingFilePath: pageModel.PageContext.ActionDescriptor.ViewEnginePath, viewPath: viewNamePath, isMainPage: true),
         () => viewEngine.GetView(executingFilePath: pageModel.PageContext.ActionDescriptor.ViewEnginePath, viewPath: viewNamePath, isMainPage: false),
         () => viewEngine.FindView(pageModel.PageContext, viewNamePath, isMainPage: true),
         () => viewEngine.FindView(pageModel.PageContext, viewNamePath, isMainPage: false)
      ];

      var searchedLocations = new List<string>();
      foreach (var retriever in viewRetrievers)
      {
         var viewResult = retriever.Invoke();

         if (viewResult.Success)
            return viewResult;
         
         searchedLocations.AddRange(viewResult.SearchedLocations);
      }
      
      throw new InvalidOperationException($"A view with the name '{viewNamePath}' could not be found. Searched locations:\r\n{string.Join("\r\n", searchedLocations.Distinct())}");
   }
}