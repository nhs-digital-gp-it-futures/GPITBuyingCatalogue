using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.FieldSet
{
    public static class FieldSetTagHelperBuilders
    {
        public const string FieldSetSizeName = "size";

        public const string NhsFieldset = "nhsuk-fieldset";
        public const string NhsFieldsetLegend = "nhsuk-fieldset__legend";
        public const string NhsFieldsetLegendExtraLarge = "nhsuk-fieldset__legend--xl";
        public const string NhsFieldsetLegendLarge = "nhsuk-fieldset__legend--l";
        public const string NhsFieldsetLegendMedium = "nhsuk-fieldset__legend--m";
        public const string NhsFieldsetLegendSmall = "nhsuk-fieldset__legend--s";
        public const string NhsFieldsetLegendExtraSmall = "nhsuk-fieldset__legend--xs";
        public const string NhsFieldSetLegendHeading = "nhsuk-fieldset__heading";

        public enum FieldSetSize
        {
            ExtraLarge = 1,
            Large = 0,
            Medium = 2,
            Small = 3,
            ExtraSmall = 4,
        }

        public static TagBuilder GetFieldsetBuilder(string formName, string labelHint)
        {
            var builder = new TagBuilder(TagHelperConstants.FieldSet);

            builder.GenerateId(formName, "_");

            builder.AddCssClass(NhsFieldset);

            if (!string.IsNullOrWhiteSpace(labelHint))
                builder.MergeAttribute(TagHelperConstants.AriaDescribedBy, TagBuilder.CreateSanitizedId($"{formName}-hint", "_"));

            return builder;
        }

        public static TagBuilder GetFieldsetLegendBuilder(FieldSetSize selectedSize)
        {
            var builder = new TagBuilder(TagHelperConstants.Legend);

            var selectedLegendClass = selectedSize switch
            {
                FieldSetSize.ExtraLarge => NhsFieldsetLegendExtraLarge,
                FieldSetSize.Medium => NhsFieldsetLegendMedium,
                FieldSetSize.Small => NhsFieldsetLegendSmall,
                FieldSetSize.ExtraSmall => NhsFieldsetLegendExtraSmall,
                FieldSetSize.Large or _ => NhsFieldsetLegendLarge,
            };

            builder.AddCssClass(NhsFieldsetLegend);
            builder.AddCssClass(selectedLegendClass);
            builder.AddCssClass(TagHelperConstants.BoldLabel);

            return builder;
        }

        public static TagBuilder GetFieldSetLegendHeadingBuilder(FieldSetSize selectedSize, string labelText)
        {
            if (labelText is null)
                return null;

            var fieldsetLegend = GetFieldsetLegendBuilder(selectedSize);

            switch (selectedSize)
            {
                case FieldSetSize.ExtraLarge:
                case FieldSetSize.Large:
                case FieldSetSize.Medium:
                    var selectedLegendTag = selectedSize switch
                    {
                        FieldSetSize.ExtraLarge => TagHelperConstants.HeaderOne,
                        FieldSetSize.Medium => TagHelperConstants.HeaderThree,
                        _ or FieldSetSize.Large => TagHelperConstants.HeaderTwo,
                    };
                    var fieldsetLegendHeader = GetFieldsetLegendHeadingTagBuilder(labelText, selectedLegendTag);
                    fieldsetLegend.InnerHtml.AppendHtml(fieldsetLegendHeader);
                    break;
                default:
                    fieldsetLegend.InnerHtml.Append(labelText);
                    break;
            }

            return fieldsetLegend;
        }

        public static TagBuilder GetFieldsetLegendHeadingTagBuilder(string labelText, string selectedLegendTag)
        {
            if (labelText == null)
                return null;

            var builder = new TagBuilder(selectedLegendTag);

            builder.AddCssClass(NhsFieldSetLegendHeading);
            builder.InnerHtml.Append(labelText);

            return builder;
        }
    }
}
