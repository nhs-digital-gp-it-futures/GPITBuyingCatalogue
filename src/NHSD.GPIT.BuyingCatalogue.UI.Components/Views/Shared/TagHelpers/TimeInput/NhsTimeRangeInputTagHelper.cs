using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.TimeInput
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormLabelTagHelper.TagHelperName)]
    public sealed class NhsTimeRangeInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-time-range-input";
        public const string ForFromName = "asp-for-from";
        public const string ForUntilName = "asp-for-until";

        private const string TimeInputItemClass = "nhsuk-time-input__item";
        private const string TimeInputLabelClass = "nhsuk-time-input__label";

        private readonly IHtmlGenerator htmlGenerator;

        public NhsTimeRangeInputTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(ForFromName)]
        [DataType(DataType.Text)]
        public ModelExpression ForFrom { get; set; }

        [HtmlAttributeName(ForUntilName)]
        [DataType(DataType.Text)]
        public ModelExpression ForUntil { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!typeof(DateTime?).IsAssignableFrom(ForFrom.ModelExplorer.ModelType))
                throw new ArgumentException($"{nameof(ForFrom)} is not of type DateTime from attribute {nameof(ForFrom.Name)}");

            if (!typeof(DateTime?).IsAssignableFrom(ForUntil.ModelExplorer.ModelType))
                throw new ArgumentException($"{nameof(ForUntil)} is not of type DateTime from attribute {nameof(ForUntil.Name)}");

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var from = BuildInputItem(ForFrom, "From");
            var until = BuildInputItem(ForUntil, "Until");

            var id = TagBuilder.CreateSanitizedId(TagHelperFunctions.GetModelKebabNameFromFor(For), "_");

            var attributes = new List<TagHelperAttribute>
            {
                new(TagHelperConstants.Id, id),
                new(TagHelperConstants.Class, TimeInputConstants.TimeInputClass),
            };

            attributes.ForEach(a => output.Attributes.Add(a));

            output.Content
                .AppendHtml(from)
                .AppendHtml(until);

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, ForFrom);
            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, ForUntil);
            TellParentThisIsATimeInput(context);
        }

        private static TagBuilder BuildTimeInputContainerItem()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(TimeInputItemClass);

            return builder;
        }

        private static void TellParentThisIsATimeInput(TagHelperContext context)
        {
            if (!context.Items.TryGetValue(typeof(ParentChildContext), out object pChildContext))
                return;

            ParentChildContext parentChildContext = (ParentChildContext)pChildContext;

            parentChildContext.IsTimeInput = true;
        }

        private TagBuilder BuildInputItem(ModelExpression modelExpression, string labelText)
        {
            var item = BuildTimeInputContainerItem();
            var label = BuildTimeLabel(modelExpression, labelText);
            var input = BuildTimeInput(modelExpression);

            item.InnerHtml
                .AppendHtml(label)
                .AppendHtml(input);

            return item;
        }

        private TagBuilder BuildTimeLabel(ModelExpression modelExpression, string labelText)
        {
            return htmlGenerator.GenerateLabel(
                ViewContext,
                modelExpression.ModelExplorer,
                modelExpression.Name,
                labelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {TimeInputLabelClass} {TagHelperConstants.BoldLabel}" });
        }

        private TagBuilder BuildTimeInput(ModelExpression modelExpression)
        {
            var value = (DateTime?)modelExpression.Model == DateTime.MinValue || modelExpression.Model is null
                ? string.Empty
                : ((DateTime?)modelExpression.Model).Value.ToString("HH:mm");

            var builder = htmlGenerator.GenerateTextBox(
                ViewContext,
                modelExpression.ModelExplorer,
                modelExpression.Name,
                value,
                null,
                new
                {
                    @class = $"{TagHelperConstants.NhsInput} {TimeInputConstants.TimeInputInputClass} {TimeInputConstants.TimeInputWidthClass}",
                    maxlength = TimeInputConstants.MaxCharacterLength.ToString(),
                });

            if (TagHelperFunctions.CheckIfModelStateHasAnyErrors(ViewContext, ForFrom, ForUntil))
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);

            return builder;
        }
    }
}
