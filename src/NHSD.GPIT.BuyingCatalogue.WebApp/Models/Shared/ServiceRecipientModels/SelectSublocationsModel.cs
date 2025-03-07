using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using EntityModels = NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using ServiceModels = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationsModel : NavBaseModel
    {
        public SelectSublocationsModel(
            EntityModels.Competition competition,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            bool isInitialSelection)
        {
            Title = "Select sublocations for this order";
            Caption = competition.Name;
            Advice = $"Select all the {competition.Organisation.Name} sublocations that will be receiving this order";

            PossibleSublocations = possibleSublocations;
            ActualSublocations = competition.CompetitionSublocations;

            PopulateCheckedState(isInitialSelection);
        }

        public IEnumerable<ServiceModels.OdsOrganisation> PossibleSublocations { get; set; }

        public IEnumerable<EntityModels.CompetitionSublocation> ActualSublocations { get; set; }

        public List<CheckboxNameAndValueModel> CheckedSublocations { get; set; } = [];

        private void PopulateCheckedState(bool isInitialSelection)
        {
            if (isInitialSelection)
            {
                CheckedSublocations.AddRange(
                    PossibleSublocations.Select(
                        sl => new CheckboxNameAndValueModel { Name = sl.OdsCode, Value = false }));
            }
            else
            {
                CheckedSublocations.AddRange(
                    ActualSublocations.Select(
                        cs => new CheckboxNameAndValueModel { Name = cs.SublocationOdsCode, Value = cs.Selected }));

                CheckedSublocations.AddRange( // Fill in sublocations if they are missing
                    PossibleSublocations
                        .Where(sl => CheckedSublocations.All(x => x.Name != sl.OdsCode))
                        .Select(sl => new CheckboxNameAndValueModel { Name = sl.OdsCode, Value = false }));
            }
        }
    }
}
