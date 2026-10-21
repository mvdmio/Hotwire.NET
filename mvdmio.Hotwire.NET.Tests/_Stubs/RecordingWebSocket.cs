using System.Net.WebSockets;
using System.Text;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

/// <summary>
///   A <see cref="WebSocket" /> that stays open and keeps the text messages sent to it.
/// </summary>
public sealed class RecordingWebSocket : WebSocket
{
   public List<string> SentMessages { get; } = [];

   public override WebSocketCloseStatus? CloseStatus => null;

   public override string? CloseStatusDescription => null;

   public override WebSocketState State => WebSocketState.Open;

   public override string? SubProtocol => null;

   public override void Abort()
   {
   }

   public override Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
   {
      return Task.CompletedTask;
   }

   public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
   {
      return Task.CompletedTask;
   }

   public override void Dispose()
   {
   }

   public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
   {
      throw new NotSupportedException();
   }

   public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
   {
      SentMessages.Add(Encoding.UTF8.GetString(buffer.AsSpan()));
      return Task.CompletedTask;
   }
}
