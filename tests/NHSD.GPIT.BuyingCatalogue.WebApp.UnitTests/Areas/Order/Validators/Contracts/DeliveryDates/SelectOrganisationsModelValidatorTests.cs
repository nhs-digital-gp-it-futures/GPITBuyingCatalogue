using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates;

public static class SelectOrganisationsModelValidatorTests
{
    [Fact]
    public static void Validate_RecipientsIsNull_ThrowsValidationError()
    {
        var model = new SelectOrganisationsModel
        {
            Recipients = null,
        };
        var validator = new SelectOrganisationsModelValidator();

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Recipients)
            .WithErrorMessage(SelectOrganisationsModelValidator.RecipientsRequiredErrorMessage);
    }

    [Fact]
    public static void Validate_NoRecipientsAreSelected_ThrowsValidationError()
    {
        var model = new SelectOrganisationsModel
        {
            Recipients =
            [
                CreateRecipient(selected: false),
                CreateRecipient(selected: false),
            ],
        };
        var validator = new SelectOrganisationsModelValidator();

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Recipients)
            .WithErrorMessage(SelectOrganisationsModelValidator.RecipientsRequiredErrorMessage);
    }

    [Fact]
    public static void Validate_AtLeastOneRecipientIsSelected_HasNoValidationErrors()
    {
        var model = new SelectOrganisationsModel
        {
            Recipients =
            [
                CreateRecipient(selected: false),
                CreateRecipient(selected: true),
            ],
        };
        var validator = new SelectOrganisationsModelValidator();

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static SelectRecipientToEditModel CreateRecipient(bool selected)
    {
        return new SelectRecipientToEditModel
        {
            Selected = selected,
            ParentSublocationName = "Sublocation organisation",
            ParentSublocationOdsCode = "SLOC1",
            RecipientOrganisationId = "recipient-id",
            RecipientName = "Recipient organisation",
            RecipientOdsCode = "RECIPIENT1",
        };
    }
}
