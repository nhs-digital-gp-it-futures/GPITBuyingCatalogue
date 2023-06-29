using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models;

public static class AdvancedTelephonyConstantsTests
{
    [Theory]
    [CommonAutoData]
    public static void TryGetFileMapping_InvalidFile_ReturnsFalse(
        string file) => AdvancedTelephonyConstants.TryGetFileMapping(file, out _).Should().BeFalse();

    [Theory]
    [CommonInlineAutoData(AdvancedTelephonyConstants.BabbleVoice)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.BuyersGuide)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.CheckCloud)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.DaisyPatientLine)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.LouisComm)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.Redcentric)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.RpmPatientContact)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.SurgeryConnect)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.ThinkHealthcare)]
    [CommonInlineAutoData(AdvancedTelephonyConstants.WaveNet)]
    public static void TryGetFileMapping_ValidFile_ReturnsTrue(
        string file) => AdvancedTelephonyConstants.TryGetFileMapping(file, out _).Should().BeTrue();
}
