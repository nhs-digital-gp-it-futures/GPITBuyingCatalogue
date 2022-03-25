using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    public class TermsOfUseModelValidator : AbstractValidator<TermsOfUseModel>
    {
        internal const string TermsOfUseErrorMessage = "Confirm you have read and agreed to the terms of use";
        internal const string PrivacyPolicyErrorMessage = "Confirm you have read and understood the privacy policy";

        public TermsOfUseModelValidator()
        {
            RuleFor(m => m.HasAcceptedTermsOfUse)
                .Equal(true)
                .WithMessage(TermsOfUseErrorMessage)
                .Unless(m => m.AlreadyAcceptedLatestTerms || !m.IsBuyer);

            RuleFor(m => m.HasAcceptedPrivacyPolicy)
                .Equal(true)
                .WithMessage(PrivacyPolicyErrorMessage)
                .Unless(m => m.AlreadyAcceptedLatestTerms || !m.IsBuyer);
        }
    }
}
