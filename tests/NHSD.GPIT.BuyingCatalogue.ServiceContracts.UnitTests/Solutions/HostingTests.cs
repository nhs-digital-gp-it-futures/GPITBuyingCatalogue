using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class HostingTests
    {
        [Theory]
        [CommonInlineAutoData(HostingType.Hybrid)]
        [CommonInlineAutoData(HostingType.OnPremise)]
        [CommonInlineAutoData(HostingType.PrivateCloud)]
        [CommonInlineAutoData(HostingType.PublicCloud)]
        public static void HostingTypeStatus_ReturnsComplete(
            HostingType hostingType,
            Hosting hosting)
        {
            hosting.HostingTypeStatus(hostingType).Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void HostingTypeStatus_HybridNotStarted_ReturnsNotStarted(
            Hosting hosting)
        {
            hosting.HybridHostingType = null;

            hosting.HostingTypeStatus(HostingType.Hybrid).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void HostingTypeStatus_OnPremiseNotStarted_ReturnsNotStarted(
            Hosting hosting)
        {
            hosting.OnPremise = null;

            hosting.HostingTypeStatus(HostingType.OnPremise).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void HostingTypeStatus_PrivateCloudNotStarted_ReturnsNotStarted(
            Hosting hosting)
        {
            hosting.PrivateCloud = null;

            hosting.HostingTypeStatus(HostingType.PrivateCloud).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void HostingTypeStatus_PublicCloudNotStarted_ReturnsNotStarted(
            Hosting hosting)
        {
            hosting.PublicCloud = null;

            hosting.HostingTypeStatus(HostingType.PublicCloud).Should().Be(TaskProgress.NotStarted);
        }
    }
}
