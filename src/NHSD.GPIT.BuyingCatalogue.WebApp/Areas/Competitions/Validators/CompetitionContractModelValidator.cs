using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class CompetitionContractModelValidator : AbstractValidator<CompetitionContractModel>
{
    internal const string ContractLengthMissing = "Enter a contract length";
    internal const string ExceedsLimitError = "Contract length cannot exceed 36 months";
    internal const string TooLowError = "Contract length must be more than zero";

    public CompetitionContractModelValidator()
    {
        RuleFor(x => x.ContractLength)
            .NotNull()
            .WithMessage(ContractLengthMissing)
            .LessThanOrEqualTo(36)
            .WithMessage(ExceedsLimitError)
            .GreaterThan(0)
            .WithMessage(TooLowError);
    }
}
