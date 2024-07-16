using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation
{
    public static class TermsOfUseModelTests
    {
        [Theory]
        [MockAutoData]
        public static void TestValidate_AcceptsLatestTerms_NoModelErrors(
            TermsOfUseModel model,
            TermsOfUseModelValidator validator)
        {
            model.AlreadyAcceptedLatestTerms = false;
            model.HasAcceptedPrivacyPolicy = true;
            model.HasAcceptedTermsOfUse = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void TestValidate_DoesNotAcceptLatestTerms_SetsModelErrors(
            TermsOfUseModel model,
            TermsOfUseModelValidator validator)
        {
            model.AlreadyAcceptedLatestTerms = false;
            model.HasAcceptedPrivacyPolicy = false;
            model.HasAcceptedTermsOfUse = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HasAcceptedTermsOfUse)
                .WithErrorMessage(TermsOfUseModelValidator.TermsOfUseErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.HasAcceptedPrivacyPolicy)
                .WithErrorMessage(TermsOfUseModelValidator.PrivacyPolicyErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void TestValidate_HasPreviouslyAcceptedTerms_NoModelErrors(
            TermsOfUseModel model,
            TermsOfUseModelValidator validator)
        {
            model.IsBuyer = true;
            model.AlreadyAcceptedLatestTerms = true;
            model.HasAcceptedPrivacyPolicy = false;
            model.HasAcceptedTermsOfUse = false;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
