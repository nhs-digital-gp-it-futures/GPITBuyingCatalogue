using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class ValidationSummaryTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-validation-summary";
        public const string TitleName = "title";

        private const string NhsValidationSummaryTitle = "nhsuk-error-summary__title";
        private const string ErrorSummaryTitle = "error-summary-title";
        private const string NhsValidationSummary = "nhsuk-error-summary";
        private const string NhsValidationSummaryList = "nhsuk-error-summary__list";

        private const string RadioIdName = "RadioId";
        private const string CheckboxIdName = "CheckboxId";

        private const string DefaultTitle = "There is a problem";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(RadioIdName)]
        public string RadioId { get; set; }

        [HtmlAttributeName(CheckboxIdName)]
        public string CheckboxId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext.ViewData.ModelState.IsValid)
            {
                output.SuppressOutput();
                return;
            }

            var header = GetHeaderBuilder();
            var errorList = GetErrorListBuilder();

            BuildOutput(output);

            output.Content.AppendHtml(header);
            output.Content.AppendHtml(errorList);
        }

        private static void BuildOutput(TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var attributes = new List<TagHelperAttribute>
            {
                new TagHelperAttribute(TagHelperConstants.Role, TagHelperConstants.RoleAlert),
                new TagHelperAttribute(TagHelperConstants.LabelledBy, ErrorSummaryTitle),
                new TagHelperAttribute(TagHelperConstants.TabIndex, "-1"),
                new TagHelperAttribute(TagHelperConstants.Class, NhsValidationSummary),
            };

            attributes.ForEach(a => output.Attributes.Add(a));
        }

        private TagBuilder GetListItemBuilder(string linkElement, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(RadioId)
                && RadioId.Split(',', StringSplitOptions.TrimEntries).Contains(linkElement))
            {
                linkElement = $"{linkElement}_0";
            }
            else if (!string.IsNullOrWhiteSpace(CheckboxId)
                && CheckboxId.Split(',', StringSplitOptions.TrimEntries).Contains(linkElement))
            {
                linkElement = $"{linkElement}_0__Selected";
            }

            var sanitizedLinkElement = TagBuilder.CreateSanitizedId(linkElement, "_");

            var listItemBuilder = new TagBuilder(TagHelperConstants.ListItem);

            var linkBuilder = new TagBuilder(TagHelperConstants.Anchor);
            linkBuilder.Attributes.Add(TagHelperConstants.Link, $"#{sanitizedLinkElement}");
            linkBuilder.InnerHtml.Append(errorMessage);
            listItemBuilder.InnerHtml.AppendHtml(linkBuilder);

            return listItemBuilder;
        }

        private TagBuilder GetHeaderBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.HeaderTwo);
            builder.AddCssClass(NhsValidationSummaryTitle);
            builder.GenerateId(ErrorSummaryTitle, "_");

            builder.InnerHtml.Append(Title ?? DefaultTitle);

            return builder;
        }

        private TagBuilder GetErrorListBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.UnorderedList);
            builder.AddCssClass(TagHelperConstants.NhsList);
            builder.AddCssClass(NhsValidationSummaryList);

            var viewType = ViewContext.ViewData.Model.GetType();

            if (viewType is null)
                throw new InvalidOperationException();

            var propertyNames = viewType.GetProperties().Select(i => i.Name).ToList();
            var orderedStates = ViewContext.ViewData.ModelState
                .OrderBy(d =>
                {
                    var key = d.Key.Contains('[')
                        ? d.Key[..d.Key.IndexOf('[')]
                        : d.Key;

                    return propertyNames.IndexOf(key);
                });

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
