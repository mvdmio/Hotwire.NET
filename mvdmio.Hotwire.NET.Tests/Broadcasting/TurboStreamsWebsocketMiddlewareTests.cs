using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.Tests._Helpers;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboStreamsWebsocketMiddlewareTests
{
   [Fact]
   public async Task InvokeAsync_ShouldPassThroughWebsocketRequestsOnOtherPaths()
   {
      var broadcaster = new InMemoryTurboBroadcaster(NullLogger<InMemoryTurboBroadcaster>.Instance);
      var sut = CreateMiddleware(broadcaster, new ChannelEncryptionStub(), NullLogger<TurboStreamsWebsocketMiddleware>.Instance);
      var context = CreateWebsocketRequest("/other/ws");
      var nextWasCalled = false;

      await sut.InvokeAsync(
         context,
         _ => {
            nextWasCalled = true;
            return Task.CompletedTask;
         }
      );

      nextWasCalled.Should().BeTrue();
      context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
   }

   [Fact]
   public async Task InvokeAsync_ShouldRespondBadRequest_WhenTokenComesFromForeignKeyRing()
   {
      using var keyDirectory = new TemporaryKeyDirectory();
      using var foreignKeyDirectory = new TemporaryKeyDirectory();
      var foreignToken = new DataProtectionChannelEncryption(DataProtectionProvider.Create(foreignKeyDirectory.DirectoryInfo)).Encrypt("test");
      var broadcaster = new InMemoryTurboBroadcaster(NullLogger<InMemoryTurboBroadcaster>.Instance);
      var channelEncryption = new DataProtectionChannelEncryption(DataProtectionProvider.Create(keyDirectory.DirectoryInfo));
      var logger = new RecordingLogger<TurboStreamsWebsocketMiddleware>();
      var sut = CreateMiddleware(broadcaster, channelEncryption, logger);
      var context = CreateWebsocketRequest("/turbo/ws/" + foreignToken);

      await sut.InvokeAsync(context, _ => Task.CompletedTask);

      context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
      logger.Entries.Should().ContainSingle(entry => entry.Level == LogLevel.Warning)
         .Which.Message.Should().Be("Received websocket connection request with invalid signature");
   }

   [Fact]
   public async Task InvokeAsync_ShouldRespondBadRequest_WhenRequestHasPathBaseAndUnsignedToken()
   {
      var broadcaster = new InMemoryTurboBroadcaster(NullLogger<InMemoryTurboBroadcaster>.Instance);
      var sut = CreateMiddleware(broadcaster, new ChannelEncryptionStub(), NullLogger<TurboStreamsWebsocketMiddleware>.Instance);
      var context = CreateWebsocketRequest("/turbo/ws/not-signed");
      context.Request.PathBase = "/compliance";
      var nextWasCalled = false;

      await sut.InvokeAsync(
         context,
         _ => {
            nextWasCalled = true;
            return Task.CompletedTask;
         }
      );

      nextWasCalled.Should().BeFalse();
      context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
   }

   [Fact]
   public async Task InvokeAsync_ShouldAcceptWebsocketRequest_WhenRequestHasPathBaseAndSignedToken()
   {
      var broadcaster = new InMemoryTurboBroadcaster(NullLogger<InMemoryTurboBroadcaster>.Instance);
      var sut = CreateMiddleware(broadcaster, new ChannelEncryptionStub(), NullLogger<TurboStreamsWebsocketMiddleware>.Instance);
      var socket = new RecordingWebSocket();
      var context = CreateWebsocketRequest("/turbo/ws/signed-channel", socket);
      context.Request.PathBase = "/compliance";
      var nextWasCalled = false;

      var invocation = sut.InvokeAsync(
         context,
         _ => {
            nextWasCalled = true;
            return Task.CompletedTask;
         }
      );
      await socket.ReceiveStarted.WaitAsync(TimeSpan.FromSeconds(5));
      await broadcaster.BroadcastAsync("channel", new RefreshTurboAction("request-1"));
      socket.RequestClientClose();
      await invocation.WaitAsync(TimeSpan.FromSeconds(5));

      nextWasCalled.Should().BeFalse();
      context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
      socket.SentMessages.Should().Equal("""<turbo-stream action="refresh" request-id="request-1"></turbo-stream>""");
   }

   private static TurboStreamsWebsocketMiddleware CreateMiddleware(InMemoryTurboBroadcaster broadcaster, IChannelEncryption channelEncryption, ILogger<TurboStreamsWebsocketMiddleware> logger)
   {
      return new TurboStreamsWebsocketMiddleware(new HostApplicationLifetimeStub(), broadcaster, channelEncryption, logger);
   }

   private static DefaultHttpContext CreateWebsocketRequest(string path, WebSocket? webSocket = null)
   {
      var context = new DefaultHttpContext();
      context.Request.Path = path;
      context.Features.Set<IHttpWebSocketFeature>(new TestHttpWebSocketFeature(isWebSocketRequest: true, webSocket));

      return context;
   }

   private sealed class RecordingLogger<T> : ILogger<T>
   {
      public List<(LogLevel Level, string Message)> Entries { get; } = [];

      public IDisposable? BeginScope<TState>(TState state) where TState : notnull
      {
         return null;
      }

      public bool IsEnabled(LogLevel logLevel)
      {
         return true;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
      {
         Entries.Add((logLevel, formatter(state, exception)));
      }
   }

   private sealed class TestHttpWebSocketFeature(bool isWebSocketRequest, WebSocket? webSocket) : IHttpWebSocketFeature
   {
      public bool IsWebSocketRequest => isWebSocketRequest;

      public Task<WebSocket> AcceptAsync(WebSocketAcceptContext context)
      {
         return webSocket is null ? throw new NotSupportedException() : Task.FromResult(webSocket);
      }
   }
}
