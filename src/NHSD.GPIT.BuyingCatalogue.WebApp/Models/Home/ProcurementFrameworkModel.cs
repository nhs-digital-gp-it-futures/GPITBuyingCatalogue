using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home
{
    [ExcludeFromCodeCoverage]
    public class ProcurementFrameworkModel
    {
        public ProcurementFrameworkModel(string title, string text, string url, string linkText)
        {
            Title = title;
            Text = text;
            Url = url;
            LinkText = linkText;
        }

        public string Title { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string LinkText { get; set; }
    }
}
