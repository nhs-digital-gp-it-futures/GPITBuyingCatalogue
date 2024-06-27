using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements
{
    public static class ServiceAvailabilityTimesObjects
    {
        public static By SupportTypeInput => By.Id(nameof(EditServiceAvailabilityTimesModel.SupportType));

        public static By FromInput => By.Id(nameof(EditServiceAvailabilityTimesModel.From));

        public static By UntilInput => By.Id(nameof(EditServiceAvailabilityTimesModel.Until));

        public static By ApplicableDaysInput => By.Id(nameof(EditServiceAvailabilityTimesModel.ApplicableDays));

        public static By SupportTypeInputError => By.Id($"{nameof(EditServiceAvailabilityTimesModel.SupportType)}-error");

        public static By TimeInputError => By.Id("edit-service-availability-times-error");

        public static By ApplicableDaysInputError => By.Id($"{nameof(EditServiceAvailabilityTimesModel.ApplicableDays)}-error");

        public static By DeleteLink => By.LinkText("Delete service availability times");

        public static By AddAvailabilityTimesLink => By.LinkText("Add availability times");

        public static By CancelLink => By.LinkText("Cancel");
    }
}
