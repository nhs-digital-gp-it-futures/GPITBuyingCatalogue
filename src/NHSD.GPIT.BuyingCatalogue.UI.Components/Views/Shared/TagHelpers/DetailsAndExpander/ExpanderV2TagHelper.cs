using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(ExpanderContentTagHelper.TagHelperName, ExpanderSummaryTagHelper.TagHelperName, ExpanderFooterTagHelper.TagHelperName)]
    public sealed class ExpanderV2TagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander-v2";
        public const string OpenName = "open";
        private const string CatchErrorsName = "catches-errors";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(OpenName)]
        public bool Open { get; set; }

        [HtmlAttributeName(CatchErrorsName)]
        public bool CatchesErrors { get; set; } = false;

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue("InExpanderContext", out _))
                context.Items.Add("InExpanderContext", true);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var expanderContext = new ExpanderContext();
            context.Items.Add(typeof(ExpanderV2TagHelper), expanderContext);

            _ = await output.GetChildContentAsync();

            output.TagName = "details";
            output.TagMode = TagMode.StartTagAndEndTag;

            var classAttribute = $"{DetailsAndExpanderTagHelperBuilders.DetailsClass} {TagHelperConstants.NhsExpanderClass} {TagHelperConstants.NhsExpanderIndexClass}";

            if (CatchesErrors && For is not null && TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, For))
                classAttribute += $" {TagHelperConstants.NhsExpanderError}";

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, classAttribute));

            if (For is not null)
                output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Id, TagBuilder.CreateSanitizedId($"{For.Name}", "_")));

            if (Open)
                output.Attributes.Add("open", string.Empty);

            output.Content
                .AppendHtml(expanderContext.SummaryContent)
                .AppendHtml(expanderContext.BodyContent)
                .AppendHtml(expanderContext.FooterContent);
        }
    }
}
