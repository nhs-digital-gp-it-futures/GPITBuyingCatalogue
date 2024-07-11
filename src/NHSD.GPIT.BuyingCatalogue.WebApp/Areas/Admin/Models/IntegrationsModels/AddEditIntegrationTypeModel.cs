using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;

public class AddEditIntegrationTypeModel : NavBaseModel
{
    internal const string EditTitle = "Edit integration type";
    internal const string AddTitle = "Add integration type";

    public AddEditIntegrationTypeModel()
    {
    }

    public AddEditIntegrationTypeModel(
        Integration integration)
    {
        IntegrationId = integration.Id;
        IntegrationName = integration.Name;
        RequiresDescription = integration.RequiresDescription;
    }

    public AddEditIntegrationTypeModel(
        Integration integration,
        IntegrationType integrationType)
        : this(integration)
    {
        IntegrationTypeId = integrationType.Id;
        IntegrationTypeName = integrationType.Name;
        Description = integrationType.Description;
    }

    public override string Title =>
        IntegrationTypeId is not null
            ? EditTitle
            : AddTitle;

    public SupportedIntegrations? IntegrationId { get; set; }

    public string IntegrationName { get; set; }

    public bool RequiresDescription { get; set; }

    public int? IntegrationTypeId { get; set; }

    [StringLength(50)]
    public string IntegrationTypeName { get; set; }

    [StringLength(350)]
    public string Description { get; set; }
}
