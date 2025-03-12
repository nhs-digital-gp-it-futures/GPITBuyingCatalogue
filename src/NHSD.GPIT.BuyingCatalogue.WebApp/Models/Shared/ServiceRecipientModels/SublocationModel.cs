using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public class SublocationModel
{
    public SublocationModel()
    {
    }

    public SublocationModel(string name, List<ServiceRecipientModel> serviceRecipients)
    {
        Name = name;
        ServiceRecipients = serviceRecipients;
    }

    public string Name { get; set; }

    public string OdsCode { get; set; }

    public List<ServiceRecipientModel> ServiceRecipients { get; set; }

    public int ServiceRecipientCount { get; set; }

    public TaskProgress TaskProgress => ServiceRecipientCount == 0 ? TaskProgress.NotStarted : TaskProgress.Completed;

    public bool AllRecipientsSelected => ServiceRecipients.All(x => x.Selected);

    public string RecipientHref { get; set; }
}
