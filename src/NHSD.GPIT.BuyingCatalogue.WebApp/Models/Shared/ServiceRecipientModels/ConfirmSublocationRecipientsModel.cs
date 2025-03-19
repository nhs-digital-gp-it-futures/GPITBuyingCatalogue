using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class ConfirmSublocationRecipientsModel : NavBaseModel
    {
        public ConfirmSublocationRecipientsModel()
        {
        }

        public ConfirmSublocationRecipientsModel(
            Competition competition,
            List<SublocationModel> sublocations,
            string addRemoveRecipientsLink)
        {
            Title = "Confirm service recipients)";
            Caption = competition.Name;
            Advice = "Review the organisations you've selected to receive the winning solution for this competition.";

            Sublocations = sublocations;
            AddRemoveRecipientsLink = addRemoveRecipientsLink;
        }

        public string AddRemoveRecipientsLink { get; init; }

        public List<SublocationModel> Sublocations { get; init; } = [];
    }
}
