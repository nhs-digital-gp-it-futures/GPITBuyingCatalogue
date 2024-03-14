using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home
{
    [ExcludeFromCodeCoverage]
    public class EmptyStateModel
    {
        public EmptyStateModel(string entity, string text, string url, string image)
        {
            Entity = entity;
            Text = text;
            Url = url;
            Image = image;
        }

        public string Entity { get; set; }

        public string EntityPlural => Entity + "s";

        public string Text { get; set; }

        public string Url { get; set; }

        public string Image { get; set; }
    }
}
