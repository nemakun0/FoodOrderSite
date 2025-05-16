using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FoodOrderSite.TagHelpers
{
    [HtmlTargetElement("restaurant-type-dropdown")]
    public class RestaurantTypeDropdownTagHelper : TagHelper
    {
        private readonly ApplicationDbContext _context;

        public RestaurantTypeDropdownTagHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Tag ayarları
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("name", AspFor.Name);
            output.Attributes.SetAttribute("class", "form-control");
            output.Attributes.SetAttribute("id", AspFor.Name);

            // Veritabanından türleri al
            var types = await _context.RestaurantTypes.ToListAsync();

            if (types == null || !types.Any())
            {
                output.Content.SetHtmlContent("<option value=''>Tür bulunamadı</option>");
                return;
            }

            var selectedValue = AspFor.Model?.ToString() ?? "";
            var optionsHtml = "<option value=''>Seçiniz</option>";

            foreach (var type in types)
            {
                var selected = type.Name == selectedValue ? "selected" : "";
                optionsHtml += $"<option value='{type.Name}' {selected}>{type.Name}</option>";
            }

            output.Content.SetHtmlContent(optionsHtml);
        }
    }
}
