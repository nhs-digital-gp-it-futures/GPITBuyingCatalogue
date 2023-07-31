using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddEditIm1IntegrationModel : NavBaseModel
    {
        public AddEditIm1IntegrationModel()
        {
        }

        public AddEditIm1IntegrationModel(CatalogueItem solution)
        {
            SolutionName = solution.Name;
            SolutionId = solution.Id;
        }

        public string SolutionName { get; }

        public List<SelectOption<string>> IntegrationTypes => new()
        {
            new("IM1 Bulk", "Bulk"),
            new("IM1 Transactional", "Transactional"),
            new("IM1 Patient Facing", "Patient Facing"),
        };

        public string SelectedIntegrationType { get; set; }

        public string SelectedProviderOrConsumer { get; set; }

        public List<SelectOption<string>> ProviderConsumerTypes => new()
        {
            new(Framework.Constants.Interoperability.Provider, Framework.Constants.Interoperability.Provider),
            new(Framework.Constants.Interoperability.Consumer, Framework.Constants.Interoperability.Consumer),
        };

        [StringLength(100)]
        public string IntegratesWith { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public CatalogueItemId SolutionId { get; }

        public Guid IntegrationId { get; set; }
    }
}
