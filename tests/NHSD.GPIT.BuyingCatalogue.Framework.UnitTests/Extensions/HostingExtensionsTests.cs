using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions;

public static class HostingExtensionsTests
{
    [Theory]
    [MemberAutoData(nameof(HasHostingTypesTestData))]
    public static void HasHostingType_Expected(
        Hosting hosting,
        HostingType hostingType,
        bool expectedOutcome)
    {
        var result = hosting.HasHostingType(hostingType);

        result.Should().Be(expectedOutcome);
    }

    public static IEnumerable<object[]> HasHostingTypesTestData => new[]
    {
        // Hybrid
        new object[] { new Hosting { HybridHostingType = null }, HostingType.Hybrid, false },
        new object[] { new Hosting { HybridHostingType = new HybridHostingType() }, HostingType.Hybrid, false },
        new object[]
        {
            new Hosting
            {
                HybridHostingType =
                    new HybridHostingType { Summary = "Summary", HostingModel = "Hosting Model" },
            },
            HostingType.Hybrid,
            true
        },
        // On-premise
        new object[] { new Hosting { OnPremise = null }, HostingType.OnPremise, false },
        new object[] { new Hosting { OnPremise = new OnPremise() }, HostingType.OnPremise, false },
        new object[]
        {
            new Hosting
            {
                OnPremise =
                    new OnPremise { Summary = "Summary", HostingModel = "Hosting Model" },
            },
            HostingType.OnPremise,
            true
        },
        // Private Cloud
        new object[] { new Hosting { PrivateCloud = null }, HostingType.PrivateCloud, false },
        new object[] { new Hosting { PrivateCloud = new PrivateCloud() }, HostingType.PrivateCloud, false },
        new object[]
        {
            new Hosting
            {
                PrivateCloud =
                    new PrivateCloud { Summary = "Summary", HostingModel = "Hosting Model" },
            },
            HostingType.PrivateCloud,
            true
        },
        // Public Cloud
        new object[] { new Hosting { PublicCloud = null }, HostingType.PublicCloud, false },
        new object[] { new Hosting { PublicCloud = new PublicCloud() }, HostingType.PublicCloud, false },
        new object[]
        {
            new Hosting
            {
                PublicCloud =
                    new PublicCloud { Summary = "Summary" },
            },
            HostingType.PublicCloud,
            true
        },
    };
}
