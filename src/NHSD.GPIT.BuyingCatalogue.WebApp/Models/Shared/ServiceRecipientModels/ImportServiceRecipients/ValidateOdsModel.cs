using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;

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
