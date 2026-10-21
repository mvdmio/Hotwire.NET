using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
   /// Channel names are signed with ASP.NET Core Data Protection, using the application's Data Protection key ring.
   /// </summary>
   public static void AddTurboStreamsBroadcasting(this IServiceCollection services)
   {
      services.AddHttpContextAccessor();
      services.AddDataProtection(); // Safe to call more than once; the host's own Data Protection configuration still applies.

      services.AddScoped<TurboStreamsWebsocketMiddleware>(); // Should be scoped so that each websocket connection has its own instance. The middleware blocks until the application is shut down or the client closes the connection.
      services.AddSingleton<InMemoryTurboBroadcaster>(); // The middleware stores connections here, whatever ITurboBroadcaster an app registers afterwards.
      services.AddSingleton<ITurboBroadcaster>(sp => sp.GetRequiredService<InMemoryTurboBroadcaster>());
      services.AddSingleton<IChannelEncryption, DataProtectionChannelEncryption>(); // Plain AddSingleton so an app registration made afterwards wins.

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