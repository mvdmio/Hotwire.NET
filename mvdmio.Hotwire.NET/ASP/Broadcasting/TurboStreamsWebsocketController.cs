using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting;

/// <summary>
/// Controller for handling Turbo Streams websocket communication.
/// </summary>
public class TurboStreamsWebsocketController : Controller
{
   private readonly ITurboBroadcaster _broadcaster;

   /// <summary>
   /// Constructor.
   /// </summary>
   public TurboStreamsWebsocketController(ITurboBroadcaster broadcaster)
   {
      _broadcaster = broadcaster;
   }
   
   /// <summary>
   ///   Handles the CONNECT request for the Turbo Streams websocket.
   /// </summary>
   [Route("/turbo/ws/{channel}")]
   public async Task<IActionResult> Get(string channel)
   {
      if (!HttpContext.WebSockets.IsWebSocketRequest)
         return BadRequest();
      
      // TODO: Validate channel name and make sure it has not been tampered with.
      
      var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(); 
      await _broadcaster.AddConnection(channel, webSocket);

      return Ok();
   }
}