﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.SupplierDefinedEpics
{
    public static class EditSupplierDefinedEpicModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task Validate_FromActiveToInactive_WithReferencedItems(
            Epic epic,
            List<CatalogueItem> itemsReferencingEpic,
            EditSupplierDefinedEpicModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            EditSupplierDefinedEpicModelValidator validator)
        {
            model.Id = epic.Id;
            model.IsActive = false;
            epic.IsActive = true;

            service.Setup(s => s.GetEpic(model.Id))
                .ReturnsAsync(epic);

            service.Setup(s => s.GetItemsReferencingEpic(model.Id))
                .ReturnsAsync(itemsReferencingEpic);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.IsActive)
                .WithErrorMessage("This supplier defined Epic cannot be set to inactive as it is referenced by another solution or service");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_FromActiveToInactive_NoReferencedItems(
            Epic epic,
            EditSupplierDefinedEpicModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            EditSupplierDefinedEpicModelValidator validator)
        {
            model.Id = epic.Id;
            model.IsActive = false;
            epic.IsActive = true;

            service.Setup(s => s.GetEpic(model.Id))
                .ReturnsAsync(epic);

            service.Setup(s => s.GetItemsReferencingEpic(model.Id))
                .ReturnsAsync(Array.Empty<CatalogueItem>().ToList());

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.IsActive);
            service.Verify(s => s.GetItemsReferencingEpic(model.Id), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_FromInactiveToInactive_NoModelError(
            Epic epic,
            EditSupplierDefinedEpicModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            EditSupplierDefinedEpicModelValidator validator)
        {
            model.Id = epic.Id;
            model.IsActive = false;
            epic.IsActive = false;

            service.Setup(s => s.GetEpic(model.Id))
                .ReturnsAsync(epic);

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.IsActive);
            service.Verify(s => s.GetItemsReferencingEpic(model.Id), Times.Never());
        }
    }
}
