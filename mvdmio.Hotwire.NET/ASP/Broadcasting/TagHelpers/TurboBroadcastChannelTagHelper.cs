using Microsoft.AspNetCore.Razor.TagHelpers;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.TagHelpers;

/// <summary>
///    Tag helper for creating Turbo Broadcasting channels.
/// </summary>
[HtmlTargetElement("turbo-broadcast-channel", TagStructure = TagStructure.NormalOrSelfClosing)]
public class TurboBroadcastChannelTagHelper : TagHelper
{
   /// <summary>
   ///    The name of the channel to listen to.
   /// </summary>
   [HtmlAttributeName("name")]
   public required string Name { get; set; }

   /// <inheritdoc />
   public override void Process(TagHelperContext context, TagHelperOutput output)
   {
      var signedChannelName = Name; // TODO: Sign the channel name.
      
      output.TagName = null;
      output.TagMode = TagMode.StartTagAndEndTag;
      output.Content.SetHtmlContent($"<turbo-stream-source src=\"/turbo/ws/{signedChannelName}\"></turbo-stream-source>");
   }
}