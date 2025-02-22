using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using mvdmio.Hotwire.NET.ASP.MVC;
using mvdmio.Hotwire.NET.Utilities;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

public class ControllerStub : ExtendedController
{
   public ControllerStub()
   {
      ControllerContext = new ControllerContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor()));
   }

   public IActionResult TestRedirectToReferer()
   {
      return RedirectToReferer();
   }

   public IActionResult TestRedirectToReferer(Func<Url, Url> urlModificationAction)
   {
      return RedirectToReferer(urlModificationAction);
   }
}