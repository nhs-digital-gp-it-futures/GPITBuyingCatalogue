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
    public static class EditSupplierDetailsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidRequest_NoValidationErrors(
            EditSupplierDetailsModel model,
            EditSupplierDetailsModelValidator validator)
        {
            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.AvailableSupplierContacts);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_NoSelectedContacts_SetsModelError(
            EditSupplierDetailsModel model,
            EditSupplierDetailsModelValidator validator)
        {
            model.AvailableSupplierContacts.ForEach(c => c.Selected = false);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.AvailableSupplierContacts)
                .WithErrorMessage("Select a supplier contact");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_MoreThan2SelectedContacts_SetsModelError(
            Fixture fixture,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                AvailableSupplierContacts = fixture.Build<AvailableSupplierContact>().With(c => c.Selected, true).CreateMany(3).ToList(),
            };

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.AvailableSupplierContacts)
                .WithErrorMessage("You can only select up to two supplier contacts");
        }
    }
}
