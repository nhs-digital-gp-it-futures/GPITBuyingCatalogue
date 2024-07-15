using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.PublicationStatusValidation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditAdditionalServiceModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_SamePublicationStatus_NoModelError(
            Solution solution,
            AdditionalService additionalService,
            EditAdditionalServiceModelValidator validator)
        {
            var model = new EditAdditionalServiceModel(solution.CatalogueItem, additionalService.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MissingDetails_SetsModelError(
            Solution solution,
            AdditionalService additionalService,
            EditAdditionalServiceModelValidator validator)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            additionalService.FullDescription = string.Empty;
            additionalService.CatalogueItem.Name = string.Empty;

            var model = new EditAdditionalServiceModel(solution.CatalogueItem, additionalService.CatalogueItem)
            {
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            model.DetailsStatus.Should().Be(TaskProgress.NotStarted);
            model.CapabilitiesStatus.Should().Be(TaskProgress.Completed);
            model.ListPriceStatus.Should().Be(TaskProgress.Completed);
            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MissingCapabilities_SetsModelError(
            Solution solution,
            AdditionalService additionalService,
            EditAdditionalServiceModelValidator validator)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            additionalService.CatalogueItem.CatalogueItemCapabilities = new HashSet<CatalogueItemCapability>();

            var model = new EditAdditionalServiceModel(solution.CatalogueItem, additionalService.CatalogueItem)
            {
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            model.DetailsStatus.Should().Be(TaskProgress.Completed);
            model.CapabilitiesStatus.Should().Be(TaskProgress.NotStarted);
            model.ListPriceStatus.Should().Be(TaskProgress.Completed);
            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }
    }
}
