using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CatalogueItemTypeTests
    {
        [TestCase(CatalogueItemType.AdditionalService, "Additional Service")]
        [TestCase(CatalogueItemType.AssociatedService, "Associated Service")]
        [TestCase(CatalogueItemType.Solution, "Catalogue Solution")]
        public static void DisplayName_ReturnsExpectedName(CatalogueItemType itemType, string expectedName)
        {
            var displayName = itemType.DisplayName();

            displayName.Should().Be(expectedName);
        }

        [Test]
        public static void MarkOrderSectionAsViewed_NullOrder_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => CatalogueItemType.AdditionalService.MarkOrderSectionAsViewed(null));
        }

        [Test]
        public static void MarkOrderSectionAsViewed_InvalidType_ThrowsException()
        {
            var order = new Order();

            Assert.Throws<ArgumentException>(() => CatalogueItemTypeExtensions.MarkOrderSectionAsViewed(0, order));
        }

        [TestCaseSource(nameof(MarkOrderSectionAsViewedTestCases))]
        public static void MarkOrderSectionAsViewed_MarksExpectedSection(
            CatalogueItemType itemType,
            Func<Order, bool> sectionViewed)
        {
            var order = new Order();
            sectionViewed(order).Should().BeFalse();

            itemType.MarkOrderSectionAsViewed(order);

            sectionViewed(order).Should().BeTrue();
        }

        [TestCaseSource(nameof(InferEstimationPeriodTestCases))]
        public static void InferEstimationPeriod_ReturnsExpectedEstimationPeriod(
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType,
            TimeUnit? estimationPeriod,
            TimeUnit? expectedEstimationPeriod)
        {
            var actual = catalogueItemType.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(expectedEstimationPeriod);
        }

        [Test]
        [AutoData]
        public static void InferEstimationPeriod_InvalidCatalogueItemType_ThrowsException(TimeUnit timeUnit)
        {
            const CatalogueItemType itemType = 0;

            Assert.Throws<ArgumentOutOfRangeException>(() => itemType.InferEstimationPeriod(0, timeUnit));
        }

        private static IEnumerable<ITestCaseData> MarkOrderSectionAsViewedTestCases()
        {
            yield return new TestCaseData(CatalogueItemType.AdditionalService, new Func<Order, bool>(o => o.Progress.AdditionalServicesViewed));
            yield return new TestCaseData(CatalogueItemType.AssociatedService, new Func<Order, bool>(o => o.Progress.AssociatedServicesViewed));
            yield return new TestCaseData(CatalogueItemType.Solution, new Func<Order, bool>(o => o.Progress.CatalogueSolutionsViewed));
        }

        private static IEnumerable<ITestCaseData> InferEstimationPeriodTestCases()
        {
            foreach (var data in AdditionalServiceSolutionData(CatalogueItemType.AdditionalService))
                yield return new TestCaseData(data);

            foreach (var data in AdditionalServiceSolutionData(CatalogueItemType.Solution))
                yield return new TestCaseData(data);

            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, null, null);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, TimeUnit.PerYear, TimeUnit.PerYear);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, TimeUnit.PerMonth, TimeUnit.PerMonth);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, null, null);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, TimeUnit.PerYear, null);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, TimeUnit.PerMonth, null);
        }

        private static IEnumerable<object[]> AdditionalServiceSolutionData(CatalogueItemType itemType)
        {
            yield return new object[] { itemType, ProvisioningType.OnDemand, null, null };
            yield return new object[] { itemType, ProvisioningType.OnDemand, TimeUnit.PerMonth, TimeUnit.PerMonth };
            yield return new object[] { itemType, ProvisioningType.OnDemand, TimeUnit.PerYear, TimeUnit.PerYear };

            yield return new object[] { itemType, ProvisioningType.Patient, null, TimeUnit.PerMonth };
            yield return new object[] { itemType, ProvisioningType.Patient, TimeUnit.PerMonth, TimeUnit.PerMonth };
            yield return new object[] { itemType, ProvisioningType.Patient, TimeUnit.PerYear, TimeUnit.PerMonth };

            yield return new object[] { itemType, ProvisioningType.Declarative, null, TimeUnit.PerYear };
            yield return new object[] { itemType, ProvisioningType.Declarative, TimeUnit.PerMonth, TimeUnit.PerYear };
            yield return new object[] { itemType, ProvisioningType.Declarative, TimeUnit.PerYear, TimeUnit.PerYear };
        }
    }
}
