using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Orders;

public sealed class CallOffTermsDeclarationModelValidator : AbstractValidator<CallOffTermsDeclarationModel>
{
    internal const string DeclarationRequiredErrorMessage = "You must agree before continuing";

    public CallOffTermsDeclarationModelValidator()
    {
        RuleFor(x => x.DeclarationAccepted)
            .Equal(true)
            .WithMessage(DeclarationRequiredErrorMessage);
    }
}
