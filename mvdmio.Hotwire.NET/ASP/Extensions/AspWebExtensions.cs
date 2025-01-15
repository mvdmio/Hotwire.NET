using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

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

      services.AddSingleton<ITurboBroadcaster, InMemoryTurboBroadcaster>();
   }
   
   /// <summary>
   /// Configures the pipeline to use Turbo Streams broadcasting.
   /// </summary>
   public static void UseTurboStreamsBroadcasting(this WebApplication app)
   {
      app.UseWebSockets();
   }
}