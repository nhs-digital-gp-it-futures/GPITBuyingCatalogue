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
    internal static class OnPremiseModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [TestCase(false)]
        [TestCase(true)]
        [TestCase(null)]
        public static void IsComplete_OnPremiseNotNull_ReturnsIsValid(bool? expected)
        {
            var mockOnPremise = new Mock<OnPremise>();
            mockOnPremise.Setup(h => h.IsValid())
                .Returns(expected);
            var model = new OnPremiseModel { OnPremise = mockOnPremise.Object, };

            var actual = model.IsComplete;

            mockOnPremise.Verify(h => h.IsValid());
            actual.Should().Be(expected);
        }

        [Test]
        public static void IsComplete_OnPremiseIsNull_ReturnsNull()
        {
            var model = new OnPremiseModel();
            model.OnPremise.Should().BeNull();

            var actual = model.IsComplete;

            actual.Should().BeNull();
        }

        [AutoData]
        [Test]
        public static void Get_RequiresHscnChecked_OnPremiseHasValidRequiresHscn_ReturnsTrue(OnPremiseModel model)
        {
            model.OnPremise.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_RequiresHscnChecked_OnPremiseHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new OnPremiseModel { OnPremise = new OnPremise { RequiresHscn = requiresHscn, }, };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnOnPremiseRequiresHscn(
            OnPremiseModel model)
        {
            model.RequiresHscnChecked = true;

            model.OnPremise.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnOnPremiseRequiresHscn(
            OnPremiseModel model)
        {
            model.RequiresHscnChecked = false;

            model.OnPremise.RequiresHscn.Should().BeNull();
        }
    }
}
