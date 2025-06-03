using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using CompetitionEntityModels = NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
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

            List<SublocationModel> existingSublocations =
                competition.CompetitionSublocations.Select(x => new SublocationModel(x, true)).ToList();

            RenderedSublocations =
                GetRenderedSublocations(possibleSublocations, existingSublocations);
        }

        public SelectSublocationsModel(
            OrderWrapper wrapper,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            string backLinkHref)
        {
            Title = "Select sublocations for this order";
            Caption = wrapper.Order.Description;
            BackLink = backLinkHref;

            IsAmendment = wrapper.IsAmendment;

            FormLabelText =
                $"Select all the {wrapper.Order.OrderingParty.Name} sublocations that will receive this order";

            // Current and previous order sublocations
            List<SublocationModel> existingSublocations =
                wrapper.Order.OrderSublocations.Select(x => new SublocationModel(x, true)).ToList();

            // Only previous order sublocations
            List<SublocationModel> previousOrderSublocations =
                wrapper.Previous?.OrderSublocations?.Select(x => new SublocationModel(x, true)).ToList();

            RenderedSublocations = IsAmendment is true
                ? GetRenderedSublocations(possibleSublocations, existingSublocations, previousOrderSublocations)
                : GetRenderedSublocations(possibleSublocations, existingSublocations);
        }

        public IReadOnlyList<SelectOption<string>> RenderedSublocations { get; init; }

        public bool? IsAmendment { get; init; }

        public string FormLabelText { get; init; }

        private static List<SelectOption<string>> GetRenderedSublocations(
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            ICollection<SublocationModel> existingSublocations)
        {
            return possibleSublocations
                .Select(sl =>
                {
                    var selected = existingSublocations.Select(es => es.OdsCode).Contains(sl.OdsCode);
                    return new SelectOption<string> { Text = sl.OdsCode, Value = sl.OdsCode, Selected = selected };
                })
                .OrderBy(x => x.Text)
                .ToList();
        }

        private static List<SelectOption<string>> GetRenderedSublocations(
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            ICollection<SublocationModel> existingSublocations,
            ICollection<SublocationModel> previousOrderSublocations)
        {
            return possibleSublocations
                .Select(sl =>
                {
                    var selected = previousOrderSublocations.Any(ps => ps.OdsCode == sl.OdsCode)
                        || existingSublocations.Any(es => es.OdsCode == sl.OdsCode);

                    var sublocationPartOfPreviousOrder = previousOrderSublocations.Any(x => x.OdsCode == sl.OdsCode);

                    return new SelectOption<string>
                    {
                        Text = sl.OdsCode,
                        Value = sl.OdsCode,
                        Selected = selected,
                        Hidden = sublocationPartOfPreviousOrder,
                    };
                })
                .OrderBy(x => x.Text)
                .ToList();
        }
    }
}
