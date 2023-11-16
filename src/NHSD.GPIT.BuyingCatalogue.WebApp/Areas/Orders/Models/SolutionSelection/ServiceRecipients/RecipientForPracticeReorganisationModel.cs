using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients
{
    public class RecipientForPracticeReorganisationModel : NavBaseModel
    {
        public RecipientForPracticeReorganisationModel()
        {
        }

        public RecipientForPracticeReorganisationModel(
            Organisation organisation,
            List<ServiceRecipientModel> recipients)
        {
            OrganisationName = organisation.Name;
            OrganisationType = organisation.OrganisationType.GetValueOrDefault();

            SubLocations = recipients
                .GroupBy(x => x.Location)
                .Select(
                    x => new SublocationModel(
                        x.Key,
                        x.ToList()))
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public string OrganisationName { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public SublocationModel[] SubLocations { get; set; } = Array.Empty<SublocationModel>();

        public string SelectedOdsCode { get; set; }
    }
}
