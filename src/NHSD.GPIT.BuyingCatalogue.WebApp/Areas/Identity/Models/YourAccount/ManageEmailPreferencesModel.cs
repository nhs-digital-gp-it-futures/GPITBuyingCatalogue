using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount
{
    public class ManageEmailPreferencesModel : YourAccountBaseModel
    {
        public const string ContractExpiryLabel = "Contract expiring notifications";
        public const string ContractExpiryHint = "You'll be sent email reminders that your contract is approaching its end date.";

        public override int Index => 1;

        public bool Saved { get; set; }

        public List<UserEmailPreferenceModel> EmailPreferences { get; set; }

        public string GetLabel(EmailPreferenceTypeEnum emailPreferenceType) => emailPreferenceType switch
        {
            EmailPreferenceTypeEnum.ContractExpiry => ContractExpiryLabel,
            _ => throw new InvalidOperationException($"Unhandled email preference type {emailPreferenceType}"),
        };

        public string GetHint(EmailPreferenceTypeEnum emailPreferenceType) => emailPreferenceType switch
        {
            EmailPreferenceTypeEnum.ContractExpiry => ContractExpiryHint,
            _ => throw new InvalidOperationException($"Unhandled email preference type {emailPreferenceType}"),
        };
    }
}
