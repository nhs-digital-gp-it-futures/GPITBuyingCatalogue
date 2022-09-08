using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users
{
    public class OrganisationModelValidator : AbstractValidator<OrganisationModel>
    {
        public const string OrganisationMissingErrorMessage = "Please select an organisation";
        public const string MustBelongToNhsDigitalErrorMessage = "Admins must be a member of NHS Digital";

        private readonly IUsersService usersService;

        public OrganisationModelValidator(IUsersService usersService)
        {
            this.usersService = usersService;

            RuleFor(x => x.SelectedOrganisationId)
                .NotEmpty()
                .WithMessage(OrganisationMissingErrorMessage)
                .Must((model, selectedOrganisationId) => BelongToCorrectOrganisation(model.UserId, selectedOrganisationId))
                .WithMessage(MustBelongToNhsDigitalErrorMessage);
        }

        private bool BelongToCorrectOrganisation(int userId, string organisationId)
        {
            var hasRole = usersService.HasRole(userId, OrganisationFunction.AuthorityName).GetAwaiter().GetResult();
            if (!hasRole)
                return true;

            return organisationId == $"{OrganisationConstants.NhsDigitalOrganisationId}";
        }
    }
}
