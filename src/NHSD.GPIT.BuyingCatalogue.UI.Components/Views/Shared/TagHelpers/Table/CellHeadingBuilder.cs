using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    public static class CellHeadingBuilder
    {
        private const string HeadingClass = "nhsuk-table-responsive__heading";

        public static TagBuilder GetHeadingBuilder(TagHelperContext context)
        {
            if (!context.Items.TryGetValue(TagHelperConstants.CellColumnContextName, out object columnNames))
                return null;

            var columnNamesConverted = (Queue<TagHelperContent>)columnNames;

            if (columnNamesConverted.Count == 0)
                return null;

            var builder = new TagBuilder(TagHelperConstants.Span);
            builder.AddCssClass(HeadingClass);

            builder.InnerHtml.AppendHtml(columnNamesConverted.Dequeue());

            return builder;
        }
    }
}
