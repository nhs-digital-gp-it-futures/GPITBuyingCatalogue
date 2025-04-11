using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
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
                    cs => new SelectOption<bool> { Text = cs.SublocationOdsCode, Value = true })
                .Concat(
                    possibleSublocations
                        .Where(sl => existingSublocations.All(cs => cs.SublocationOdsCode != sl.OdsCode))
                        .Select(
                            sl => new SelectOption<bool> { Text = sl.OdsCode, Value = false }))
                .OrderBy(x => x.Text)
                .ToList();
        }

        public IReadOnlyList<SelectOption<bool>> RenderedSublocations { get; init; }
    }
}
