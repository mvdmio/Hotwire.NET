using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.TagHelpers;

/// <summary>
///    Tag helper for creating Turbo Broadcasting channels.
/// </summary>
[HtmlTargetElement("turbo-broadcast-channel", TagStructure = TagStructure.NormalOrSelfClosing)]
public class TurboBroadcastChannelTagHelper : TagHelper
{
   private readonly IHttpContextAccessor _httpContextAccessor;
   private readonly IChannelEncryption _channelEncryption;

   /// <summary>
   ///    The name of the channel to listen to.
   /// </summary>
   [HtmlAttributeName("name")]
   public string? Name { get; set; }

   /// <summary>
   ///   Constructor.
   /// </summary>
   public TurboBroadcastChannelTagHelper(IHttpContextAccessor httpContextAccessor, IChannelEncryption channelEncryption)
   {
      _httpContextAccessor = httpContextAccessor;
      _channelEncryption = channelEncryption;
   }
   
   /// <inheritdoc />
   public override void Process(TagHelperContext context, TagHelperOutput output)
   {
      output.TagName = null;
      output.TagMode = TagMode.StartTagAndEndTag;

      if (Name == null)
         return;

      var signedChannelName = _channelEncryption.Encrypt(Name);
      
      var host = _httpContextAccessor.HttpContext?.Request.Host;
      if (host is null)
         throw new InvalidOperationException("Unable to get host from http context");

      output.Content.SetHtmlContent($"<turbo-stream-source src=\"wss://{host}/turbo/ws/{signedChannelName}\"></turbo-stream-source>");
   }
}