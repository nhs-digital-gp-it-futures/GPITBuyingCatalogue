using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ApplicationTypeTests
    {
        [Theory]
        [CommonInlineAutoData(ApplicationType.BrowserBased)]
        [CommonInlineAutoData(ApplicationType.MobileTablet)]
        [CommonInlineAutoData(ApplicationType.Desktop)]
        public static void ApplicationTypes_IsUpdatedCorrectly(
            ApplicationType applicationType,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.EnsureApplicationTypePresent(applicationType);

            applicationTypeDetail.ApplicaitonTypes.Should().Contain(applicationType.AsString(EnumFormat.EnumMemberValue));
        }

        [Theory]
        [CommonAutoData]
        public static void HasApplicationType_True(
            ApplicationTypeDetail application)
        {
            application.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            application.HasApplicationType(ApplicationType.BrowserBased).Should().BeTrue();
        }

        [Fact]
        public static void HasApplicationType_False()
        {
            var applicationTypeDetail = new ApplicationTypeDetail();

            applicationTypeDetail.HasApplicationType(ApplicationType.BrowserBased).Should().BeFalse();
        }
    }
}
