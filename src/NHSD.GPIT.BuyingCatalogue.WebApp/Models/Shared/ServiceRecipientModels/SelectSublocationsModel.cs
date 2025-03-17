using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using EntityModels = NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using ServiceModels = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationsModel : NavBaseModel
    {
        public SelectSublocationsModel()
        {
        }

        public SelectSublocationsModel(
            EntityModels.Competition competition,
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations)
        {
            Title = "Select sublocations for this order";
            Caption = competition.Name;
            Advice = $"Select all the {competition.Organisation.Name} sublocations that will be receiving this order";

            PossibleSublocations = possibleSublocations;
            ActualSublocations = competition.CompetitionSublocations;

            PopulateRenderedSublocations();
        }

        public List<CheckboxNameAndValueModel> RenderedSublocations { get; init; } = [];

        private IEnumerable<ServiceModels.OdsOrganisation> PossibleSublocations { get; }

        private IEnumerable<EntityModels.CompetitionSublocation> ActualSublocations { get; }

        private void PopulateRenderedSublocations()
        {
            RenderedSublocations.AddRange(
                ActualSublocations.Select(
                    cs => new CheckboxNameAndValueModel
                    {
                        Name = cs.SublocationOdsCode, Value = CheckboxNameAndValueModel.Selected,
                    }));

            RenderedSublocations.AddRange(
                PossibleSublocations
                    .Where(sl => RenderedSublocations.All(x => x.Name != sl.OdsCode))
                    .Select(
                        sl => new CheckboxNameAndValueModel
                        {
                            Name = sl.OdsCode, Value = CheckboxNameAndValueModel.NotSelected,
                        }));
            RenderedSublocations.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        }
    }
}
