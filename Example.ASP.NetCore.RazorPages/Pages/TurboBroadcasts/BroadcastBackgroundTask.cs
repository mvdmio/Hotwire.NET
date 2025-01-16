using System.Diagnostics;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.ASP.ViewRendering;

namespace Example.ASP.NetCore.RazorPages.Pages.TurboBroadcasts;

public class BroadcastBackgroundTask : BackgroundService
{
   private readonly IServiceProvider _serviceProvider;

   public BroadcastBackgroundTask(IServiceProvider serviceProvider)
   {
      _serviceProvider = serviceProvider;
   }
   
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      using var scope = _serviceProvider.CreateScope();
      var broadcaster = scope.ServiceProvider.GetRequiredService<ITurboBroadcaster>();
      var viewRenderService = scope.ServiceProvider.GetRequiredService<IViewRenderService>();
      
      while(stoppingToken.IsCancellationRequested == false)
      {
         var startTime = Stopwatch.GetTimestamp();
         
         var view = await viewRenderService.RenderAsync("_Content", DateTime.Now);
         var replaceTurboAction = new ReplaceTurboAction("content", view);

         await broadcaster.BroadcastAsync("broadcasts-example", replaceTurboAction, stoppingToken);

         var elapsedTime = Stopwatch.GetElapsedTime(startTime);
         if (elapsedTime > TimeSpan.FromSeconds(1))
            elapsedTime = elapsedTime.Subtract(TimeSpan.FromSeconds(elapsedTime.TotalSeconds - 1));
         
         var delay = TimeSpan.FromSeconds(1).Subtract(elapsedTime);
         await Task.Delay(delay, stoppingToken);
      }
   }
}