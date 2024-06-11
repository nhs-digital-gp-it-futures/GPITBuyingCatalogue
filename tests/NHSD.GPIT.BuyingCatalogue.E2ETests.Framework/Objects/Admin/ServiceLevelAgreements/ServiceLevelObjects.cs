using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements
{
    public static class ServiceLevelObjects
    {
        public static By ServiceTypeInput => By.Id(nameof(AddEditServiceLevelModel.ServiceType));

        public static By ServiceTypeError => By.Id($"{nameof(AddEditServiceLevelModel.ServiceType)}-error");

        public static By ServiceLevelInput => By.Id(nameof(AddEditServiceLevelModel.ServiceLevel));

        public static By ServiceLevelError => By.Id($"{nameof(AddEditServiceLevelModel.ServiceLevel)}-error");

        public static By HowMeasuredInput => By.Id(nameof(AddEditServiceLevelModel.HowMeasured));

        public static By HowMeasuredError => By.Id($"{nameof(AddEditServiceLevelModel.HowMeasured)}-error");

        public static By CreditsRadioListInput => By.Id("add-edit-service-level");

        public static By CreditsRadioListError => By.Id("add-edit-service-level-error");

        public static By DeleteLink => By.LinkText("Delete service level");

        public static By AddServiceLevelLink => By.LinkText("Add service levels");

        public static By CancelLink => By.LinkText("Cancel");
    }
}
