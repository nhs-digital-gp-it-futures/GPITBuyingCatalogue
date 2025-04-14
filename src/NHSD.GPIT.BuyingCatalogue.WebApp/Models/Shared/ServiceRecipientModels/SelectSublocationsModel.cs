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

            RenderedSublocations =
                possibleSublocations
                    .Select(
                        sl => new SelectOption<string>
                        {
                            Text = sl.OdsCode,
                            Value = sl.OdsCode,
                            Selected = existingSublocations.Select(es => es.SublocationOdsCode).Contains(sl.OdsCode),
                        })
                    .OrderBy(x => x.Text)
                    .ToList();
        }

        public IReadOnlyList<SelectOption<string>> RenderedSublocations { get; init; }
    }
}
