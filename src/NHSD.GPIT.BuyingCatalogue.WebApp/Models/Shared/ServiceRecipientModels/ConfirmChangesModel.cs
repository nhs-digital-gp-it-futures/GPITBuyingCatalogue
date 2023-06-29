using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
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

        public JourneyType Journey { get; set; }

        public RoutingSource? Source { get; set; }

        public List<ServiceRecipientModel> Selected { get; set; } =
            Enumerable.Empty<ServiceRecipientModel>().ToList();

        public List<ServiceRecipientModel> PreviouslySelected { get; set; } =
            Enumerable.Empty<ServiceRecipientModel>().ToList();
    }
}
