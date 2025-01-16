using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace mvdmio.Hotwire.NET.ASP.ViewRendering;

/// <summary>
///   Service for rendering views.
/// </summary>
public interface IViewRenderService
{
   /// <summary>
   ///   Render a view with the specified model.
   /// </summary>
   Task<HtmlString> RenderAsync<TModel>(string viewName, TModel model);
}