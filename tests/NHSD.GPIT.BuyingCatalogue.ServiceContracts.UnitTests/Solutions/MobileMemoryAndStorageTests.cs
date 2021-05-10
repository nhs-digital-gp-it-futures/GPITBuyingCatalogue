using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class MobileMemoryAndStorageTests
    {
        private static readonly Fixture Fixture = new();
        private static string[] InvalidStrings = { null, string.Empty, "    " };

        [Test, AutoData]
        public static void IsValid_BothPropertiesValid_ReturnsTrue(MobileMemoryAndStorage mobileMemoryAndStorage)
        {
            mobileMemoryAndStorage.Description.Should().NotBeNullOrWhiteSpace();
            mobileMemoryAndStorage.MinimumMemoryRequirement.Should().NotBeNullOrWhiteSpace();
            
            mobileMemoryAndStorage.IsValid().Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsValid_DescriptionIsInvalid_ReturnsFalse(string invalid)
        {
            var mobileMemoryAndStorage = Fixture.Build<MobileMemoryAndStorage>().Without(m => m.Description).Create();
            mobileMemoryAndStorage.Description = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeFalse();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsValid_MinimumMemoryRequirementIsInvalid_ReturnsFalse(string invalid)
        {
            var mobileMemoryAndStorage =
                Fixture.Build<MobileMemoryAndStorage>().Without(m => m.MinimumMemoryRequirement).Create();
            mobileMemoryAndStorage.MinimumMemoryRequirement = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeFalse();
        }
    }
}