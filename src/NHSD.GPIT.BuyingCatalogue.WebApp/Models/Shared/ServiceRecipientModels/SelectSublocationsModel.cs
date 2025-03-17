using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using EntityModels = NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using ServiceModels = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationsModel : NavBaseModel
    {
        private const bool Selected = true;
        private const bool NotSelected = false;

        public SelectSublocationsModel(
            EntityModels.Competition competition,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations)
        {
            Title = "Select sublocations for this order";
            Caption = competition.Name;
            Advice = $"Select all the {competition.Organisation.Name} sublocations that will be receiving this order";

            PossibleSublocations = possibleSublocations;
            ActualSublocations = competition.CompetitionSublocations;

            PopulateCheckedState();
        }

        public IEnumerable<ServiceModels.OdsOrganisation> PossibleSublocations { get; init; }

        public IEnumerable<EntityModels.CompetitionSublocation> ActualSublocations { get; init; }

        public List<CheckboxNameAndValueModel> RenderedSublocations { get; init; } = [];

        private void PopulateCheckedState()
        {
            RenderedSublocations.AddRange(
                ActualSublocations.Select(
                    cs => new CheckboxNameAndValueModel { Name = cs.SublocationOdsCode, Value = Selected }));

            RenderedSublocations.AddRange(
                PossibleSublocations
                    .Where(sl => RenderedSublocations.All(x => x.Name != sl.OdsCode))
                    .Select(sl => new CheckboxNameAndValueModel { Name = sl.OdsCode, Value = NotSelected }));
        }
    }
}
