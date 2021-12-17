using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements
{
    internal static class SLAContactObjects
    {
        internal static By Channel => By.Id("Channel");

        internal static By ChannelError => By.Id("Channel-error");

        internal static By ContactInformation => By.Id("ContactInformation");

        internal static By ContactInformationError => By.Id("ContactInformation-error");

        internal static By ApplicableDays => By.Id("ApplicableDays");

        internal static By ApplicableDaysError => By.Id("ApplicableDays-error");

        internal static By From => By.Id("From");

        internal static By TimeInputError => By.Id("edit-sla-contact-error");

        internal static By Until => By.Id("Until");

        internal static By DeleteLink => By.LinkText("Delete contact");

        internal static By CancelLink => By.LinkText("Cancel");
    }
}
