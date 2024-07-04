using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Google;

[HtmlTargetElement(TagName)]
public class GoogleRecaptchaTagHelper : TagHelper
{
    public const string TagName = "recaptcha";

    private const string IdKeyAttribute = "id";
    private const string SiteKeyAttribute = "data-sitekey";
    private const string RecaptchaClass = "g-recaptcha";

    [ViewContext]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var settings = ViewContext.HttpContext.RequestServices.GetRequiredService<IOptions<RecaptchaSettings>>().Value;
        if (!settings.IsEnabled) return;

        var formGroup = TagHelperBuilders.GetFormGroupBuilder();
        var recaptcha = GetRecaptchaTag(settings);

        var isErrored = ViewContext.ModelState.TryGetValue(TagName, out var errorMessage);
        if (isErrored)
        {
            formGroup.AddCssClass(TagHelperConstants.NhsFormGroupError);
            formGroup.InnerHtml.AppendHtml(GetFormError(errorMessage.Errors.First().ErrorMessage));
        }

        formGroup
            .InnerHtml
            .AppendHtml(recaptcha);

        output.TagName = TagHelperConstants.Div;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Content.AppendHtml(formGroup);
    }

    private static TagBuilder GetFormError(string errorMessage)
    {
        var outerBuilder = new TagBuilder(TagHelperConstants.Span);

        outerBuilder.AddCssClass(TagHelperConstants.NhsErrorMessage);
        outerBuilder.GenerateId($"{TagName}-error", "_");

        var innerBuilder = new TagBuilder(TagHelperConstants.Span);

        innerBuilder.AddCssClass("nhsuk-u-visually-hidden");
        innerBuilder.InnerHtml.Append("Error: ");

        outerBuilder.InnerHtml.AppendHtml(innerBuilder);
        outerBuilder.InnerHtml.Append(errorMessage);

        return outerBuilder;
    }

    private static TagBuilder GetRecaptchaTag(RecaptchaSettings settings)
    {
        if (settings is null) throw new InvalidOperationException("Recaptcha settings must not be null");

        var recaptcha = new TagBuilder(TagHelperConstants.Div);

        recaptcha.Attributes.Add(IdKeyAttribute, TagName);
        recaptcha.Attributes.Add(SiteKeyAttribute, settings.SiteKey);
        recaptcha.AddCssClass(RecaptchaClass);

        return recaptcha;
    }
}
