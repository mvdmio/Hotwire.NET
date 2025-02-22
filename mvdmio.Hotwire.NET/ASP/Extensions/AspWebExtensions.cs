using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Broadcasting.TagHelpers;
using mvdmio.Hotwire.NET.ASP.ViewRendering;

namespace mvdmio.Hotwire.NET.ASP.Extensions;

/// <summary>
/// Extensions for the ASP Web Applications.
/// </summary>
public static class AspWebExtensions
{
   /// <summary>
   /// Adds the Turbo Streams broadcasting services to the application.
   /// </summary>
   public static void AddTurboStreamsBroadcasting(this IServiceCollection services)
   {
      services.AddHttpContextAccessor();

      services.AddSingleton<TurboStreamsWebsocketMiddleware>();
      services.AddSingleton<ITurboBroadcaster, InMemoryTurboBroadcaster>();
      services.AddSingleton<IChannelEncryption, InMemoryChannelEncryption>();

      services.AddScoped<IViewRenderService, ViewRenderService>();

      services.AddTransient<ITagHelperComponent, TurboBroadcastChannelTagHelper>();
   }
   
   /// <summary>
   /// Configures the pipeline to use Turbo Streams broadcasting.
   /// </summary>
   public static void UseTurboStreamsBroadcasting(this WebApplication app)
   {
      app.UseWebSockets();
      app.UseMiddleware<TurboStreamsWebsocketMiddleware>();
   }
}