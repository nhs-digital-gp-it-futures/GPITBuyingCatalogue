using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public class AutoCompleteTagHelper : SelectListTagHelper
    {
        public new const string TagHelperName = "nhs-auto-complete";

        private const string AccessibleAutoCompleteSourceFile = "~/js/accessible-autocomplete/accessible-autocomplete.min.js";
        private const string AccessibleAutoCompleteConfigFile = "~/js/accessible-autocomplete/autoCompleteConfig.js";

        private readonly IUrlHelper urlHelper;

        public AutoCompleteTagHelper(
            IActionContextAccessor actionContextAccessor,
            IHtmlGenerator htmlGenerator,
            IUrlHelperFactory urlHelperFactory)
            : base(htmlGenerator)
        {
            urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var formGroup = BuildSelect();

            formGroup.InnerHtml
                .AppendHtml(ScriptInclude(AccessibleAutoCompleteSourceFile))
                .AppendHtml(ScriptInclude(AccessibleAutoCompleteConfigFile))
                .AppendHtml(AutoCompleteScriptExecution());

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, true);
        }

        private TagBuilder ScriptInclude(string scriptLocation)
        {
            var builder = new TagBuilder(TagHelperConstants.Script);

            builder.Attributes.Add(TagHelperConstants.Type, TagHelperConstants.Javascript);
            builder.Attributes.Add(TagHelperConstants.Source, urlHelper.Content(scriptLocation));

            return builder;
        }

        private TagBuilder AutoCompleteScriptExecution()
        {
            var builder = new TagBuilder(TagHelperConstants.Script);

            builder.Attributes.Add(TagHelperConstants.Type, TagHelperConstants.Javascript);
            builder.InnerHtml.AppendHtml($"new autoCompleteConfig('#{For.Name}').Implement();");

            return builder;
        }
    }
}
