using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.ImportServiceRecipients;

public class ValidateOdsModel : NavBaseModel
{
    public ValidateOdsModel()
    {
    }

    public ValidateOdsModel(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        string catalogueItemName,
        ServiceRecipientImportMode importMode,
        IEnumerable<ServiceRecipientImportModel> invalidServiceRecipients)
    {
        InternalOrgId = internalOrgId;
        CallOffId = callOffId;
        CatalogueItemId = catalogueItemId;
        CatalogueItemName = catalogueItemName;
        ImportMode = importMode;
        InvalidServiceRecipients = invalidServiceRecipients.OrderBy(s => s.Organisation).ToList();
    }

    public string InternalOrgId { get; set; }

    public CallOffId CallOffId { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public string CatalogueItemName { get; set; }

    public ServiceRecipientImportMode ImportMode { get; set; }

    public IList<ServiceRecipientImportModel> InvalidServiceRecipients { get; set; }
}
