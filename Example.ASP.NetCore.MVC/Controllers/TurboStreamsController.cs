using Microsoft.AspNetCore.Mvc;
using mvdmio.Hotwire.NET.ASP.Extensions;
using mvdmio.Hotwire.NET.ASP.MVC;
using mvdmio.Hotwire.NET.ASP.TurboActions;

namespace Example.ASP.NetCore.MVC.Controllers;

[Route("example/turbo-streams")]
public class TurboStreamsController : Controller
{
   [Route(""), HttpGet, HttpPost]
   public async Task<IActionResult> Index()
   {
      if (Request.IsTurboRequest())
      {
         var contentPartial = await this.RenderView("_Content", DateTime.Now);
         return this.TurboStream(new ReplaceTurboAction("content", contentPartial));
      }

      return View();
   }
}