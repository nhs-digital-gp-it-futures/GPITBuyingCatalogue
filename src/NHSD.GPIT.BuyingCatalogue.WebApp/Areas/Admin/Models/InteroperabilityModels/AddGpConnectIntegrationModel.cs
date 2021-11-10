using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddGpConnectIntegrationModel : NavBaseModel
    {
        public AddGpConnectIntegrationModel()
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

        public AddGpConnectIntegrationModel(CatalogueItem solution)
            : this()
        {
            BackLink = $"/admin/catalogue-solutions/manage/{solution.Id}/interoperability";
            SolutionName = solution.Name;
        }

        public string SolutionName { get; }

        public IEnumerable<object> IntegrationTypes { get; }

        [Required(ErrorMessage = "Select integration type")]
        public string SelectedIntegrationType { get; set; }

        [Required(ErrorMessage = "Select if your system is a provider or consumer")]
        public string SelectedProviderOrConsumer { get; set; }

        public IEnumerable<object> ProviderConsumerTypes { get; }

        [Required(ErrorMessage = "Enter additional information")]
        [StringLength(1000)]
        public string AdditionalInformation { get; set; }
    }
}
