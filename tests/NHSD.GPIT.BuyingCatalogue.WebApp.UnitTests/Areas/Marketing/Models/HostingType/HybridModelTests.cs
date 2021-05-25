using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HybridModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };
        
        [TestCase(false)]
        [TestCase(true)]
        [TestCase(null)]
        public static void IsComplete_HybridHostingTypeNotNull_ReturnsIsValid(bool? expected)
        {
            var mockHybridHostingType = new Mock<HybridHostingType>();
            mockHybridHostingType.Setup(h => h.IsValid())
                .Returns(expected);
            var model = new HybridModel { HybridHostingType = mockHybridHostingType.Object, };

            var actual = model.IsComplete;
            
            mockHybridHostingType.Verify(h => h.IsValid());
            actual.Should().Be(expected);
        }

        [Test]
        public static void IsComplete_HybridHostingTypeIsNull_ReturnsNull()
        {
            var model = new HybridModel();
            model.HybridHostingType.Should().BeNull();

            var actual = model.IsComplete;
            
            actual.Should().BeNull();
        }

        [AutoData]
        [Test]
        public static void Get_RequiresHscnChecked_HybridHostingTypeHasValidRequiresHscn_ReturnsTrue(HybridModel model)
        {
            model.HybridHostingType.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_RequiresHscnChecked_HybridHostingTypeHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new HybridModel { HybridHostingType = new HybridHostingType { RequiresHscn = requiresHscn, }, };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnHybridHostingTypeRequiresHscn(
            HybridModel model)
        {
            model.RequiresHscnChecked = true;

            model.HybridHostingType.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [AutoData]
        [Test]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnHybridHostingTypeRequiresHscn(
            HybridModel model)
        {
            model.RequiresHscnChecked = false;

            model.HybridHostingType.RequiresHscn.Should().BeNull();
        }
    }
}
