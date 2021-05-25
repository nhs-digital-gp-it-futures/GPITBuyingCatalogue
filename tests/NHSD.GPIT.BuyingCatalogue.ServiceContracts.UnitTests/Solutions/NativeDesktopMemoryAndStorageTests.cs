using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeDesktopMemoryAndStorageTests
    {
        private static readonly List<string> InvalidStrings = new List<string>{ null, string.Empty, "    " };
        
        [AutoData]
        [Test]
        public static void IsValid_AllPropertiesValid_ReturnsTrue(
            NativeDesktopMemoryAndStorage model)
        {
            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [AutoData]
        [Test]
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

        [AutoData]
        [Test]
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

        [AutoData]
        [Test]
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
