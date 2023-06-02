using Microsoft.AspNetCore.Mvc;

namespace Example.ASP.NetCore.MVC.Controllers;

[Route("")]
public class HomeController : Controller
{
   [Route("")]
   public IActionResult Index()
   {
      return View();
   }
}