using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation.TestHelper;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditSolutionContactsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidRequest_NoValidationErrors(
            EditSolutionContactsModel model,
            EditSolutionContactsModelValidator validator)
        {
            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(EditSolutionContactsModelValidator.ErrorElementName);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_NoSelectedContacts_SetsModelError(
            EditSolutionContactsModel model,
            EditSolutionContactsModelValidator validator)
        {
            model.AvailableSupplierContacts.ForEach(c => c.Selected = false);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(EditSolutionContactsModelValidator.ErrorElementName)
                .WithErrorMessage("Select a supplier contact");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_MoreThan2SelectedContacts_SetsModelError(
            Fixture fixture,
            EditSolutionContactsModelValidator validator)
        {
            var model = new EditSolutionContactsModel
            {
                AvailableSupplierContacts = fixture.Build<AvailableSupplierContact>().With(c => c.Selected, true).CreateMany(3).ToList(),
            };

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(EditSolutionContactsModelValidator.ErrorElementName)
                .WithErrorMessage("You can only select up to two supplier contacts");
        }
    }
}
