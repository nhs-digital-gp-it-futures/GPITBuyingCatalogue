using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;

public class ValidateOdsModel : NavBaseModel
{
    public ValidateOdsModel()
    {
    }

    public ValidateOdsModel(
        IEnumerable<ServiceRecipientImportModel> invalidServiceRecipients)
    {
        InvalidServiceRecipients = invalidServiceRecipients.OrderBy(s => s.Organisation).ToList();
    }

    public string CancelLink { get; set; }

    public string ValidateNamesLink { get; set; }

    public IList<ServiceRecipientImportModel> InvalidServiceRecipients { get; set; }
}
