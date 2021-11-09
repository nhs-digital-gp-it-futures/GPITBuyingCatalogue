using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements
{
    internal static class ServiceLevelObjects
    {
        internal static By ServiceTypeInput => By.Id(nameof(AddEditServiceLevelModel.ServiceType));

        internal static By ServiceTypeError => By.Id($"{nameof(AddEditServiceLevelModel.ServiceType)}-error");

        internal static By ServiceLevelInput => By.Id(nameof(AddEditServiceLevelModel.ServiceLevel));

        internal static By ServiceLevelError => By.Id($"{nameof(AddEditServiceLevelModel.ServiceLevel)}-error");

        internal static By HowMeasuredInput => By.Id(nameof(AddEditServiceLevelModel.HowMeasured));

        internal static By HowMeasuredError => By.Id($"{nameof(AddEditServiceLevelModel.HowMeasured)}-error");

        internal static By CreditsRadioListInput => By.Id("add-edit-service-level");

        internal static By CreditsRadioListError => By.Id("add-edit-service-level-error");

        internal static By DeleteLink => By.LinkText("Delete service level");

        internal static By CancelLink => By.LinkText("Cancel");
    }
}
