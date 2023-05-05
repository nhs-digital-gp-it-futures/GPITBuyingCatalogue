using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.ManageFilters
{
    public class SaveFilterModelValidator : AbstractValidator<SaveFilterModel>
    {
        public const string NameRequiredErrorMessage = "Enter a filter name";
        public const string DuplicateNameErrorMessage = "You already have a filter with that name. Try a different one";
        public const string DescriptionRequiredErrorMessage = "Enter a filter description";

        private readonly IManageFiltersService manageFiltersService;

        public SaveFilterModelValidator(IManageFiltersService manageFiltersService)
        {
            this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(NameRequiredErrorMessage)
                .Must((model, name) => NotBeDuplicated(name, model.OrganisationId))
                .WithMessage(DuplicateNameErrorMessage);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(DescriptionRequiredErrorMessage);
        }

        private bool NotBeDuplicated(string name, int organisationId)
        {
            return !manageFiltersService.FilterExists(name, organisationId).GetAwaiter().GetResult();
        }
    }
}
