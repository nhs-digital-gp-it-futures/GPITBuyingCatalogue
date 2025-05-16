using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using CompetitionEntityModels = NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using OrderEntityModels = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using ServiceModels = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationsModel : NavBaseModel
    {
        public SelectSublocationsModel()
        {
        }

        public SelectSublocationsModel(
            CompetitionEntityModels.Competition competition,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            string backLinkHref)
        {
            Title = "Select sublocations for this competition";
            Caption = competition.Name;
            BackLink = backLinkHref;

            FormLabelText =
                $"Select all the {competition.Organisation.Name} sublocations that will be part of this competition";

            ICollection<ISublocation> existingSublocations =
                competition.CompetitionSublocations.Cast<ISublocation>().ToList();

            RenderedSublocations =
                GetRenderedSublocations(possibleSublocations, existingSublocations);
        }

        public SelectSublocationsModel(
            OrderEntityModels.Order order,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            string backLinkHref,
            bool isAmendment)
        {
            Title = "Select sublocations for this order";
            Caption = order.Description;
            BackLink = backLinkHref;

            IsAmendment = isAmendment;

            FormLabelText = $"Select all the {order.OrderingParty.Name} sublocations that will receive this order";

            ICollection<ISublocation> existingSublocations = order.OrderSublocations.Cast<ISublocation>().ToList();

            RenderedSublocations =
                GetRenderedSublocations(possibleSublocations, existingSublocations);
        }

        public IReadOnlyList<SelectOption<string>> RenderedSublocations { get; init; }

        public bool? IsAmendment { get; init; }

        public string FormLabelText { get; init; }

        private List<SelectOption<string>> GetRenderedSublocations(
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            ICollection<ISublocation> existingSublocations)
        {
            return possibleSublocations
                .Select(sl =>
                {
                    var selected = existingSublocations.Select(es => es.SublocationOdsCode).Contains(sl.OdsCode);
                    return new SelectOption<string>
                    {
                        Text = sl.OdsCode,
                        Value = sl.OdsCode,
                        Selected = selected,
                        Disabled = selected && IsAmendment is true,
                    };
                })
                .OrderBy(x => x.Text)
                .ToList();
        }
    }
}
