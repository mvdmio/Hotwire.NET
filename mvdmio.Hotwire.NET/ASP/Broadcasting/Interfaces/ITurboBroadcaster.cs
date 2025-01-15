using System.Net.WebSockets;
using System.Threading.Tasks;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

/// <summary>
/// Interface for Turbo Streams broadcasters.
/// </summary>
public interface ITurboBroadcaster
{
   internal Task AddConnection(string channel, WebSocket webSocket);
}