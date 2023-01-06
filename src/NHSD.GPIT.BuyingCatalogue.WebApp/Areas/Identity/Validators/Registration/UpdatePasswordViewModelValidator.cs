using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration
{
    public class UpdatePasswordViewModelValidator : AbstractValidator<UpdatePasswordViewModel>
    {
        public const string CurrentPasswordRequired = "Enter current password";
        public const string CurrentPasswordMismatchCode = "PasswordMismatch";
        public const string CurrentPasswordIncorrect = "Current password incorrect";
        public const string NewPasswordRequired = "Enter a new password";
        public const string ConfirmPasswordRequired = "Confirm new password";
        public const string PasswordMismatch = "Passwords do not match";

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
                .Matches(x => x.NewPassword)
                .WithMessage(PasswordMismatch);

        }
    }
}
