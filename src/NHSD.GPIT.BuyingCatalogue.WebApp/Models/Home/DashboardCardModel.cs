using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home;

public class DashboardCardModel : ActionCardModel
{
    public DashboardCardModel(string title, string text, string url)
        : base(title, text, url)
    {
    }

    public DashboardCardModel(string title, string text, string url, string linkText)
        : base(title, text, url, linkText)
    {
    }

    public DashboardCardModel(string title, string text, string url, string linkText, HeadingSize headingSize)
        : this(title, text, url, linkText)
    {
        HeadingSize = headingSize;
    }

    public DashboardCardModel(string title, string text, string url, string linkText, ColumnWidth columnWidth)
        : this(title, text, url, linkText)
    {
        ColumnWidth = columnWidth;
    }

    public DashboardCardModel(string title, string text, string url, string linkText, HeadingSize headingSize, ColumnWidth columnWidth)
        : this(title, text, url, linkText, headingSize)
    {
        ColumnWidth = columnWidth;
    }

    public HeadingSize HeadingSize { get; set; } = HeadingSize.Large;

    public ColumnWidth ColumnWidth { get; set; } = ColumnWidth.OneThird;
}
