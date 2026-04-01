using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Broadcasting.ValueObjects;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboStreamsWebsocketMiddlewareTests
{
   [Fact]
   public async Task InvokeAsync_ShouldPassThroughWebsocketRequestsOnOtherPaths()
   {
      var broadcaster = new TestTurboBroadcaster();
      var channelEncryption = new TestChannelEncryption();
      var applicationLifetime = new TestHostApplicationLifetime();
      var sut = new TurboStreamsWebsocketMiddleware(applicationLifetime, broadcaster, channelEncryption, NullLogger<TurboStreamsWebsocketMiddleware>.Instance);
      var context = new DefaultHttpContext();
      context.Request.Path = "/other/ws";
      context.Features.Set<IHttpWebSocketFeature>(new TestHttpWebSocketFeature(isWebSocketRequest: true));
      var nextWasCalled = false;

      await sut.InvokeAsync(
         context,
         _ => {
            nextWasCalled = true;
            return Task.CompletedTask;
         }
      );

      nextWasCalled.Should().BeTrue();
      broadcaster.AddConnectionWasCalled.Should().BeFalse();
      context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
   }

   private sealed class TestTurboBroadcaster : ITurboBroadcaster
   {
      public bool AddConnectionWasCalled { get; private set; }

      public Task<ConnectionId> AddConnection(string channel, WebSocket webSocket, CancellationToken ct = default)
      {
         AddConnectionWasCalled = true;
         return Task.FromResult(new ConnectionId());
      }

      public Task RemoveConnection(ConnectionId connectionId, CancellationToken ct = default)
      {
         return Task.CompletedTask;
      }

      public Task BroadcastAsync(string channel, ITurboAction turboAction, CancellationToken ct = default)
      {
         return Task.CompletedTask;
      }
   }

   private sealed class TestChannelEncryption : IChannelEncryption
   {
      public string Encrypt(string channelName)
      {
         return channelName;
      }

      public string Decrypt(string signedChannelName)
      {
         return signedChannelName;
      }
   }

   private sealed class TestHostApplicationLifetime : IHostApplicationLifetime
   {
      public CancellationToken ApplicationStarted => CancellationToken.None;

      public CancellationToken ApplicationStopping => CancellationToken.None;

      public CancellationToken ApplicationStopped => CancellationToken.None;

      public void StopApplication()
      {
      }
   }

   private sealed class TestHttpWebSocketFeature(bool isWebSocketRequest) : IHttpWebSocketFeature
   {
      public bool IsWebSocketRequest => isWebSocketRequest;

      public Task<WebSocket> AcceptAsync(WebSocketAcceptContext context)
      {
         throw new NotSupportedException();
      }
   }
}
