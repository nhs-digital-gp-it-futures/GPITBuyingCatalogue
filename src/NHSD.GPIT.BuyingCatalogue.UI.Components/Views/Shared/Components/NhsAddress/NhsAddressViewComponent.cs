using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.Address
{
    public sealed class NhsAddressViewComponent : ViewComponent
    {
        public HtmlString Invoke(EntityFramework.Models.Ordering.Address address)
        {
            return new HtmlString(ProcessAdressBreakRowSeperatedString(address));
        }

        private static string ProcessAdressBreakRowSeperatedString(EntityFramework.Models.Ordering.Address address)
        {
            var addressProperties = address.GetType().GetProperties().Where(a => a.PropertyType == typeof(string));

            List<string> values = new List<string>();

            foreach (var property in addressProperties)
            {
                var propertyValue = property.GetValue(address)?.ToString();

                if (!string.IsNullOrWhiteSpace(propertyValue))
                    values.Add(propertyValue);
            }

            var builder = new TagBuilder("p");

            foreach (string value in values)
            {
                builder.InnerHtml.Append(value);
                builder.InnerHtml.AppendHtmlLine("<br/>");
            }

            using var writer = new System.IO.StringWriter();

            builder.WriteTo(writer, HtmlEncoder.Default);

            return writer.ToString();
        }
    }
}
