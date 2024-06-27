using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.Services.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.Services.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.PublishStatus;

public static class SolutionPublicationStatusServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(SolutionPublicationStatusService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task SetPublicationStatus_CallsPublicationStatusService(
        CatalogueItemId catalogueItemId,
        PublicationStatus publicationStatus,
        [Frozen] IPublicationStatusService publicationStatusService,
        SolutionPublicationStatusService service)
    {
        await service.SetPublicationStatus(catalogueItemId, publicationStatus);

        await publicationStatusService.Received().SetPublicationStatus(catalogueItemId, publicationStatus);
    }

    [Theory]
    [MockAutoData]
    public static async Task SetPublicationStatus_Unpublish_UnpublishesAdditionalServices(
        CatalogueItemId catalogueItemId,
        List<CatalogueItem> additionalServices,
        [Frozen] IPublicationStatusService publicationStatusService,
        [Frozen] IAdditionalServicesService additionalServicesService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        SolutionPublicationStatusService service)
    {
        additionalServicesService.GetAdditionalServicesBySolutionId(catalogueItemId, true).Returns(additionalServices);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(catalogueItemId, null).Returns(Enumerable.Empty<CatalogueItem>().ToList());

        await service.SetPublicationStatus(catalogueItemId, PublicationStatus.Unpublished);

        additionalServices.ForEach(
            x => publicationStatusService.Received().SetPublicationStatus(x.Id, PublicationStatus.Unpublished));
    }

    [Theory]
    [MockAutoData]
    public static async Task SetPublicationStatus_Unpublish_UnpublishesStaleAssociatedServices(
        CatalogueItemId catalogueItemId,
        List<CatalogueItem> associatedServices,
        [Frozen] IPublicationStatusService publicationStatusService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        [Frozen] IAdditionalServicesService additionalServicesService,
        SolutionPublicationStatusService service)
    {
        associatedServicesService.GetPublishedAssociatedServicesForSolution(catalogueItemId, null).Returns(associatedServices);

        associatedServicesService.GetAllSolutionsForAssociatedService(Arg.Any<CatalogueItemId>()).Returns(Enumerable.Empty<CatalogueItem>().ToList());

        additionalServicesService.GetAdditionalServicesBySolutionId(catalogueItemId, true).Returns(Enumerable.Empty<CatalogueItem>().ToList());

        await service.SetPublicationStatus(catalogueItemId, PublicationStatus.Unpublished);

        associatedServices.ForEach(
            x => publicationStatusService.Received().SetPublicationStatus(x.Id, PublicationStatus.Unpublished));
    }

    [Theory]
    [MockAutoData]
    public static async Task SetPublicationStatus_Unpublish_DoesNotUnpublishReferencedAssociatedServices(
        CatalogueItemId catalogueItemId,
        CatalogueItem referencedSolution,
        List<CatalogueItem> associatedServices,
        [Frozen] IPublicationStatusService publicationStatusService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        [Frozen] IAdditionalServicesService additionalServicesService,
        SolutionPublicationStatusService service)
    {
        var associatedService = associatedServices.First();

        referencedSolution.PublishedStatus = PublicationStatus.Published;

        associatedServicesService.GetPublishedAssociatedServicesForSolution(catalogueItemId, null).Returns(associatedServices);

        associatedServicesService.GetAllSolutionsForAssociatedService(associatedService.Id).Returns(new List<CatalogueItem> { referencedSolution });

        associatedServicesService.GetAllSolutionsForAssociatedService(Arg.Is<CatalogueItemId>(y => y != associatedService.Id)).Returns(Enumerable.Empty<CatalogueItem>().ToList());

        additionalServicesService.GetAdditionalServicesBySolutionId(catalogueItemId, true).Returns(Enumerable.Empty<CatalogueItem>().ToList());

        await service.SetPublicationStatus(catalogueItemId, PublicationStatus.Unpublished);

        await publicationStatusService.Received(0).SetPublicationStatus(associatedService.Id, Arg.Any<PublicationStatus>());

        associatedServices.Skip(1)
            .ToList()
            .ForEach(
                x => publicationStatusService.Received().SetPublicationStatus(x.Id, PublicationStatus.Unpublished));
    }
}
