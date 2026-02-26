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

        public SubLocationModel(SubLocationModel sublocationModel)
        {
            Name = sublocationModel.Name;
            OdsCode = sublocationModel.OdsCode;
            ServiceRecipients = sublocationModel.ServiceRecipients;
        }

        public SubLocationModel(
            string odsCode,
            string name,
            ServiceRecipientQuantityModel[] serviceRecipients)
            : this(name, serviceRecipients)
        {
            OdsCode = odsCode;
        }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public string ForwardingLink { get; set; }

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
