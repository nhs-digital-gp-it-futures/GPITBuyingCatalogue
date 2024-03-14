using System.Diagnostics.CodeAnalysis;
using Humanizer;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home
{
    [ExcludeFromCodeCoverage]
    public class EmptyStateModel
    {
        public EmptyStateModel(string entity, string url, string image)
        {
            Entity = entity;
            Url = url;
            Image = image;
        }

        public string Entity { get; set; }

        public string EntityPlural => Entity.Pluralize();

        public string Url { get; set; }

        public string Image { get; set; }
    }
}
