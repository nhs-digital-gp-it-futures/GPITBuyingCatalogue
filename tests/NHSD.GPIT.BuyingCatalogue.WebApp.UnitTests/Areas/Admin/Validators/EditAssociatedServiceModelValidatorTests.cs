using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.PublicationStatusValidation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditAssociatedServiceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SamePublicationStatus_NoModelError(
            Solution solution,
            AssociatedService associatedService,
            EditAssociatedServiceModelValidator validator)
        {
            var model = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MissingDetails_SetsModelError(
            Solution solution,
            AssociatedService associatedService,
            EditAssociatedServiceModelValidator validator)
        {
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            associatedService.Description = string.Empty;
            associatedService.CatalogueItem.Name = string.Empty;

            var model = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem)
            {
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            model.DetailsStatus.Should().Be(TaskProgress.NotStarted);
            model.ListPriceStatus.Should().Be(TaskProgress.Completed);
            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MissingListPrices_SetsModelError(
            Solution solution,
            AssociatedService associatedService,
            EditAssociatedServiceModelValidator validator)
        {
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            associatedService.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>();

            var model = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem)
            {
                SelectedPublicationStatus = PublicationStatus.Published,
            };

            var result = validator.TestValidate(model);

            model.DetailsStatus.Should().Be(TaskProgress.Completed);
            model.ListPriceStatus.Should().Be(TaskProgress.NotStarted);
            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage("Complete all mandatory sections before publishing");
        }
    }
}
