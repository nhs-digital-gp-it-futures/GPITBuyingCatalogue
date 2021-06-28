using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class NativeDesktopMemoryAndStorageTests
    {
        private static readonly List<string> InvalidStrings = new() { null, string.Empty, "    " };

        [Theory]
        [AutoData]
        public static void IsValid_AllPropertiesValid_ReturnsTrue(
            NativeDesktopMemoryAndStorage model)
        {
            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public static void IsValid_MinimumCpuNotValid_ReturnsFalse(
            NativeDesktopMemoryAndStorage model)
        {
            InvalidStrings.ForEach(
                invalid =>
                {
                    model.MinimumCpu = invalid;

                    var actual = model.IsValid();

                    actual.Should().BeFalse();
                });
        }

        [Theory]
        [AutoData]
        public static void IsValid_MinimumMemoryRequirementNotValid_ReturnsFalse(
            NativeDesktopMemoryAndStorage model)
        {
            InvalidStrings.ForEach(
                invalid =>
                {
                    model.MinimumMemoryRequirement = invalid;

                    var actual = model.IsValid();

                    actual.Should().BeFalse();
                });
        }

        [Theory]
        [AutoData]
        public static void IsValid_StorageRequirementsDescriptionNotValid_ReturnsFalse(
            NativeDesktopMemoryAndStorage model)
        {
            InvalidStrings.ForEach(
                invalid =>
                {
                    model.StorageRequirementsDescription = invalid;

                    var actual = model.IsValid();

                    actual.Should().BeFalse();
                });
        }
    }
}
