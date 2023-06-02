using Microsoft.AspNetCore.Mvc;

namespace Example.ASP.NetCore.MVC.Controllers;

[Route("example/turbo-frames")]
public class TurboFramesController : Controller
{
   [Route("")]
   public IActionResult Index()
   {
      return View();
   }

   [Route("content")]
   public IActionResult Content()
   {
      return PartialView("_Content");
   }
}