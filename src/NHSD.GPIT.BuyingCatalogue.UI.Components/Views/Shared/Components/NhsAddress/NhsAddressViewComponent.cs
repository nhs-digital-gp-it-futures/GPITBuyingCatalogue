using System;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsAddress
{
    public sealed class NhsAddressViewComponent : ViewComponent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Making this Static causes Razor to be unable to call the Invoke Function.")]
        public HtmlString Invoke(EntityFramework.Addresses.Models.Address address)
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            return new HtmlString(ProcessAddressBreakRowSeperatedString(address));
        }

        private static string ProcessAddressBreakRowSeperatedString(EntityFramework.Addresses.Models.Address address)
        {
            var addressProperties = address.GetType().GetProperties().Where(a => a.PropertyType == typeof(string));

            var values = addressProperties.Select(property => property.GetValue(address)?.ToString()).Where(propertyValue => !string.IsNullOrWhiteSpace(propertyValue)).ToList();

            var builder = new TagBuilder("span");

            var breakRow = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };

            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];
                builder.InnerHtml.Append(value!);
                if (i < (values.Count - 1))
                {
                    builder.InnerHtml.AppendHtml(breakRow);
                }
            }

            using var writer = new System.IO.StringWriter();

            builder.WriteTo(writer, HtmlEncoder.Default);

            return writer.ToString();
        }
    }
}
