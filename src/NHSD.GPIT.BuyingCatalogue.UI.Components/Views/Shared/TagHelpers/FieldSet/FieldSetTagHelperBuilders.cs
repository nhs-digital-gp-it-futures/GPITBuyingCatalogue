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
                FieldSetSize.Large => NhsFieldsetLegendLarge,
                FieldSetSize.Medium => NhsFieldsetLegendMedium,
                FieldSetSize.Small => NhsFieldsetLegendSmall,
                _ => throw new System.InvalidCastException(),
            };

            builder.AddCssClass(NhsFieldsetLegend);
            builder.AddCssClass(selectedLedgendClass);

            return builder;
        }

        public static TagBuilder GetFieldSetLegendHeadingBuilder(string formName, FieldSetSize selectedSize, string labelText, bool? disableLabelAndHint)
        {
            var fieldset = GetFieldsetBuilder(formName);
            var fieldsetLegend = GetFieldsetLegendBuilder(selectedSize);
            var fieldsetlegendheader = GetFieldsetLegendHeadingTagBuilder(labelText, disableLabelAndHint, selectedSize);

            fieldsetLegend.InnerHtml.AppendHtml(fieldsetlegendheader);
            fieldset.InnerHtml.AppendHtml(fieldsetLegend);

            return fieldset;
        }

        public static TagBuilder GetFieldsetLegendHeadingTagBuilder(string labelText, bool? disableLabelAndHint, FieldSetSize selectedSize)
        {
            if (labelText == null || disableLabelAndHint == true)
                return null;

            var selectedLedgendTag = selectedSize switch
            {
                FieldSetSize.ExtraLarge => TagHelperConstants.HeaderOne,
                FieldSetSize.Large => TagHelperConstants.HeaderTwo,
                FieldSetSize.Medium => TagHelperConstants.HeaderThree,
                FieldSetSize.Small => "label",
                _ => throw new System.InvalidCastException(),
            };

            var builder = new TagBuilder(selectedLedgendTag);
            builder.AddCssClass(NhsFieldSetLegendHeading);

            builder.InnerHtml.Append(labelText);

            return builder;
        }
    }
}
