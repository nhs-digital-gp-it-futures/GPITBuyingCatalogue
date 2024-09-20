using System.Collections.Generic;
using System.Linq;

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

    public List<ServiceRecipientModel> ServiceRecipients { get; set; }

    public bool AllRecipientsSelected => ServiceRecipients.All(x => x.Selected);
}
