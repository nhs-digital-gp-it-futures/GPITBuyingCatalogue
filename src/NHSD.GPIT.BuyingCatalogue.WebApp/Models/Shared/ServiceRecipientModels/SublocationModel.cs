using System.Collections.Generic;

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
}
