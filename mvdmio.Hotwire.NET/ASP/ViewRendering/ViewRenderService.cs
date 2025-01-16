using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace mvdmio.Hotwire.NET.ASP.ViewRendering;

internal class ViewRenderService : IViewRenderService
{
   private readonly ICompositeViewEngine _viewEngine;
   private readonly ITempDataProvider _tempDataProvider;
   private readonly IServiceProvider _serviceProvider;

   public ViewRenderService(ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
   {
      _viewEngine = viewEngine;
      _tempDataProvider = tempDataProvider;
      _serviceProvider = serviceProvider;
   }

   public async Task<HtmlString> RenderAsync<TModel>(string viewName, TModel model)
   {
      var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
      var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

      await using var sw = new StringWriter();
      var viewResult = _viewEngine.FindView(actionContext, viewName, false);

      if (viewResult.View == null)
      {
         throw new ArgumentNullException($"{viewName} does not match any available view");
      }

      var viewDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
      {
         Model = model
      };

      var viewContext = new ViewContext(
         actionContext,
         viewResult.View,
         viewDictionary,
         new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
         sw,
         new HtmlHelperOptions()
      );

      await viewResult.View.RenderAsync(viewContext);
      return new HtmlString(sw.ToString());
   }
}