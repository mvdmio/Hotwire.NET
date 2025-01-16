using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace mvdmio.Hotwire.NET.ASP.ViewRendering;

internal class ViewRenderService : IViewRenderService
{
   private readonly IWebHostEnvironment _environment;
   private readonly ICompositeViewEngine _viewEngine;
   private readonly ITempDataProvider _tempDataProvider;
   private readonly IServiceProvider _serviceProvider;

   public ViewRenderService(IWebHostEnvironment environment, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
   {
      _environment = environment;
      _viewEngine = viewEngine;
      _tempDataProvider = tempDataProvider;
      _serviceProvider = serviceProvider;
   }

   public async Task<HtmlString> RenderAsync<TModel>(string viewName, TModel model)
   {
      var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
      var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

      var correctedViewName = CorrectViewName(viewName);

      var viewResult = _viewEngine.GetView(_environment.ContentRootPath, correctedViewName, false);
      if (viewResult.View == null)
      {
         throw new ArgumentNullException($"{viewName} does not match any available view");
      }

      var viewDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
         Model = model
      };

      await using var sw = new StringWriter();
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

   private string CorrectViewName(string viewName)
   {      
      var correctedViewName = viewName;

      if (!correctedViewName.StartsWith("~") && !correctedViewName.StartsWith("/"))
      {
         var viewDirectory = IsUsingRazorPages() ? "Pages/" : IsUsingMvc() ? "Views/" : "";
         correctedViewName = $"~/{viewDirectory}{correctedViewName}";
      }
      
      if (!viewName.EndsWith(".cshtml"))
         correctedViewName = $"{correctedViewName}.cshtml";

      return correctedViewName;
   }

   public bool IsUsingRazorPages()
   {
      return _serviceProvider.GetService<RazorPagesOptions>() is not null;
   }

   public bool IsUsingMvc()
   {
      return _serviceProvider.GetService<MvcOptions>() is not null;
   }
}