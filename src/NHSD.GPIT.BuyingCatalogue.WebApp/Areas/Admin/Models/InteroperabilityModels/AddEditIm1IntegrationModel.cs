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
    public class AddEditIm1IntegrationModel : NavBaseModel
    {
        public AddEditIm1IntegrationModel()
        {
        }

        public AddEditIm1IntegrationModel(
            CatalogueItem solution,
            IEnumerable<IntegrationType> integrationTypes)
        {
            SolutionName = solution.Name;
            SolutionId = solution.Id;

            WithIntegrationTypes(integrationTypes);
        }

        public AddEditIm1IntegrationModel(
            CatalogueItem solution,
            IEnumerable<IntegrationType> integrationTypes,
            SolutionIntegration solutionIntegration)
        : this(solution, integrationTypes)
        {
            IntegrationId = solutionIntegration.Id;
            SelectedIntegrationType = solutionIntegration.IntegrationTypeId;
            IsConsumer = solutionIntegration.IsConsumer.GetValueOrDefault();
            Description = solutionIntegration.Description;
            IntegratesWith = solutionIntegration.IntegratesWith;
        }

        public string SolutionName { get; set; }

        public List<SelectOption<string>> IntegrationTypes { get; set; }

        public int? SelectedIntegrationType { get; set; }

        public bool? IsConsumer { get; set; }

        public List<SelectOption<string>> ProviderConsumerTypes => new()
        {
            new(Interoperability.Provider, false.ToString()),
            new(Interoperability.Consumer, true.ToString()),
        };

        [StringLength(100)]
        public string IntegratesWith { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public int? IntegrationId { get; set; }

        public AddEditIm1IntegrationModel WithIntegrationTypes(IEnumerable<IntegrationType> integrationTypes)
        {
            IntegrationTypes = integrationTypes.Select(x => new SelectOption<string>(x.Name, x.Id.ToString())).ToList();

            return this;
        }
    }
}
