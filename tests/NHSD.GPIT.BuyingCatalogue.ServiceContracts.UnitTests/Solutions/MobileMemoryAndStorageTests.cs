using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class MobileMemoryAndStorageTests
    {
        private static readonly Fixture Fixture = new();

        [Theory]
        [AutoData]
        public static void Status_BothPropertiesValid_ReturnsCompleted(MobileMemoryAndStorage mobileMemoryAndStorage)
        {
            mobileMemoryAndStorage.Description.Should().NotBeNullOrWhiteSpace();
            mobileMemoryAndStorage.MinimumMemoryRequirement.Should().NotBeNullOrWhiteSpace();

            mobileMemoryAndStorage.Status().Should().Be(Enums.TaskProgress.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_DescriptionIsInvalid_ReturnsNotStarted(string invalid)
        {
            var mobileMemoryAndStorage = Fixture.Build<MobileMemoryAndStorage>().Without(m => m.Description).Create();
            mobileMemoryAndStorage.Description = invalid;

            mobileMemoryAndStorage.Status().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_MinimumMemoryRequirementIsInvalid_ReturnsNotStarted(string invalid)
        {
            var mobileMemoryAndStorage =
                Fixture.Build<MobileMemoryAndStorage>().Without(m => m.MinimumMemoryRequirement).Create();
            mobileMemoryAndStorage.MinimumMemoryRequirement = invalid;

            mobileMemoryAndStorage.Status().Should().Be(Enums.TaskProgress.NotStarted);
        }
    }
}
