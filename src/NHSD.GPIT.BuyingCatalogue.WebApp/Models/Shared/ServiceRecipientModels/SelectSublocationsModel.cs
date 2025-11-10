using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using CompetitionEntityModels = NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using ServiceModels = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationsModel : NavBaseModel
    {
        public const string DefaultTitle = "Select sublocations for this order";
        public const string DefaultAdvice = "Select all the {0} sublocations that will receive this order.";
        public const string MergerTitle = "Select sublocations for this merger";
        public const string MergerAdvice = "Select all the {0} sublocations that will be involved in the merger. They must all be using the same catalogue solution.";
        public const string SplitTitle = "Select sublocations for this split";
        public const string SplitAdvice = "Select all the {0} sublocations that will be involved in this split. They must all be using the same catalogue solution.";

        public SelectSublocationsModel()
        {
        }

        public SelectSublocationsModel(
            CompetitionEntityModels.Competition competition,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            string backLink)
        {
            Caption = competition.Name;
            BackLink = backLink;

            Advice = string.Format(DefaultAdvice, competition.Organisation.Name);

            List<SublocationModel> existingSublocations =
                competition.CompetitionSublocations.Select(x => new SublocationModel(x, true)).ToList();

            RenderedSublocations =
                GetRenderedSublocations(possibleSublocations, existingSublocations);
        }

        public SelectSublocationsModel(
            OrderWrapper wrapper,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            string backLink)
        {
            Title = DefaultTitle;
            Caption = wrapper.Order.CallOffId.ToString();
            BackLink = backLink;

            IsAmendment = wrapper.IsAmendment;
            var isMergerOrSplit = wrapper.Order.OrderType.MergerOrSplit;
            var isMerger = wrapper.Order.OrderType.ToPracticeReorganisationType == PracticeReorganisationTypeEnum.Merger;
            var organisationName = wrapper.Order.OrderingParty?.Name ?? "organisation";
            SetDisplayContent(isMergerOrSplit, isMerger, organisationName);

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

        private static List<SelectOption<string>> GetRenderedSublocations(
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            ICollection<SublocationModel> existingSublocations)
        {
            return possibleSublocations
                .Select(sl =>
                {
                    var selected = existingSublocations.Select(es => es.OdsCode).Contains(sl.OdsCode);
                    return new SelectOption<string>(sl.OdsCode, sl.OdsCode, selected);
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

                    return new SelectOption<string>(
                        sl.OdsCode,
                        sl.OdsCode,
                        selected,
                        sublocationPartOfPreviousOrder);
                })
                .OrderBy(x => x.Text)
                .ToList();
        }

        private void SetDisplayContent(bool isMergerOrSplit, bool isMerger, string organisationName)
        {
            if (isMergerOrSplit)
            {
                Title = isMerger
                    ? MergerTitle
                    : SplitTitle;
                Advice = isMerger
                    ? string.Format(MergerAdvice, organisationName)
                    : string.Format(SplitAdvice, organisationName);
            }
            else
            {
                Title = DefaultTitle;
                Advice = string.Format(DefaultAdvice, organisationName);
            }
        }
    }
}
