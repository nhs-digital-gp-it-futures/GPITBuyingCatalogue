using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration
{
    public class UpdatePasswordViewModelValidator : AbstractValidator<UpdatePasswordViewModel>
    {
        public const string CurrentPasswordRequired = "Enter your current password";
        public const string CurrentPasswordIncorrect = "Current password incorrect";
        public const string NewPasswordRequired = "Enter a new password";
        public const string ConfirmPasswordRequired = "Confirm new password";
        public const string ConfirmPasswordMismatch = "Passwords do not match";

        public UpdatePasswordViewModelValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(CurrentPasswordRequired);

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(NewPasswordRequired);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(ConfirmPasswordRequired)
                .Equal(x => x.NewPassword)
                .WithMessage(ConfirmPasswordMismatch);

            RuleFor(x => x)
                .Custom((x, context) =>
                {
                    if (!x.IdentityResult.Succeeded)
                    {
                        if (x.IncorrectPasswordError)
                            context.AddFailure(nameof(x.CurrentPassword), CurrentPasswordIncorrect);

                        if (x.InvalidPasswordError)
                            context.AddFailure(nameof(x.NewPassword), PasswordValidator.PasswordConditionsNotMet);

                        if (x.PasswordUsedBefore)
                            context.AddFailure(nameof(x.NewPassword), PasswordValidator.PasswordAlreadyUsed);
                    }
                })
                .When(x => x.IdentityResult != null);
        }
    }
}
