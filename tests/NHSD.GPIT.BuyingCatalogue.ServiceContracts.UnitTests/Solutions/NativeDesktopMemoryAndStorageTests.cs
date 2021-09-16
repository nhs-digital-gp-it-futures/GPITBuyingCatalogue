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
        public static void Status_AllPropertiesValid_ReturnsCompleted(
            NativeDesktopMemoryAndStorage model)
        {
            var actual = model.Status();

            actual.Should().Be(Enums.TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void Status_MinimumCpuNotValid_ReturnsNotStarted(
            NativeDesktopMemoryAndStorage model)
        {
            InvalidStrings.ForEach(
                invalid =>
                {
                    model.MinimumCpu = invalid;

                    var actual = model.Status();

                    actual.Should().Be(Enums.TaskProgress.NotStarted);
                });
        }

        [Theory]
        [AutoData]
        public static void Status_MinimumMemoryRequirementNotValid_ReturnsNotStarted(
            NativeDesktopMemoryAndStorage model)
        {
            InvalidStrings.ForEach(
                invalid =>
                {
                    model.MinimumMemoryRequirement = invalid;

                    var actual = model.Status();

                    actual.Should().Be(Enums.TaskProgress.NotStarted);
                });
        }

        [Theory]
        [AutoData]
        public static void Status_StorageRequirementsDescriptionNotValid_ReturnsNotStarted(
            NativeDesktopMemoryAndStorage model)
        {
            InvalidStrings.ForEach(
                invalid =>
                {
                    model.StorageRequirementsDescription = invalid;

                    var actual = model.Status();

                    actual.Should().Be(Enums.TaskProgress.NotStarted);
                });
        }
    }
}
