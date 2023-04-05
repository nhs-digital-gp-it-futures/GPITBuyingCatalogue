using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients
{
    public class ConfirmChangesModel : NavBaseModel
    {
        public const string AdviceText = "Review the organisations you’ve selected to receive this {0}.";
        public const string AdditionalAdviceText = "Review the new organisations you’ve selected to receive this {0}.";
        public const string TitleText = "Confirm Service Recipients";

        public ConfirmChangesModel()
        {
        }

        public ConfirmChangesModel(Organisation organisation)
        {
            Title = $"{TitleText} for {organisation.Name} ({organisation.ExternalIdentifier})";
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public JourneyType Journey { get; set; }

        public RoutingSource? Source { get; set; }

        public List<ServiceRecipientModel> Selected { get; set; }

        public List<ServiceRecipientModel> PreviouslySelected { get; set; }
    }
}
