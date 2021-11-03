using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements
{
    internal static class ServiceAvailabilityTimesObjects
    {
        internal static By SupportTypeInput => By.Id(nameof(EditServiceAvailabilityTimesModel.SupportType));

        internal static By FromInput => By.Id(nameof(EditServiceAvailabilityTimesModel.From));

        internal static By UntilInput => By.Id(nameof(EditServiceAvailabilityTimesModel.Until));

        internal static By ApplicableDaysInput => By.Id(nameof(EditServiceAvailabilityTimesModel.ApplicableDays));

        internal static By SupportTypeInputError => By.Id($"{nameof(EditServiceAvailabilityTimesModel.SupportType)}-error");

        internal static By TimeInputError => By.Id("edit-service-availability-times-error");

        internal static By ApplicableDaysInputError => By.Id($"{nameof(EditServiceAvailabilityTimesModel.ApplicableDays)}-error");

        internal static By DeleteLink => By.LinkText("Delete service availability times");

        internal static By CancelLink => By.LinkText("Cancel");
    }
}
