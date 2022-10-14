using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users
{
    public class AccountTypeModelValidator : AbstractValidator<AccountTypeModel>
    {
        public const string SelectedAccountTypeEmptyErrorMessage = "Select an account type";
        public const string MustBelongToNhsDigitalErrorMessage = "Admins must be a member of NHS Digital";

        private readonly IUsersService usersService;

        public AccountTypeModelValidator(IUsersService usersService)
        {
            this.usersService = usersService;

            RuleFor(x => x.SelectedAccountType)
                .NotEmpty()
                .WithMessage(SelectedAccountTypeEmptyErrorMessage)
                .Must((model, accountType) => BelongToCorrectOrganisation(model.UserId, accountType))
                .WithMessage(MustBelongToNhsDigitalErrorMessage);
        }

        private bool BelongToCorrectOrganisation(int userId, string accountType)
        {
            if (accountType != OrganisationFunction.Authority.Name)
                return true;

            var user = usersService.GetUser(userId).Result;

            return user.PrimaryOrganisationId == OrganisationConstants.NhsDigitalOrganisationId;
        }
    }
}
