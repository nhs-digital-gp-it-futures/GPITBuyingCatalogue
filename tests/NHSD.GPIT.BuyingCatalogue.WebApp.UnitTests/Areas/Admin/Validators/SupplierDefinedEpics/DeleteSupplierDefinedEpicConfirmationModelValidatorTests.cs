using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.SupplierDefinedEpics
{
    public static class DeleteSupplierDefinedEpicConfirmationModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_ActiveNoReferences_SetsModelError(
            Epic epic,
            DeleteSupplierDefinedEpicConfirmationModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            DeleteSupplierDefinedEpicConfirmationModelValidator validator)
        {
            model.Id = epic.Id;
            model.Name = epic.Name;

            epic.IsActive = true;
            epic.SupplierDefined = true;

            service.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            service.Setup(s => s.GetItemsReferencingEpic(epic.Id))
                .ReturnsAsync(Array.Empty<CatalogueItem>().ToList());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ActiveWithReferences_SetsModelError(
            Epic epic,
            List<CatalogueItem> items,
            DeleteSupplierDefinedEpicConfirmationModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            DeleteSupplierDefinedEpicConfirmationModelValidator validator)
        {
            model.Id = epic.Id;
            model.Name = epic.Name;

            epic.IsActive = true;
            epic.SupplierDefined = true;

            service.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            service.Setup(s => s.GetItemsReferencingEpic(epic.Id))
                .ReturnsAsync(items);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InactiveWithReferences_SetsModelError(
            Epic epic,
            List<CatalogueItem> items,
            DeleteSupplierDefinedEpicConfirmationModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            DeleteSupplierDefinedEpicConfirmationModelValidator validator)
        {
            model.Id = epic.Id;
            model.Name = epic.Name;

            epic.IsActive = false;
            epic.SupplierDefined = true;

            service.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            service.Setup(s => s.GetItemsReferencingEpic(epic.Id))
                .ReturnsAsync(items);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InactiveNoReferences_NoModelError(
            Epic epic,
            DeleteSupplierDefinedEpicConfirmationModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            DeleteSupplierDefinedEpicConfirmationModelValidator validator)
        {
            model.Id = epic.Id;
            model.Name = epic.Name;

            epic.IsActive = false;
            epic.SupplierDefined = true;

            service.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            service.Setup(s => s.GetItemsReferencingEpic(epic.Id))
                .ReturnsAsync(Array.Empty<CatalogueItem>().ToList());

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

    }
}
