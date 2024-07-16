using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public static class EditSLAContactModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_EmptyChannel_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.Channel = string.Empty;

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Channel)
                .WithErrorMessage("Enter a contact channel");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmptyContactInformation_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.ContactInformation = string.Empty;

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ContactInformation)
                .WithErrorMessage("Enter contact information");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmptyFrom_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.From = null;

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.From)
                .WithErrorMessage("Enter a from time");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmptyUntil_SetsModelError(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.Until = null;

            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Until)
                .WithErrorMessage("Enter an until time");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Duplicate_SetsModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            slaContact.ApplicableDays = string.Empty;
            model.ApplicableDays = string.Empty;

            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;

            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId)
                .Returns(new ServiceLevelAgreements
                {
                    Contacts = new HashSet<SlaContact>
                    {
                        slaContact,
                    },
                });

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Channel|ContactInformation")
                .WithErrorMessage(EditSLAContactModelValidator.DuplicateContactErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_DuplicateWithApplicableDays_SetsModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;
            model.ApplicableDays = slaContact.ApplicableDays;

            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId)
                .Returns(new ServiceLevelAgreements
                {
                    Contacts = new HashSet<SlaContact>
                    {
                        slaContact,
                    },
                });

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Channel|ContactInformation|ApplicableDays")
                .WithErrorMessage(EditSLAContactModelValidator.DuplicateContactErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Edit_NoModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            model.ContactId = slaContact.Id;
            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;

            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId)
                .Returns(new ServiceLevelAgreements
                {
                    Contacts = new HashSet<SlaContact>
                    {
                        slaContact,
                    },
                });

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor("Channel|ContactInformation");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditWithApplicableDays_NoModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] IServiceLevelAgreementsService serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            model.ContactId = slaContact.Id;
            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;
            model.ApplicableDays = slaContact.ApplicableDays;

            serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(model.SolutionId)
                .Returns(new ServiceLevelAgreements
                {
                    Contacts = new HashSet<SlaContact>
                    {
                        slaContact,
                    },
                });

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor("Channel|ContactInformation|ApplicableDays");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoErrors(
            ServiceLevelAgreements serviceLevelAgreement,
            [Frozen] IServiceLevelAgreementsService service,
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            service.GetServiceLevelAgreementForSolution(model.SolutionId).Returns(serviceLevelAgreement);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
