using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage
{
    public sealed class SelectFrameworkModel : NavBaseModel
    {
        public const string TitleText = "Select a framework";

        public SelectFrameworkModel()
        {
        }

        public SelectFrameworkModel(string organisationName, IList<EntityFramework.Catalogue.Models.Framework> frameworks, string selectedFrameworkId)
        {
            OrganisationName = organisationName;
            SelectedFrameworkId = selectedFrameworkId;

            Frameworks = frameworks
                .Select(f => new SelectOption<string>($"{f.ShortName}{(f.IsExpired ? " (Expired)" : string.Empty)}", f.Id, disabled: f.IsExpired))
                .ToList();
        }

        public string OrganisationName { get; set; }

        public string SelectedFrameworkId { get; set; }

        public IList<SelectOption<string>> Frameworks { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public OrderTypeEnum OrderType { get; set; }
    }
}
