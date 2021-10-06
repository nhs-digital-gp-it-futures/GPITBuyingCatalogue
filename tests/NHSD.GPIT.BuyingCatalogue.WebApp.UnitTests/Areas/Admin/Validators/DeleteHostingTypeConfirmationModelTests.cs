using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteHostingTypeConfirmationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task Validate_WhenPublishedSolutionHasMultipleHostingTypes_NoValidationErrors(
            CatalogueItem catalogueItem,
            DeleteHostingTypeConfirmationModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Published;

            solutionsService.Setup(s => s.GetSolution(model.SolutionId))
                .ReturnsAsync(catalogueItem);

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_WhenPublishedSolutionHasOneHostingType_SetsModelError(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            catalogueItem.Solution.Hosting.HybridHostingType = new();
            catalogueItem.Solution.Hosting.OnPremise = new();
            catalogueItem.Solution.Hosting.PrivateCloud = new();

            var model = new DeleteHostingTypeConfirmationModel
            {
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
                HostingType = HostingType.PublicCloud,
            };

            solutionsService.Setup(s => s.GetSolution(model.SolutionId))
                .ReturnsAsync(catalogueItem);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m)
                .WithErrorMessage(DeleteHostingTypeConfirmationModelValidator.ErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_WhenUnpublishedSolutionHasMultipleHostingTypes_NoValidationErrors(
            CatalogueItem catalogueItem,
            DeleteHostingTypeConfirmationModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            solutionsService.Setup(s => s.GetSolution(model.SolutionId))
                .ReturnsAsync(catalogueItem);

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_WhenUnpublishedSolutionHasOneHostingType_NoValidationErrors(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;
            catalogueItem.Solution.Hosting.HybridHostingType = new();
            catalogueItem.Solution.Hosting.OnPremise = new();
            catalogueItem.Solution.Hosting.PrivateCloud = new();

            var model = new DeleteHostingTypeConfirmationModel
            {
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
                HostingType = HostingType.PublicCloud,
            };

            solutionsService.Setup(s => s.GetSolution(model.SolutionId))
                .ReturnsAsync(catalogueItem);

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
