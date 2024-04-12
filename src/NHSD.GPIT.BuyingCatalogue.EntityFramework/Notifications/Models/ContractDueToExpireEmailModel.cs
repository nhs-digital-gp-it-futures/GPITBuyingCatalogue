using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    public class ContractDueToExpireEmailModel() : EmailModel(EmailNotificationTypeEnum.ContractDueToExpire) 
    {
        public string CallOffId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DaysRemaining { get; set; }
    }
}
