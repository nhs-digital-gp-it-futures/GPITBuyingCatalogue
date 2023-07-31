using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

        public List<SelectOption<string>> IntegrationTypes => new()
        {
            new("GP Connect - HTML View", "HTML View"),
            new("GP Connect - Appointment Booking", "Appointment Booking"),
            new("GP Connect - Structured Record", "Structured Record"),
        };

        public string SelectedIntegrationType { get; set; }

        public string SelectedProviderOrConsumer { get; set; }

        public List<SelectOption<string>> ProviderConsumerTypes => new()
        {
            new(Framework.Constants.Interoperability.Provider, Framework.Constants.Interoperability.Provider),
            new(Framework.Constants.Interoperability.Consumer, Framework.Constants.Interoperability.Consumer),
        };

        [StringLength(1000)]
        public string AdditionalInformation { get; set; }

        public CatalogueItemId SolutionId { get; init; }

        public Guid IntegrationId { get; init; }
    }
}
