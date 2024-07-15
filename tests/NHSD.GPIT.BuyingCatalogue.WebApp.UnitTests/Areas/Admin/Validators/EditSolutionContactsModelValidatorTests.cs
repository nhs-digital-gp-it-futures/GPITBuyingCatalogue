using System.Linq;
using AutoFixture;
using FluentValidation.TestHelper;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditSolutionContactsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValidRequest_NoValidationErrors(
            EditSolutionContactsModel model,
            EditSolutionContactsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(EditSolutionContactsModelValidator.ErrorElementName);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectedContacts_SetsModelError(
            EditSolutionContactsModel model,
            EditSolutionContactsModelValidator validator)
        {
            model.AvailableSupplierContacts.ForEach(c => c.Selected = false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(EditSolutionContactsModelValidator.ErrorElementName)
                .WithErrorMessage("Select a supplier contact");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MoreThan2SelectedContacts_SetsModelError(
            Fixture fixture,
            EditSolutionContactsModelValidator validator)
        {
            var model = new EditSolutionContactsModel
            {
                AvailableSupplierContacts = fixture.Build<AvailableSupplierContact>().With(c => c.Selected, true).CreateMany(3).ToList(),
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(EditSolutionContactsModelValidator.ErrorElementName)
                .WithErrorMessage("You can only select up to two supplier contacts");
        }
    }
}
