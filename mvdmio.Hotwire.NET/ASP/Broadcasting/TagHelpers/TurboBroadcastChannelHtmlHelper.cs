using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.TagHelpers;

/// <summary>
///   HTML Helper for creating Turbo Broadcasting channels.
/// </summary>
public static class TurboBroadcastChannelHtmlHelper
{
   /// <summary>
   ///   Adds a Turbo Broadcasting channel to the HTML.
   /// </summary>
   public static string TurboBroadcastChannel(this IHtmlHelper htmlHelper, string name)
   {
      var channelEncryption = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IChannelEncryption>();
      var signedChannelName = channelEncryption.Encrypt(name);

      var host = htmlHelper.ViewContext.HttpContext.Request.Host;

      return $"<turbo-stream-source src=\"wss://{host}/turbo/ws/{signedChannelName}\"></turbo-stream-source>";
   }
}