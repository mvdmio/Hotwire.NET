using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using mvdmio.Hotwire.NET.ASP.RazorPages;
using mvdmio.Hotwire.NET.Utilities;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

public class PageModelStub : ExtendedPageModel
{
   public PageModelStub()
   {
      PageContext = new PageContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()));
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