using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PublicCloudModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [TestCase(false)]
        [TestCase(true)]
        [TestCase(null)]
        public static void IsComplete_PublicCloudNotNull_ReturnsIsValid(bool? expected)
        {
            var mockPublicCloud = new Mock<PublicCloud>();
            mockPublicCloud.Setup(h => h.IsValid())
                .Returns(expected);
            var model = new PublicCloudModel { PublicCloud = mockPublicCloud.Object, };

            var actual = model.IsComplete;

            mockPublicCloud.Verify(h => h.IsValid());
            actual.Should().Be(expected);
        }

        [Test]
        public static void IsComplete_PublicCloudIsNull_ReturnsNull()
        {
            var model = new PublicCloudModel();
            model.PublicCloud.Should().BeNull();

            var actual = model.IsComplete;

            actual.Should().BeNull();
        }

        [AutoData]
        [Test]
        public static void Get_RequiresHscnChecked_PublicCloudHasValidRequiresHscn_ReturnsTrue(PublicCloudModel model)
        {
            model.PublicCloud.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_RequiresHscnChecked_PublicCloudHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new PublicCloudModel { PublicCloud = new PublicCloud { RequiresHscn = requiresHscn, }, };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnPublicCloudRequiresHscn(
            PublicCloudModel model)
        {
            model.RequiresHscnChecked = true;

            model.PublicCloud.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnPublicCloudRequiresHscn(
            PublicCloudModel model)
        {
            model.RequiresHscnChecked = false;

            model.PublicCloud.RequiresHscn.Should().BeNull();
        }
    }
}
