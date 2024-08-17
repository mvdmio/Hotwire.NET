using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using mvdmio.Hotwire.NET.ASP.Extensions;
using mvdmio.Hotwire.NET.ASP.TurboActions;

namespace Example.ASP.NetCore.RazorPages.Pages.TurboFrames;

public class Index : PageModel
{
   public IActionResult OnGet()
   {
      return Page();
   }

   public async Task<IActionResult> OnPost()
   {
      if (Request.IsTurboRequest())
      {
         var renderedPartial = await this.RenderView("_Content", DateTime.Now);
         return this.TurboStream(new ReplaceTurboAction("content", renderedPartial));
      }

      return Page();
   }
}