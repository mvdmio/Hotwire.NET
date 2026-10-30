using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

/// <summary>
///   <see cref="IHtmlHelper" /> that only exposes a <see cref="ViewContext" />; any other member throws.
/// </summary>
public class HtmlHelperStub : DispatchProxy
{
   private ViewContext? _viewContext;

   public static IHtmlHelper Create(ViewContext viewContext)
   {
      var helper = Create<IHtmlHelper, HtmlHelperStub>();
      ((HtmlHelperStub)(object)helper)._viewContext = viewContext;

      return helper;
   }

   protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
   {
      if (targetMethod?.Name == "get_" + nameof(IHtmlHelper.ViewContext))
         return _viewContext;

      throw new NotSupportedException($"{targetMethod?.Name} is not supported by {nameof(HtmlHelperStub)}");
   }
}
