using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TechnoShop.Infrastructure
{
  [HtmlTargetElement("span", Attributes = "price")]
  public class PriceTagHelper : TagHelper
  {
    public decimal Price { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      output.Content.SetContent(Price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("vi-VN")));
      var classAttr = output.Attributes["class"]?.Value?.ToString();
      output.Attributes.SetAttribute("class", string.IsNullOrEmpty(classAttr) ? "price-display" : classAttr + " price-display");
    }
  }
}
