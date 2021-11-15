using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddEditIm1IntegrationModel : NavBaseModel
    {
        public AddEditIm1IntegrationModel()
        {
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

        public AddEditIm1IntegrationModel(CatalogueItem solution)
            : this()
        {
            SolutionName = solution.Name;
            SolutionId = solution.Id;
        }

        public string SolutionName { get; }

        public IEnumerable<object> IntegrationTypes { get; }

        [Required(ErrorMessage = "Select integration type")]
        public string SelectedIntegrationType { get; set; }

        [Required(ErrorMessage = "Select if your system is a provider or consumer")]
        public string SelectedProviderOrConsumer { get; set; }

        public IEnumerable<object> ProviderConsumerTypes { get; }

        [Required(ErrorMessage = "Enter the system being integrated with")]
        [StringLength(100)]
        public string IntegratesWith { get; set; }

        [Required(ErrorMessage = "Enter a description")]
        [StringLength(1000)]
        public string Description { get; set; }

        public CatalogueItemId SolutionId { get; }

        public Guid IntegrationId { get; set; }
    }
}
