using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.FieldSet
{
    public static class FieldSetTagHelperBuilders
    {
        public const string FieldSetSizeName = "size";

        public const string NhsFieldset = "nhsuk-fieldset";
        public const string NhsFieldsetLegend = "nhsuk-fieldset__legend";
        public const string NhsFieldsetLegendExtraLarge = "nhsuk-label--xl";
        public const string NhsFieldsetLegendLarge = "nhsuk-label--l";
        public const string NhsFieldsetLegendMedium = "nhsuk-label--m";
        public const string NhsFieldsetLegendSmall = "nhsuk-label--s";
        public const string NhsFieldSetLegendHeading = "nhsuk-fieldset__heading";

        public enum FieldSetSize
        {
            ExtraLarge = 1,
            Large = 0,
            Medium = 2,
            Small = 3,
        }

        public static TagBuilder GetFieldsetBuilder(string formName)
        {
            var builder = new TagBuilder(TagHelperConstants.FieldSet);

            builder.AddCssClass(NhsFieldset);
            builder.MergeAttribute(TagHelperConstants.AriaDescribedBy, $"{formName}-hint");

            return builder;
        }

        public static TagBuilder GetFieldsetLegendBuilder(FieldSetSize selectedSize)
        {
            var builder = new TagBuilder(TagHelperConstants.Legend);

            var selectedLedgendClass = selectedSize switch
            {
                FieldSetSize.ExtraLarge => NhsFieldsetLegendExtraLarge,
                FieldSetSize.Medium => NhsFieldsetLegendMedium,
                FieldSetSize.Small => NhsFieldsetLegendSmall,
                FieldSetSize.Large or _ => NhsFieldsetLegendLarge,
            };

            builder.AddCssClass(NhsFieldsetLegend);
            builder.AddCssClass(selectedLedgendClass);

            return builder;
        }

        public static TagBuilder GetFieldSetLegendHeadingBuilder(FieldSetSize selectedSize, string labelText)
        {
            var fieldsetLegend = GetFieldsetLegendBuilder(selectedSize);
            var fieldsetlegendheader = GetFieldsetLegendHeadingTagBuilder(labelText, selectedSize);

            fieldsetLegend.InnerHtml.AppendHtml(fieldsetlegendheader);

            return fieldsetLegend;
        }

        public static TagBuilder GetFieldsetLegendHeadingTagBuilder(string labelText, FieldSetSize selectedSize)
        {
            if (labelText == null)
                return null;

            var selectedLedgendTag = selectedSize switch
            {
                FieldSetSize.ExtraLarge => TagHelperConstants.HeaderOne,
                FieldSetSize.Medium => TagHelperConstants.HeaderThree,
                FieldSetSize.Small => "label",
                _ or FieldSetSize.Large => TagHelperConstants.HeaderTwo,
            };

            var builder = new TagBuilder(selectedLedgendTag);
            builder.AddCssClass(NhsFieldSetLegendHeading);

            builder.InnerHtml.Append(labelText);

            return builder;
        }
    }
}
