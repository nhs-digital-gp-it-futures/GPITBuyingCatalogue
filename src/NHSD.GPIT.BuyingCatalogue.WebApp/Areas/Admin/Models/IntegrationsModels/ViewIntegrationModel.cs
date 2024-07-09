using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;

public class ViewIntegrationModel(Integration integration) : NavBaseModel
{
    public string IntegrationName { get; set; } = integration.Name;

    public ICollection<IntegrationType> IntegrationTypes { get; set; } = integration.IntegrationTypes;
}
