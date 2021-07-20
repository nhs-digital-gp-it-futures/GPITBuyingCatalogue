using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.Address
{
    public sealed class NhsAddressViewComponent : ViewComponent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Making this Static causes Razor to be unable to call the Invoke Function.")]
        public HtmlString Invoke(EntityFramework.Addresses.Models.Address address)
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            return new HtmlString(ProcessAdressBreakRowSeperatedString(address));
        }

        private static string ProcessAdressBreakRowSeperatedString(EntityFramework.Addresses.Models.Address address)
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

            var breakRow = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };

            foreach (string value in values)
            {
                builder.InnerHtml.Append(value);
                builder.InnerHtml.AppendHtml(breakRow);
            }

            using var writer = new System.IO.StringWriter();

            builder.WriteTo(writer, HtmlEncoder.Default);

            return writer.ToString();
        }
    }
}
