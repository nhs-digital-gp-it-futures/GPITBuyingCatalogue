using System;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public sealed class EditSLAContactModelValidator : AbstractValidator<EditSLAContactModel>
    {
        public const string ContactChannelError = "Enter a contact channel";
        public const string ContactInformationError = "Enter contact information";
        public const string EnterFromTimeError = "Enter a from time";
        public const string EnterUntilTimeError = "Enter an until time";

        internal const string DuplicateContactErrorMessage = "A contact with these details already exists";

        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;

        public EditSLAContactModelValidator(IServiceLevelAgreementsService serviceLevelAgreementsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService
                ?? throw new ArgumentNullException(nameof(serviceLevelAgreementsService));

            RuleFor(slac => slac)
            .Must(NotBeDuplicateContact)
            .OverridePropertyName(
                slac => slac.Channel,
                slac => slac.ContactInformation)
            .WithMessage(DuplicateContactErrorMessage)
            .When(slac => string.IsNullOrWhiteSpace(slac.ApplicableDays));

            RuleFor(slac => slac)
            .Must(NotBeDuplicateContact)
            .OverridePropertyName(
                slac => slac.Channel,
                slac => slac.ContactInformation,
                slac => slac.ApplicableDays)
            .WithMessage(DuplicateContactErrorMessage)
            .Unless(slac => string.IsNullOrWhiteSpace(slac.ApplicableDays));

            RuleFor(slac => slac.Channel)
                .NotEmpty()
                .WithMessage(ContactChannelError);

            RuleFor(slac => slac.ContactInformation)
                .NotEmpty()
                .WithMessage(ContactInformationError);

            RuleFor(slac => slac.From)
                .NotEmpty()
                .WithMessage(EnterFromTimeError);

            RuleFor(slac => slac.Until)
                .NotEmpty()
                .Unless(slac => !slac.From.HasValue)
                .WithMessage(EnterUntilTimeError);
        }

        private bool NotBeDuplicateContact(EditSLAContactModel model)
        {
            var serviceLevelAgreements = serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId).GetAwaiter().GetResult();

            return !serviceLevelAgreements
                .Contacts
                .Any(slac =>
                    slac.Id != model.ContactId
                    && string.Equals(slac.ApplicableDays, model.ApplicableDays)
                    && string.Equals(slac.Channel, model.Channel, StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(slac.ContactInformation, model.ContactInformation, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
