using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public sealed class EditSLAContactsModelValidator : AbstractValidator<EditSLAContactModel>
    {
        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;

        public EditSLAContactsModelValidator(IServiceLevelAgreementsService serviceLevelAgreementsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService
                ?? throw new ArgumentNullException(nameof(serviceLevelAgreementsService));

            RuleFor(slac => slac)
            .MustAsync(NotBeDuplicateContact)
            .OverridePropertyName(
            slac => slac.Channel,
            slac => slac.ContactInformation)
            .WithMessage("A contact with these details already exists");

            RuleFor(slac => slac.Channel)
                .NotEmpty()
                .WithMessage("Enter a contact channel");

            RuleFor(slac => slac.ContactInformation)
                .NotEmpty()
                .WithMessage("Enter contact information");

            RuleFor(slac => slac.From)
                .NotEmpty()
                .WithMessage("Enter a from time");

            RuleFor(slac => slac.Until)
                .NotEmpty()
                .Unless(slac => !slac.From.HasValue)
                .WithMessage("Enter an until time");
        }

        private async Task<bool> NotBeDuplicateContact(EditSLAContactModel model, CancellationToken cancellationToken)
        {
            _ = cancellationToken;

            var serviceLevelAgreements = await serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId);

            return !serviceLevelAgreements.Contacts.Any(slac =>
            slac.Id != model.ContactId
            && string.Equals(slac.Channel, model.Channel, StringComparison.CurrentCultureIgnoreCase)
            && string.Equals(slac.ContactInformation, model.ContactInformation, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
