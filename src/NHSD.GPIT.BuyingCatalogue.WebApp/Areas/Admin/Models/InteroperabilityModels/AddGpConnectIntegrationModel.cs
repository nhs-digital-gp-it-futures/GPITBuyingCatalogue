using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddGpConnectIntegrationModel : NavBaseModel
    {
        public AddGpConnectIntegrationModel()
        {
            BackLinkText = "Go back";

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

        public AddGpConnectIntegrationModel(CatalogueItemId solutionId)
            : this()
        {
            BackLink = $"/admin/catalogue-solutions/manage/{solutionId}/interoperability";
        }

        public IEnumerable<object> IntegrationTypes { get; }

        [Required(ErrorMessage = "Select an integration type")]
        public string SelectedIntegrationType { get; set; }

        [Required(ErrorMessage = "Select Provider or Consumer")]
        public string SelectedProviderOrConsumer { get; set; }

        public IEnumerable<object> ProviderConsumerTypes { get; }

        [Required(ErrorMessage = "Enter any additional information")]
        [StringLength(1000)]
        public string AdditionalInformation { get; set; }
    }
}
