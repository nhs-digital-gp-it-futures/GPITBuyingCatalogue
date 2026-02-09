using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities
{
    public class SubLocationModel
    {
        public SubLocationModel()
        {
        }

        public SubLocationModel(string name, ServiceRecipientQuantityModel[] serviceRecipients)
        {
            Name = name;
            ServiceRecipients = serviceRecipients;
        }

        public string Name { get; set; }

        public ServiceRecipientQuantityModel[] ServiceRecipients { get; set; }

        public TaskProgress Status
        {
            get
            {
                if (ServiceRecipients.All(recipient => !string.IsNullOrWhiteSpace(recipient.InputQuantity)))
                    return TaskProgress.Completed;

                return ServiceRecipients.Any(recipient => !string.IsNullOrWhiteSpace(recipient.InputQuantity))
                    ? TaskProgress.InProgress
                    : TaskProgress.NotStarted;
            }
        }
    }
}
