using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddEditGpConnectIntegrationModel : NavBaseModel
    {
        public AddEditGpConnectIntegrationModel()
        {
        }

        public AddEditGpConnectIntegrationModel(CatalogueItem solution)
        {
            SolutionName = solution.Name;
            SolutionId = solution.Id;
        }

        public string SolutionName { get; }

        public List<SelectOption<string>> IntegrationTypes => Interoperability.GpConnectIntegrations
            .Select(x => new SelectOption<string>(x.Value, x.Key))
            .ToList();

        public string SelectedIntegrationType { get; set; }

        public string SelectedProviderOrConsumer { get; set; }

        public List<SelectOption<string>> ProviderConsumerTypes => new()
        {
            new(Interoperability.Provider, Interoperability.Provider),
            new(Interoperability.Consumer, Interoperability.Consumer),
        };

        [StringLength(1000)]
        public string AdditionalInformation { get; set; }

        public CatalogueItemId SolutionId { get; init; }

        public Guid IntegrationId { get; init; }
    }
}
