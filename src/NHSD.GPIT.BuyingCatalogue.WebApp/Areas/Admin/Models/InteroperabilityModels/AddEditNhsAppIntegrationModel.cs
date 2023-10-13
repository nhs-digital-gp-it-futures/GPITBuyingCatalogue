using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class AddEditNhsAppIntegrationModel : NavBaseModel
    {
        public AddEditNhsAppIntegrationModel()
        {
        }

        public AddEditNhsAppIntegrationModel(CatalogueItem solution)
        {
            SolutionName = solution.Name;
            SolutionId = solution.Id;
            SetIntegrationTypes();
            var integrations = solution.Solution.GetIntegrations();
            CheckIntegrationTypes(integrations);
        }

        public string SolutionName { get; }

        public NhsAppIntegrationTypeModel[] NhsAppIntegrationTypes { get; set; }

        public CatalogueItemId SolutionId { get; init; }

        public Guid IntegrationId { get; set; }

        public void SetIntegrationTypes()
        {
            NhsAppIntegrationTypes = new NhsAppIntegrationTypeModel[]
            {
                new() { IntegrationType = "Online Consultation Services" },
                new() { IntegrationType = "Notifications & Messaging" },
                new() { IntegrationType = "Secondary Care Appointments" },
                new() { IntegrationType = "Personal Health Records (PHR)" },
                new() { IntegrationType = "Secondary Care Notifications & Messaging" },
                new() { IntegrationType = "Register with a GP" },
                new() { IntegrationType = "Apply to participate in Health & Care research" },
            };
        }

        public void CheckIntegrationTypes(ICollection<Integration> integrations)
        {
            var nhsAppIntegration = integrations?.FirstOrDefault(i => i.IntegrationType.EqualsIgnoreCase(Framework.Constants.Interoperability.NhsAppIntegrationType));

            string[] nhsAppIntegrationTypes = nhsAppIntegration?.NHSAppIntegrationTypes?.Distinct().ToArray();

            if (nhsAppIntegrationTypes != null)
            {
                 IntegrationId = integrations
                         .Where(i => i.IntegrationType.EqualsIgnoreCase(Framework.Constants.Interoperability.NhsAppIntegrationType))
                         .Select(i => i.Id)
                         .FirstOrDefault();
                 foreach (var integrationType in NhsAppIntegrationTypes)
                 {
                     if (nhsAppIntegrationTypes.Any(s => s.Equals(integrationType.IntegrationType, StringComparison.InvariantCultureIgnoreCase)))
                     {
                         integrationType.Checked = true;
                     }
                 }
            }
        }
    }
}
