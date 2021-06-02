using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PrivateCloudModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [TestCase(false)]
        [TestCase(true)]
        [TestCase(null)]
        public static void IsComplete_PrivateCloudNotNull_ReturnsIsValid(bool? expected)
        {
            var mockPrivateCloud = new Mock<PrivateCloud>();
            mockPrivateCloud.Setup(h => h.IsValid())
                .Returns(expected);
            var model = new PrivateCloudModel { PrivateCloud = mockPrivateCloud.Object, };

            var actual = model.IsComplete;

            mockPrivateCloud.Verify(h => h.IsValid());
            actual.Should().Be(expected);
        }

        [Test]
        public static void IsComplete_PrivateCloudIsNull_ReturnsNull()
        {
            var model = new PrivateCloudModel();
            model.PrivateCloud.Should().BeNull();

            var actual = model.IsComplete;

            actual.Should().BeNull();
        }

        [AutoData]
        [Test]
        public static void Get_RequiresHscnChecked_PrivateCloudHasValidRequiresHscn_ReturnsTrue(PrivateCloudModel model)
        {
            model.PrivateCloud.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_RequiresHscnChecked_PrivateCloudHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new PrivateCloudModel { PrivateCloud = new PrivateCloud { RequiresHscn = requiresHscn, }, };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnPrivateCloudRequiresHscn(
            PrivateCloudModel model)
        {
            model.RequiresHscnChecked = true;

            model.PrivateCloud.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnPrivateCloudRequiresHscn(
            PrivateCloudModel model)
        {
            model.RequiresHscnChecked = false;

            model.PrivateCloud.RequiresHscn.Should().BeNull();
        }
    }
}
