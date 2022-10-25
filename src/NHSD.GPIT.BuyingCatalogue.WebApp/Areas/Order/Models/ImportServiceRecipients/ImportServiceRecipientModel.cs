using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ImportServiceRecipients;

public class ImportServiceRecipientModel : NavBaseModel
{
    public ImportServiceRecipientModel()
    {
    }

    public ImportServiceRecipientModel(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        string catalogueItemName)
    {
        InternalOrgId = internalOrgId;
        CallOffId = callOffId;
        CatalogueItemId = catalogueItemId;
        CatalogueItemName = catalogueItemName;
    }

    public string InternalOrgId { get; set; }

    public CallOffId CallOffId { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public string CatalogueItemName { get; set; }

    public IFormFile File { get; set; }
}
