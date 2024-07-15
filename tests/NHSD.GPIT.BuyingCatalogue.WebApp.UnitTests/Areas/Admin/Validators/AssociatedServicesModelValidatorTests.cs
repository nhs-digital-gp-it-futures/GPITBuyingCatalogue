using System.Collections.Generic;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AssociatedServicesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_SolutionMergerAndSplits_SetsModelError(
            List<SelectableAssociatedService> services,
            AssociatedServicesModel model,
            AssociatedServicesModelValidator validator)
        {
            services.ForEach(s =>
            {
                s.PracticeReorganisation = PracticeReorganisationTypeEnum.Merger;
                s.Selected = true;
            });

            model.SelectableAssociatedServices.AddRange(services);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("selectable-associated-services")
                .WithErrorMessage("The solution already has an Associated Service of this type, so you cannot add another");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            AssociatedServicesModel model,
            AssociatedServicesModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
