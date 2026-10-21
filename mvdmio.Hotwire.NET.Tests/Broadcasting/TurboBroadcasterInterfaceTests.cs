using System.Reflection;
using FluentAssertions;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Broadcasting;

public sealed class TurboBroadcasterInterfaceTests
{
   [Fact]
   public void ITurboBroadcaster_ShouldOnlyHavePublicMethods()
   {
      var methods = typeof(ITurboBroadcaster).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

      methods.Should().NotBeEmpty();
      methods.Should().OnlyContain(method => method.IsPublic);
   }
}
