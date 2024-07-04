using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class SelectFrameworkModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectedFramework_SetsModelError(
            SelectFrameworkModel model,
            SelectFrameworkModelValidator validator)
        {
            model.SelectedFrameworkId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedFrameworkId)
                .WithErrorMessage(SelectFrameworkModelValidator.FrameworkRequiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ExpiredFramework_SetsModelError(
            SelectFrameworkModel model,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] IFrameworkService frameworkService,
            SelectFrameworkModelValidator validator)
        {
            framework.IsExpired = true;
            frameworkService.GetFramework(model.SelectedFrameworkId).Returns(framework);
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedFrameworkId)
                .WithErrorMessage(SelectFrameworkModelValidator.FrameworkExpiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectedFramework_NoModelError(
            SelectFrameworkModel model,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] IFrameworkService frameworkService,
            SelectFrameworkModelValidator validator)
        {
            framework.IsExpired = false;
            frameworkService.GetFramework(model.SelectedFrameworkId).Returns(framework);
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
