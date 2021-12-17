using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ServiceLevelAgreementValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ServiceLevelAgreementValidators
{
    public static class EditSLAContactModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_EmptyChannel_SetsModelError(
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.Channel = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Channel)
                .WithErrorMessage("Enter a contact channel");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmptyContactInformation_SetsModelError(
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.ContactInformation = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ContactInformation)
                .WithErrorMessage("Enter contact information");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmptyFrom_SetsModelError(
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.From = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.From)
                .WithErrorMessage("Enter a from time");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmptyUntil_SetsModelError(
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            model.Until = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Until)
                .WithErrorMessage("Enter an until time");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Duplicate_SetsModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            slaContact.ApplicableDays = string.Empty;
            model.ApplicableDays = string.Empty;

            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;

            serviceLevelAgreementsService.Setup(s => s.GetServiceLevelAgreementForSolution(model.SolutionId))
                .ReturnsAsync(new ServiceLevelAgreements
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
        [CommonAutoData]
        public static void Validate_DuplicateWithApplicableDays_SetsModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;
            model.ApplicableDays = slaContact.ApplicableDays;

            serviceLevelAgreementsService.Setup(s => s.GetServiceLevelAgreementForSolution(model.SolutionId))
                .ReturnsAsync(new ServiceLevelAgreements
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
        [CommonAutoData]
        public static void Validate_Edit_NoModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            model.ContactId = slaContact.Id;
            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;

            serviceLevelAgreementsService.Setup(s => s.GetServiceLevelAgreementForSolution(model.SolutionId))
                .ReturnsAsync(new ServiceLevelAgreements
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
        [CommonAutoData]
        public static void Validate_EditWithApplicableDays_NoModelError(
            SlaContact slaContact,
            EditSLAContactModel model,
            [Frozen] Mock<IServiceLevelAgreementsService> serviceLevelAgreementsService,
            EditSLAContactModelValidator validator)
        {
            model.ContactId = slaContact.Id;
            model.Channel = slaContact.Channel;
            model.ContactInformation = slaContact.ContactInformation;
            model.ApplicableDays = slaContact.ApplicableDays;

            serviceLevelAgreementsService.Setup(s => s.GetServiceLevelAgreementForSolution(model.SolutionId))
                .ReturnsAsync(new ServiceLevelAgreements
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
        [CommonAutoData]
        public static void Validate_Valid_NoErrors(
            EditSLAContactModel model,
            EditSLAContactModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
