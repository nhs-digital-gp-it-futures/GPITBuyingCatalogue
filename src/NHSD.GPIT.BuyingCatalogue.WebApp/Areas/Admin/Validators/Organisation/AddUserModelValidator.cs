using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation
{
    public sealed class AddUserModelValidator : AbstractValidator<AddUserModel>
    {
        private readonly ICreateBuyerService createBuyerService;

        public AddUserModelValidator(ICreateBuyerService createBuyerService)
        {
            this.createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));

            RuleFor(m => m.FirstName)
                .NotEmpty()
                .WithMessage("Enter a first name");

            RuleFor(m => m.LastName)
                .NotEmpty()
                .WithMessage("Enter a last name");

            RuleFor(m => m.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Enter an email address")
                .EmailAddress()
                .WithMessage("Enter an email address in the correct format, like name@example.com")
                .MustAsync(NotBeDuplicateUserEmail)
                .WithMessage("A user with this email address is already registered on the Buying Catalogue");
        }

        private async Task<bool> NotBeDuplicateUserEmail(string emailAddress, CancellationToken cancellationToken) => !await createBuyerService.UserExistsWithEmail(emailAddress);
    }
}
