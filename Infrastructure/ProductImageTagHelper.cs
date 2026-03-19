using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TechnoShop.Infrastructure
{
  [HtmlTargetElement("img", Attributes = "product-image")]
  public class ProductImageTagHelper : TagHelper
  {
    public string? ProductImage { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (string.IsNullOrEmpty(ProductImage))
      {
        output.Attributes.SetAttribute("src", "/images/no-image.jpg");
      }
      else
      {
        output.Attributes.SetAttribute("src", ProductImage);
      }
      
      output.Attributes.RemoveAll("product-image");
      output.AddClass("img-fluid");
    }
  }
}
