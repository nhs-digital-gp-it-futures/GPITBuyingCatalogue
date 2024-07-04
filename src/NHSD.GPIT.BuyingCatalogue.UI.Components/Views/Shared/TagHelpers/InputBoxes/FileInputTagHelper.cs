using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.InputBoxes;

[HtmlTargetElement(TagHelperName)]
public class FileInputTagHelper : TagHelper
{
    private const string TagHelperName = "nhs-file-upload";

    private readonly IHtmlGenerator htmlGenerator;

    public FileInputTagHelper(IHtmlGenerator htmlGenerator)
    {
        this.htmlGenerator = htmlGenerator;
    }

    public enum FileType
    {
        Csv = 0,
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    [HtmlAttributeName(TagHelperConstants.For)]
    public ModelExpression For { get; set; }

    [HtmlAttributeName(TagHelperConstants.FileType)]
    public FileType File { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var formGroup = TagHelperBuilders.GetFormGroupBuilder();
        var validation = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator);

        var input = htmlGenerator.GenerateTextBox(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            For.Model,
            null,
            new
            {
                @class = TagHelperConstants.GovUkFileUploadInput,
                type = TagHelperConstants.InputTypeFile,
                accept = $"text/{File.ToString()}",
            });

        formGroup.InnerHtml
            .AppendHtml(validation)
            .AppendHtml(input);

        TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, false);
    }
}
