using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using mvdmio.Hotwire.NET.ASP.Broadcasting;
using mvdmio.Hotwire.NET.ASP.TurboActions;
using mvdmio.Hotwire.NET.Tests._Stubs;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboBroadcasterDeliveryTests
{
   [Fact]
   public async Task BroadcastAsync_ShouldDeliverToConnectionOfWrappedBroadcaster_WhenSentThroughDecorator()
   {
      var transport = new InMemoryTurboBroadcaster(NullLogger<InMemoryTurboBroadcaster>.Instance);
      var socket = new RecordingWebSocket();
      var otherChannelSocket = new RecordingWebSocket();
      await transport.AddConnection("channel", socket);
      await transport.AddConnection("other-channel", otherChannelSocket);
      var action = new RefreshTurboAction("request-1");
      var sut = new RecordingTurboBroadcaster(transport);

      await sut.BroadcastAsync("channel", action);

      sut.Broadcasts.Should().ContainSingle().Which.Should().Be(("channel", action));
      socket.SentMessages.Should().Equal("""<turbo-stream action="refresh" request-id="request-1"></turbo-stream>""");
      otherChannelSocket.SentMessages.Should().BeEmpty();
   }
}
