using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using mvdmio.Hotwire.NET.ASP.Broadcasting.ValueObjects;
using mvdmio.Hotwire.NET.ASP.TurboActions.Interfaces;
using mvdmio.Hotwire.NET.Tests._Helpers;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboStreamsWebsocketMiddlewareTests
{
   [Fact]
   public async Task InvokeAsync_ShouldPassThroughWebsocketRequestsOnOtherPaths()
   {
      var broadcaster = new TestTurboBroadcaster();
      var sut = CreateMiddleware(broadcaster, new TestChannelEncryption(), NullLogger<TurboStreamsWebsocketMiddleware>.Instance);
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
      broadcaster.AddConnectionWasCalled.Should().BeFalse();
      context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
   }

   [Fact]
   public async Task InvokeAsync_ShouldRespondBadRequest_WhenTokenComesFromForeignKeyRing()
   {
      using var keyDirectory = new TemporaryKeyDirectory();
      using var foreignKeyDirectory = new TemporaryKeyDirectory();
      var foreignToken = new DataProtectionChannelEncryption(DataProtectionProvider.Create(foreignKeyDirectory.DirectoryInfo)).Encrypt("test");
      var broadcaster = new TestTurboBroadcaster();
      var channelEncryption = new DataProtectionChannelEncryption(DataProtectionProvider.Create(keyDirectory.DirectoryInfo));
      var logger = new RecordingLogger<TurboStreamsWebsocketMiddleware>();
      var sut = CreateMiddleware(broadcaster, channelEncryption, logger);
      var context = CreateWebsocketRequest("/turbo/ws/" + foreignToken);

      await sut.InvokeAsync(context, _ => Task.CompletedTask);

      context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
      broadcaster.AddConnectionWasCalled.Should().BeFalse();
      logger.Entries.Should().ContainSingle(entry => entry.Level == LogLevel.Warning)
         .Which.Message.Should().Be("Received websocket connection request with invalid signature");
   }

   private static TurboStreamsWebsocketMiddleware CreateMiddleware(ITurboBroadcaster broadcaster, IChannelEncryption channelEncryption, ILogger<TurboStreamsWebsocketMiddleware> logger)
   {
      return new TurboStreamsWebsocketMiddleware(new TestHostApplicationLifetime(), broadcaster, channelEncryption, logger);
   }

   private static DefaultHttpContext CreateWebsocketRequest(string path)
   {
      var context = new DefaultHttpContext();
      context.Request.Path = path;
      context.Features.Set<IHttpWebSocketFeature>(new TestHttpWebSocketFeature(isWebSocketRequest: true));

      return context;
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

   private sealed class TestHttpWebSocketFeature(bool isWebSocketRequest) : IHttpWebSocketFeature
   {
      public bool IsWebSocketRequest => isWebSocketRequest;

      public Task<WebSocket> AcceptAsync(WebSocketAcceptContext context)
      {
         throw new NotSupportedException();
      }
   }
}
