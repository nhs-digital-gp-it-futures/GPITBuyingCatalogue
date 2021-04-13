using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.Framework.TagHelpers
{
    [ExcludeFromCodeCoverage]
    [HtmlTargetElement(TagHelperConstants.Div, Attributes = TagHelperName)]
    public sealed class ValidationSummaryTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-validation-summary";
        public const string TitleName = "title";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (ViewContext.ViewData.ModelState.IsValid)
            {
                output.Content.Clear();
                return;
            }

            var errorSummary = GetErrorSummaryBuilder();
            var header = GetHeaderBuilder();
            var errorList = GetErrorListBuilder();

            errorSummary.InnerHtml.AppendHtml(header);
            errorSummary.InnerHtml.AppendHtml(errorList);

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(errorSummary);
        }

        private static TagBuilder GetErrorSummaryBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsValidationSummary);
            builder.Attributes[TagHelperConstants.Role] = TagHelperConstants.RoleAlert;
            builder.Attributes[TagHelperConstants.LabelledBy] = TagHelperConstants.ErrorSummaryTitle;
            builder.Attributes[TagHelperConstants.TabIndex] = "-1";

            return builder;
        }

        private static TagBuilder GetListItemBuilder(string linkElement, string errorMessage)
        {
            var listItemBuilder = new TagBuilder(TagHelperConstants.ListItem);
            var linkBuilder = new TagBuilder(TagHelperConstants.Anchor);
            linkBuilder.Attributes.Add(TagHelperConstants.Link, $"#{linkElement}");
            linkBuilder.AddCssClass("bc-c-login-page-error-box-padding");
            linkBuilder.InnerHtml.Append(errorMessage);
            listItemBuilder.InnerHtml.AppendHtml(linkBuilder);

            return listItemBuilder;
        }

        private TagBuilder GetHeaderBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.SubHeader);
            builder.AddCssClass(TagHelperConstants.NhsValidationSummaryTitle);
            builder.Attributes[TagHelperConstants.Id] = TagHelperConstants.ErrorSummaryTitle;
            builder.InnerHtml.Append(Title);

            return builder;
        }

        private TagBuilder GetErrorListBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.UnorderedList);
            builder.AddCssClass(TagHelperConstants.NhsList);
            builder.AddCssClass(TagHelperConstants.NhsValidationSummaryList);

            var viewType = ViewContext.ViewData.Model.GetType();

            if (viewType is null)
            {
                throw new InvalidOperationException();
            }

            var propertyNames = viewType.GetProperties().Select(i => i.Name).ToList();
            var orderedStates = ViewContext.ViewData.ModelState
                .OrderBy(d => propertyNames.IndexOf(d.Key))
                .ToList();

            foreach ((var key, ModelStateEntry modelStateEntry) in orderedStates)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    var listItem = GetListItemBuilder(key, error.ErrorMessage);
                    builder.InnerHtml.AppendHtml(listItem);
                }
            }

            return builder;
        }
    }
}
