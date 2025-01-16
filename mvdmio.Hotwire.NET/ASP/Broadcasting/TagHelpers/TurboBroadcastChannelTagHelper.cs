using Microsoft.AspNetCore.Razor.TagHelpers;
using mvdmio.Hotwire.NET.ASP.Broadcasting.Interfaces;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.TagHelpers;

/// <summary>
///    Tag helper for creating Turbo Broadcasting channels.
/// </summary>
[HtmlTargetElement("turbo-broadcast-channel", TagStructure = TagStructure.NormalOrSelfClosing)]
public class TurboBroadcastChannelTagHelper : TagHelper
{
   private readonly IChannelEncryption _channelEncryption;

   /// <summary>
   ///    The name of the channel to listen to.
   /// </summary>
   [HtmlAttributeName("name")]
   public required string Name { get; set; }

   /// <summary>
   ///   Constructor.
   /// </summary>
   public TurboBroadcastChannelTagHelper(IChannelEncryption channelEncryption)
   {
      _channelEncryption = channelEncryption;
   }
   
   /// <inheritdoc />
   public override void Process(TagHelperContext context, TagHelperOutput output)
   {
      var signedChannelName = _channelEncryption.Encrypt(Name);
      
      output.TagName = null;
      output.TagMode = TagMode.StartTagAndEndTag;
      output.Content.SetHtmlContent($"<turbo-stream-source src=\"/turbo/ws/{signedChannelName}\"></turbo-stream-source>");
   }
}