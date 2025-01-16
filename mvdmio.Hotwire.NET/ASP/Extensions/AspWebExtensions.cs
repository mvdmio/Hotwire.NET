using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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
      var controllerAssembly = typeof(TurboStreamsWebsocketController).Assembly;
      services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(controllerAssembly));

      services.AddSingleton<IViewRenderService, ViewRenderService>();
      services.AddSingleton<ITurboBroadcaster, InMemoryTurboBroadcaster>();
      services.AddSingleton<IChannelEncryption, InMemoryChannelEncryption>();
      services.AddTransient<ITagHelperComponent, TurboBroadcastChannelTagHelper>();
   }
   
   /// <summary>
   /// Configures the pipeline to use Turbo Streams broadcasting.
   /// </summary>
   public static void UseTurboStreamsBroadcasting(this WebApplication app)
   {
      app.UseWebSockets();
   }
}