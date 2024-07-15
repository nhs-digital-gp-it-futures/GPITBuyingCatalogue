using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteHostingTypeConfirmationModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_WhenPublishedSolutionHasMultipleHostingTypes_NoValidationErrors(
            Solution solution,
            DeleteHostingTypeConfirmationModel model,
            [Frozen] ISolutionsService solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;

            solutionsService.GetSolutionThin(model.SolutionId).Returns(catalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_WhenPublishedSolutionHasOneHostingType_SetsModelError(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            solution.Hosting.HybridHostingType = new HybridHostingType();
            solution.Hosting.OnPremise = new OnPremise();
            solution.Hosting.PrivateCloud = new PrivateCloud();

            var model = new DeleteHostingTypeConfirmationModel
            {
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
                HostingType = HostingType.PublicCloud,
            };

            solutionsService.GetSolutionThin(model.SolutionId).Returns(catalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m)
                .WithErrorMessage(DeleteHostingTypeConfirmationModelValidator.ErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_WhenUnpublishedSolutionHasMultipleHostingTypes_NoValidationErrors(
            Solution solution,
            DeleteHostingTypeConfirmationModel model,
            [Frozen] ISolutionsService solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            solutionsService.GetSolutionThin(model.SolutionId).Returns(catalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_WhenUnpublishedSolutionHasOneHostingType_NoValidationErrors(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            DeleteHostingTypeConfirmationModelValidator validator)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;
            solution.Hosting.HybridHostingType = new HybridHostingType();
            solution.Hosting.OnPremise = new OnPremise();
            solution.Hosting.PrivateCloud = new PrivateCloud();

            var model = new DeleteHostingTypeConfirmationModel
            {
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
                HostingType = HostingType.PublicCloud,
            };

            solutionsService.GetSolutionThin(model.SolutionId).Returns(catalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
