using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount
{
    public class ManageEmailPreferencesModel : YourAccountBaseModel
    {
        public override int Index => 1;

        public bool Saved { get; set; }

        public List<UserEmailPreferenceModel> EmailPreferences { get; set; }

        public string GetLabel(EmailPreferenceTypeEnum emailPreferenceType) => emailPreferenceType switch
        {
            EmailPreferenceTypeEnum.ContractExpiry => "Contract expiring notifications",
            _ => throw new InvalidOperationException($"Unhandled email preference type {emailPreferenceType}"),
        };

        public string GetHint(EmailPreferenceTypeEnum emailPreferenceType) => emailPreferenceType switch
        {
            EmailPreferenceTypeEnum.ContractExpiry => "You'll be sent email reminders that your contract is approaching its end date.",
            _ => throw new InvalidOperationException($"Unhandled email preference type {emailPreferenceType}"),
        };
    }
}
