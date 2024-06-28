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

        public AddEditGpConnectIntegrationModel(
            CatalogueItem solution,
            IEnumerable<IntegrationType> integrationTypes)
        {
            SolutionName = solution.Name;
            SolutionId = solution.Id;
            IntegrationTypes = integrationTypes.Select(x => new SelectOption<string>(x.Name, x.Id.ToString())).ToList();
        }

        public AddEditGpConnectIntegrationModel(
            CatalogueItem solution,
            IEnumerable<IntegrationType> integrationTypes,
            SolutionIntegration solutionIntegration)
            : this(solution, integrationTypes)
        {
            IntegrationTypeId = solutionIntegration.IntegrationTypeId;
            SelectedIntegrationType = solutionIntegration.IntegrationTypeId;
            IsConsumer = solutionIntegration.IsConsumer.GetValueOrDefault();
            AdditionalInformation = solutionIntegration.Description;
        }

        public string SolutionName { get; }

        public List<SelectOption<string>> IntegrationTypes { get; set; }

        public int? SelectedIntegrationType { get; set; }

        public bool IsConsumer { get; set; }

        public List<SelectOption<string>> ProviderConsumerTypes => new()
        {
            new(Interoperability.Provider, false.ToString()),
            new(Interoperability.Consumer, true.ToString()),
        };

        [StringLength(1000)]
        public string AdditionalInformation { get; set; }

        public CatalogueItemId SolutionId { get; init; }

        public int? IntegrationTypeId { get; set; }
    }
}
