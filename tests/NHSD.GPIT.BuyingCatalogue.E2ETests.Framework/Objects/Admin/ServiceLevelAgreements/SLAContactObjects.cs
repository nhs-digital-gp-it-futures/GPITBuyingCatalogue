using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements
{
    public static class SLAContactObjects
    {
        public static By Channel => By.Id("Channel");

        public static By ChannelError => By.Id("Channel-error");

        public static By ContactInformation => By.Id("ContactInformation");

        public static By ContactInformationError => By.Id("ContactInformation-error");

        public static By ApplicableDays => By.Id("ApplicableDays");

        public static By ApplicableDaysError => By.Id("ApplicableDays-error");

        public static By From => By.Id("From");

        public static By TimeInputError => By.Id("edit-sla-contact-error");

        public static By Until => By.Id("Until");

        public static By DeleteLink => By.LinkText("Delete contact");

        public static By CancelLink => By.LinkText("Cancel");

        public static By AddContactLevelDetailsLink => By.LinkText("Add contact details");
    }
}
