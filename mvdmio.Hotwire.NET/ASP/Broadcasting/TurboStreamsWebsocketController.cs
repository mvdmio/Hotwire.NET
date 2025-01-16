using System;
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
   private readonly IChannelEncryption _channelEncryption;

   /// <summary>
   /// Constructor.
   /// </summary>
   public TurboStreamsWebsocketController(ITurboBroadcaster broadcaster, IChannelEncryption channelEncryption)
   {
      _broadcaster = broadcaster;
      _channelEncryption = channelEncryption;
   }

   /// <summary>
   ///   Handles the CONNECT request for the Turbo Streams websocket.
   /// </summary>
   [Route("/turbo/ws/{signedChannelName}")]
   public async Task<IActionResult> Get(string signedChannelName)
   {
      if (!HttpContext.WebSockets.IsWebSocketRequest)
         return BadRequest();

      try
      {
         var channelName = _channelEncryption.Decrypt(signedChannelName);

         var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
         await _broadcaster.AddConnection(channelName, webSocket);

         return Ok();
      }
      catch (ArgumentException) // Thrown by the channel encryption when signature is not valid.
      {
         return BadRequest();
      }
   }
}