using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.SupplierDefinedEpics
{
    public static class EditSupplierDefinedEpicModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_FromActiveToInactive_WithReferencedItems(
            Epic epic,
            List<CatalogueItem> itemsReferencingEpic,
            EditSupplierDefinedEpicDetailsModel model,
            [Frozen] ISupplierDefinedEpicsService service,
            EditSupplierDefinedEpicModelValidator validator)
        {
            model.Id = epic.Id;
            model.IsActive = false;
            epic.IsActive = true;

            service.GetEpic(model.Id).Returns(epic);

            service.GetItemsReferencingEpic(model.Id).Returns(itemsReferencingEpic);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsActive)
                .WithErrorMessage("This supplier defined Epic cannot be set to inactive as it is referenced by another solution or service");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_FromActiveToInactive_NoReferencedItems(
            Epic epic,
            EditSupplierDefinedEpicDetailsModel model,
            [Frozen] ISupplierDefinedEpicsService service,
            EditSupplierDefinedEpicModelValidator validator)
        {
            model.Id = epic.Id;
            model.IsActive = false;
            epic.IsActive = true;

            service.GetEpic(model.Id).Returns(epic);

            service.GetItemsReferencingEpic(model.Id).Returns(Array.Empty<CatalogueItem>().ToList());

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.IsActive);
            service.Received().GetItemsReferencingEpic(model.Id);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_FromInactiveToInactive_NoModelError(
            Epic epic,
            EditSupplierDefinedEpicDetailsModel model,
            [Frozen] ISupplierDefinedEpicsService service,
            EditSupplierDefinedEpicModelValidator validator)
        {
            model.Id = epic.Id;
            model.IsActive = false;
            epic.IsActive = false;

            service.GetEpic(model.Id).Returns(epic);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.IsActive);
            service.DidNotReceive().GetItemsReferencingEpic(model.Id);
        }
    }
}
