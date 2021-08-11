using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddIm1IntegrationModel : NavBaseModel
    {
        public AddIm1IntegrationModel()
        {
            BackLink = "./";
            BackLinkText = "Go back";

            IntegrationTypes = new List<object>
            {
                new { Text = "IM1 Bulk", Value = "Bulk" },
                new { Text = "IM1 Transactional", Value = "Transactional" },
                new { Text = "IM1 Patient Facing", Value = "Patient Facing" },
            };

            ProviderConsumerTypes = new List<object>
            {
                new { Text = Framework.Constants.Interoperability.Provider, Value = Framework.Constants.Interoperability.Provider },
                new { Text = Framework.Constants.Interoperability.Consumer, Value = Framework.Constants.Interoperability.Consumer },
            };
        }

        public IEnumerable<object> IntegrationTypes { get; }

        [Required(ErrorMessage = "Select an integration type")]
        public string SelectedIntegrationType { get; set; }

        [Required(ErrorMessage = "Select Provider or Consumer")]
        public string SelectedProviderOrConsumer { get; set; }

        public IEnumerable<object> ProviderConsumerTypes { get; }

        [Required(ErrorMessage = "Enter what system it integrates with")]
        [StringLength(100)]
        public string IntegratesWith { get; set; }

        [Required(ErrorMessage = "Enter a description")]
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
