using System.Net.WebSockets;
using System.Text;

namespace mvdmio.Hotwire.NET.Tests._Stubs;

/// <summary>
///   A <see cref="WebSocket" /> that stays open and keeps the text messages sent to it. Receiving waits until <see cref="RequestClientClose" />.
/// </summary>
public sealed class RecordingWebSocket : WebSocket
{
   private readonly TaskCompletionSource _receiveStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
   private readonly TaskCompletionSource _clientClose = new(TaskCreationOptions.RunContinuationsAsynchronously);

   /// <summary>
   ///   Completes when the first receive starts.
   /// </summary>
   public Task ReceiveStarted => _receiveStarted.Task;

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

   /// <summary>
   ///   Makes the pending receive return a close message, as when the client closes the connection.
   /// </summary>
   public void RequestClientClose()
   {
      _clientClose.TrySetResult();
   }

   public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
   {
      _receiveStarted.TrySetResult();
      await _clientClose.Task.WaitAsync(cancellationToken);

      return new WebSocketReceiveResult(0, WebSocketMessageType.Close, true);
   }

   public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
   {
      SentMessages.Add(Encoding.UTF8.GetString(buffer.AsSpan()));
      return Task.CompletedTask;
   }
}
