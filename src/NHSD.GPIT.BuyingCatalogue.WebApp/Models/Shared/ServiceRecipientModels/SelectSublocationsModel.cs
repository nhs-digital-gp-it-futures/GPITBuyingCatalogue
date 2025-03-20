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
            IEnumerable<ServiceModels.OdsOrganisation> possibleSublocations,
            string backLinkHref)
        {
            Title = "Select sublocations for this order";
            Caption = competition.Name;
            Advice = $"Select all the {competition.Organisation.Name} sublocations that will be receiving this order";
            BackLink = backLinkHref;

            ICollection<EntityModels.CompetitionSublocation> existingSublocations = competition.CompetitionSublocations;

            RenderedSublocations = existingSublocations
                .Select(
                    cs => new CheckboxNameAndValueModel
                    {
                        Name = cs.SublocationOdsCode, Value = CheckboxNameAndValueModel.Selected,
                    })
                .Concat(
                    possibleSublocations
                        .Where(sl => existingSublocations.All(cs => cs.SublocationOdsCode != sl.OdsCode))
                        .Select(
                            sl => new CheckboxNameAndValueModel
                            {
                                Name = sl.OdsCode, Value = CheckboxNameAndValueModel.NotSelected,
                            }))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public IReadOnlyList<CheckboxNameAndValueModel> RenderedSublocations { get; init; }
    }
}
