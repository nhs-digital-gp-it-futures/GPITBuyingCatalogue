using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.Services.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.PublishStatus;

public static class SolutionPublicationStatusServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(SolutionPublicationStatusService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task SetPublicationStatus_CallsPublicationStatusService(
        CatalogueItemId catalogueItemId,
        PublicationStatus publicationStatus,
        [Frozen] Mock<IPublicationStatusService> publicationStatusService,
        SolutionPublicationStatusService service)
    {
        await service.SetPublicationStatus(catalogueItemId, publicationStatus);

        publicationStatusService.Verify(x => x.SetPublicationStatus(catalogueItemId, publicationStatus));
    }

    [Theory]
    [CommonAutoData]
    public static async Task SetPublicationStatus_Unpublish_UnpublishesAdditionalServices(
        CatalogueItemId catalogueItemId,
        List<CatalogueItem> additionalServices,
        [Frozen] Mock<IPublicationStatusService> publicationStatusService,
        [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
        SolutionPublicationStatusService service)
    {
        additionalServicesService
            .Setup(x => x.GetAdditionalServicesBySolutionId(catalogueItemId, true))
            .ReturnsAsync(additionalServices);

        await service.SetPublicationStatus(catalogueItemId, PublicationStatus.Unpublished);

        additionalServices.ForEach(
            x => publicationStatusService.Verify(y => y.SetPublicationStatus(x.Id, PublicationStatus.Unpublished)));
    }

    [Theory]
    [CommonAutoData]
    public static async Task SetPublicationStatus_Unpublish_UnpublishesStaleAssociatedServices(
        CatalogueItemId catalogueItemId,
        List<CatalogueItem> associatedServices,
        [Frozen] Mock<IPublicationStatusService> publicationStatusService,
        [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
        SolutionPublicationStatusService service)
    {
        associatedServicesService
            .Setup(x => x.GetPublishedAssociatedServicesForSolution(catalogueItemId))
            .ReturnsAsync(associatedServices);

        associatedServicesService
            .Setup(x => x.GetAllSolutionsForAssociatedService(It.IsAny<CatalogueItemId>()))
            .ReturnsAsync(Enumerable.Empty<CatalogueItem>().ToList());

        await service.SetPublicationStatus(catalogueItemId, PublicationStatus.Unpublished);

        associatedServices.ForEach(
            x => publicationStatusService.Verify(y => y.SetPublicationStatus(x.Id, PublicationStatus.Unpublished)));
    }

    [Theory]
    [CommonAutoData]
    public static async Task SetPublicationStatus_Unpublish_DoesNotUnpublishReferencedAssociatedServices(
        CatalogueItemId catalogueItemId,
        CatalogueItem referencedSolution,
        List<CatalogueItem> associatedServices,
        [Frozen] Mock<IPublicationStatusService> publicationStatusService,
        [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
        SolutionPublicationStatusService service)
    {
        var associatedService = associatedServices.First();

        referencedSolution.PublishedStatus = PublicationStatus.Published;

        associatedServicesService
            .Setup(x => x.GetPublishedAssociatedServicesForSolution(catalogueItemId))
            .ReturnsAsync(associatedServices);

        associatedServicesService
            .Setup(x => x.GetAllSolutionsForAssociatedService(associatedService.Id))
            .ReturnsAsync(new List<CatalogueItem> { referencedSolution });

        associatedServicesService
            .Setup(x => x.GetAllSolutionsForAssociatedService(It.Is<CatalogueItemId>(y => y != associatedService.Id)))
            .ReturnsAsync(Enumerable.Empty<CatalogueItem>().ToList());

        await service.SetPublicationStatus(catalogueItemId, PublicationStatus.Unpublished);

        publicationStatusService.Verify(
            x => x.SetPublicationStatus(associatedService.Id, It.IsAny<PublicationStatus>()),
            Times.Never());

        associatedServices.Skip(1)
            .ToList()
            .ForEach(
                x => publicationStatusService.Verify(y => y.SetPublicationStatus(x.Id, PublicationStatus.Unpublished)));
    }
}
