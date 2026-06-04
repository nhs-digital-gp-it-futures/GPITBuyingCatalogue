using System.Data;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AccountRequestsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.AccountRequests;

public sealed class AccountRequestModelValidator : AbstractValidator<AccountRequestModel>
{
    public AccountRequestModelValidator()
    {
        RuleFor(x => x.DecisionJustification)
            .NotEmpty()
            .WithMessage("Enter a decision justification");

        RuleFor(x => x.SelectedStatus)
            .NotNull()
            .WithMessage("Select an account request status");
    }
}
