using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public class ConfirmChangesModel : NavBaseModel
    {
        public const string TitleText = "Confirm Service Recipients";
        private string addRemoveRecipientsLink;

        public ConfirmChangesModel()
        {
        }

        public ConfirmChangesModel(Organisation organisation)
        {
            Title = $"{TitleText} for {organisation.Name} ({organisation.ExternalIdentifier})";
        }

        public string AddRemoveRecipientsLink
        {
            get
            {
                return addRemoveRecipientsLink == null
                    ? BackLink
                    : addRemoveRecipientsLink;
            }

            set
            {
                addRemoveRecipientsLink = value;
            }
        }

        public OrderType OrderType { get; set; }

        public ServiceRecipientModel PracticeReorganisationRecipient { get; set; }

        public List<ServiceRecipientModel> Selected { get; set; } =
            Enumerable.Empty<ServiceRecipientModel>().ToList();

        public List<ServiceRecipientModel> PreviouslySelected { get; set; } =
            Enumerable.Empty<ServiceRecipientModel>().ToList();
    }
}
