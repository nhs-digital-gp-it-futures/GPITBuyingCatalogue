using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.PublicationStatusValidation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditAssociatedServiceModelValidatorTests
    {
        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.Suspended)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static void Validate_UnpublishWithActiveSolutions_SetsModelError(
            PublicationStatus solutionsPublicationStatus,
            List<Solution> solutions,
            [Frozen] IAssociatedServicesService service,
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            solutions.ForEach(s => s.CatalogueItem.PublishedStatus = solutionsPublicationStatus);

            service.GetAllSolutionsForAssociatedService(model.AssociatedServiceId).Returns(solutions.Select(s => s.CatalogueItem).ToList());

            model.AssociatedServicePublicationStatus = PublicationStatus.Published;
            model.SelectedPublicationStatus = PublicationStatus.Unpublished;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage("This Associated Service cannot be unpublished as it is referenced by at least one solution");
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Draft)]
        [MockInlineAutoData(PublicationStatus.Unpublished)]
        public static void Validate_UnpublishWithInactiveSolutions_NoModelError(
            PublicationStatus solutionsPublicationStatus,
            List<Solution> solutions,
            [Frozen] IAssociatedServicesService service,
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            solutions.ForEach(s => s.CatalogueItem.PublishedStatus = solutionsPublicationStatus);

            service.GetAllSolutionsForAssociatedService(model.AssociatedServiceId).Returns(solutions.Select(s => s.CatalogueItem).ToList());

            model.AssociatedServicePublicationStatus = PublicationStatus.Published;
            model.SelectedPublicationStatus = PublicationStatus.Unpublished;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SelectedPublicationStatus);
        }
    }
}
