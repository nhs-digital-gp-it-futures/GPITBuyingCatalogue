using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander;

public class ExpanderContext
{
    public TagBuilder BodyContent { get; set; }

    public TagBuilder SummaryContent { get; set; }

    public IHtmlContent FooterContent { get; set; }
}
