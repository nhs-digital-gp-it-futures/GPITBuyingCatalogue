using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card;

public class CardContext
{
    public TagBuilder BodyContent { get; set; }

    public IHtmlContent FooterContent { get; set; }

    public bool HorizontalAlign { get; set; }

    public bool ShouldBeClickable { get; set; }
}
