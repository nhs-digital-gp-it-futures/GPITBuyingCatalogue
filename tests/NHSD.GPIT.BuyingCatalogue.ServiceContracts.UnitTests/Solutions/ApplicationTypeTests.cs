using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ApplicationTypeTests
    {
        [Theory]
        [MockInlineAutoData(ApplicationType.BrowserBased)]
        [MockInlineAutoData(ApplicationType.MobileTablet)]
        [MockInlineAutoData(ApplicationType.Desktop)]
        public static void ApplicationTypes_IsUpdatedCorrectly(
            ApplicationType applicationType,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.EnsureApplicationTypePresent(applicationType);

            applicationTypeDetail.ApplicationTypes.Should().Contain(applicationType.AsString(EnumFormat.EnumMemberValue));
        }

        [Theory]
        [MockAutoData]
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
