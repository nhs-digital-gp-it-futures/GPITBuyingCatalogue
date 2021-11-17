using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddEditGpConnectIntegrationModel : NavBaseModel
    {
        public AddEditGpConnectIntegrationModel()
        {
            IntegrationTypes = new List<object>
            {
                new { Text = "GP Connect - HTML View", Value = "HTML View" },
                new { Text = "GP Connect - Appointment Booking", Value = "Appointment Booking" },
                new { Text = "GP Connect - Structured Record", Value = "Structured Record" },
            };

            ProviderConsumerTypes = new List<object>
            {
                new { Text = Framework.Constants.Interoperability.Provider, Value = Framework.Constants.Interoperability.Provider },
                new { Text = Framework.Constants.Interoperability.Consumer, Value = Framework.Constants.Interoperability.Consumer },
            };
        }

        public AddEditGpConnectIntegrationModel(CatalogueItem solution)
            : this()
        {
            SolutionName = solution.Name;
            SolutionId = solution.Id;
        }

        public string SolutionName { get; }

        public IEnumerable<object> IntegrationTypes { get; }

        public string SelectedIntegrationType { get; set; }

        public string SelectedProviderOrConsumer { get; set; }

        public IEnumerable<object> ProviderConsumerTypes { get; }

        [StringLength(1000)]
        public string AdditionalInformation { get; set; }

        public CatalogueItemId SolutionId { get; init; }

        public Guid IntegrationId { get; init; }
    }
}
