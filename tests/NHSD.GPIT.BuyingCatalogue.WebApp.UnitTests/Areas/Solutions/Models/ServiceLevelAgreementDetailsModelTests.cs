using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ServiceLevelAgreementDetailsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructing_NullSolution(
            Solution solution)
            => Assert.Throws<ArgumentNullException>(() => new ServiceLevelAgreementDetailsModel(
                null,
                solution.ServiceLevelAgreement.ServiceHours,
                solution.ServiceLevelAgreement.Contacts,
                solution.ServiceLevelAgreement.ServiceLevels));

        [Theory]
        [CommonAutoData]
        public static void Constructing_NullServiceAvailabilityTimes(
            Solution solution)
            => Assert.Throws<ArgumentNullException>(() => new ServiceLevelAgreementDetailsModel(
                solution.CatalogueItem,
                null,
                solution.ServiceLevelAgreement.Contacts,
                solution.ServiceLevelAgreement.ServiceLevels));

        [Theory]
        [CommonAutoData]
        public static void Constructing_NullSlaContacts(
            Solution solution)
            => Assert.Throws<ArgumentNullException>(() => new ServiceLevelAgreementDetailsModel(
                solution.CatalogueItem,
                solution.ServiceLevelAgreement.ServiceHours,
                null,
                solution.ServiceLevelAgreement.ServiceLevels));

        [Theory]
        [CommonAutoData]
        public static void Constructing_NullServiceLevels(
            Solution solution)
            => Assert.Throws<ArgumentNullException>(() => new ServiceLevelAgreementDetailsModel(
                solution.CatalogueItem,
                solution.ServiceLevelAgreement.ServiceHours,
                solution.ServiceLevelAgreement.Contacts,
                null));
    }
}
