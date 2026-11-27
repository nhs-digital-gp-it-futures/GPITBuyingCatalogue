using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions;

public static class SolutionSectionsServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionSectionsAsync_ReturnsExpectedSolutionSections(
        [Frozen] BuyingCatalogueDbContext context,
        SolutionSectionsService service)
    {
        var solutionSection1 = new SolutionSections
        {
            SolutionType = SolutionType.GPIT,
            Details = SolutionSectionRequirement.Mandatory,
            Description = SolutionSectionRequirement.Mandatory,
            Features = SolutionSectionRequirement.Optional,
            Integrations = SolutionSectionRequirement.Optional,
            Implementation = SolutionSectionRequirement.Optional,
            ApplicationType = SolutionSectionRequirement.Mandatory,
            HostingType = SolutionSectionRequirement.Optional,
            DataProcessingInformation = SolutionSectionRequirement.Optional,
            ListPrice = SolutionSectionRequirement.Mandatory,
            CapabilitiesAndEpics = SolutionSectionRequirement.Mandatory,
            AdditionalServices = SolutionSectionRequirement.Optional,
            AssociatedServices = SolutionSectionRequirement.Optional,
            Standards = SolutionSectionRequirement.Optional,
            DevelopmentPlans = SolutionSectionRequirement.Optional,
            SupplierDetails = SolutionSectionRequirement.Mandatory,
            ServiceLevelAgreement = SolutionSectionRequirement.Mandatory,
        };

        var solutionSection2 = new SolutionSections
        {
            SolutionType = SolutionType.CommunityPharmacy,
            Details = SolutionSectionRequirement.Mandatory,
            Description = SolutionSectionRequirement.Mandatory,
            Features = SolutionSectionRequirement.NotApplicable,
            Integrations = SolutionSectionRequirement.Mandatory,
            Implementation = SolutionSectionRequirement.NotApplicable,
            ApplicationType = SolutionSectionRequirement.Mandatory,
            HostingType = SolutionSectionRequirement.Mandatory,
            DataProcessingInformation = SolutionSectionRequirement.NotApplicable,
            ListPrice = SolutionSectionRequirement.Mandatory,
            CapabilitiesAndEpics = SolutionSectionRequirement.Mandatory,
            AdditionalServices = SolutionSectionRequirement.NotApplicable,
            AssociatedServices = SolutionSectionRequirement.NotApplicable,
            Standards = SolutionSectionRequirement.Mandatory,
            DevelopmentPlans = SolutionSectionRequirement.Mandatory,
            SupplierDetails = SolutionSectionRequirement.Mandatory,
            ServiceLevelAgreement = SolutionSectionRequirement.NotApplicable,
        };

        context.SolutionSections.Add(solutionSection1);
        context.SolutionSections.Add(solutionSection2);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetSolutionSectionsAsync(solutionSection2.SolutionType);

        result.Should().NotBeNull();
        result.SolutionType.Should().Be(solutionSection2.SolutionType);
        result.Details.Should().Be(solutionSection2.Details);
        result.Description.Should().Be(solutionSection2.Description);
        result.Features.Should().Be(solutionSection2.Features);
        result.Integrations.Should().Be(solutionSection2.Integrations);
        result.Implementation.Should().Be(solutionSection2.Implementation);
        result.ApplicationType.Should().Be(solutionSection2.ApplicationType);
        result.HostingType.Should().Be(solutionSection2.HostingType);
        result.DataProcessingInformation.Should().Be(solutionSection2.DataProcessingInformation);
        result.ListPrice.Should().Be(solutionSection2.ListPrice);
        result.CapabilitiesAndEpics.Should().Be(solutionSection2.CapabilitiesAndEpics);
        result.AdditionalServices.Should().Be(solutionSection2.AdditionalServices);
        result.AssociatedServices.Should().Be(solutionSection2.AssociatedServices);
        result.Standards.Should().Be(solutionSection2.Standards);
        result.DevelopmentPlans.Should().Be(solutionSection2.DevelopmentPlans);
        result.SupplierDetails.Should().Be(solutionSection2.SupplierDetails);
        result.ServiceLevelAgreement.Should().Be(solutionSection2.ServiceLevelAgreement);
    }
}
