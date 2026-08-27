using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home
{
    [ExcludeFromCodeCoverage]
    public class ActionCardModel
    {
        private const string DefaultWidthClass = "nhsuk-grid-column-one-third";

        public ActionCardModel(string title, string text, string url)
        {
            Title = title;
            Text = text;
            Url = url;
        }

        public ActionCardModel(string title, string text, string url, string linkText)
            : this(title, text, url)
        {
            LinkText = linkText;
        }

        public string Title { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string LinkText { get; set; }

        public string WidthClass { get; set; } = DefaultWidthClass;
    }
}
