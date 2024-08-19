using FluentAssertions;
using mvdmio.Hotwire.NET.Utilities;
using Xunit;

namespace mvdmio.Hotwire.NET.Tests.Utilities;

public class UrlTests
{
   [Fact]
   public void ToString_ShouldReturnUrl()
   {
      var url = new Url("https://jewelsoftware.com");
      
      url.ToString().Should().Be("https://jewelsoftware.com/");
   }

   [Fact]
   public void WithoutQuery_ShouldCreateNewUrlWithoutQueryParam()
   {
      var url = new Url("https://jewelsoftware.com?q1=value1&q2=value2");
      var urlWithoutQuery = url.WithoutQueryParam("q1");

      urlWithoutQuery.Should().NotBeSameAs(url);
      urlWithoutQuery.ToString().Should().Be("https://jewelsoftware.com/?q2=value2");
   }
   
   [Fact]
   public void WithQuery_ShouldCreateNewUrlWithNewQueryParam()
   {
      var url = new Url("https://jewelsoftware.com?q1=value1&q2=value2");
      var urlWithoutQuery = url.WithQueryParam("q3", "value3");

      urlWithoutQuery.Should().NotBeSameAs(url);
      urlWithoutQuery.ToString().Should().Be("https://jewelsoftware.com/?q1=value1&q2=value2&q3=value3");
   }
   
   [Fact]
   public void WithQuery_ShouldCreateNewUrlWithOverriddenQueryParam()
   {
      var url = new Url("https://jewelsoftware.com?q1=value1&q2=value2");
      var urlWithoutQuery = url.WithQueryParam("q1", "valueNew");

      urlWithoutQuery.Should().NotBeSameAs(url);
      urlWithoutQuery.ToString().Should().Be("https://jewelsoftware.com/?q1=valueNew&q2=value2");
   }
}