using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckboxTagHelper.TagHelperName, TagHelperConstants.Input)]
    public sealed class CheckboxContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox-container";

        private ConditionalContext conditionalContext;

        public enum CheckboxSize
        {
            Normal,
            Small,
        }

        [HtmlAttributeName(TagHelperConstants.Size)]
        public CheckboxSize Size { get; set; } = CheckboxSize.Normal;

        public override void Init(TagHelperContext context)
        {
            if (context.Items.TryGetValue(TagHelperConstants.ConditionalContextName, out _))
                return;

            conditionalContext = new ConditionalContext();

            context.Items.Add(TagHelperConstants.ConditionalContextName, conditionalContext);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = await output.GetChildContentAsync();

            var classesToAdd = new List<string> { TagHelperConstants.NhsCheckboxes };

            if (Size == CheckboxSize.Small)
            {
                classesToAdd.Add(TagHelperConstants.NhsCheckboxesSizeSmall);
            }

            if (TagHelperFunctions.ShouldIncludeClassForConditionalContent(context, conditionalContext))
            {
                classesToAdd.Add(TagHelperConstants.NhsCheckBoxParentConditionalClass);
            }

            var existingClass = output.Attributes.FirstOrDefault(f => f.Name == TagHelperConstants.Class);
            if (existingClass != null)
            {
                classesToAdd.AddRange(existingClass.Value.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
                output.Attributes.Remove(existingClass);
            }

            classesToAdd.ToList().ForEach(c => output.AddClass(c, HtmlEncoder.Default));

            output.Content.AppendHtml(content);
        }
    }
}
