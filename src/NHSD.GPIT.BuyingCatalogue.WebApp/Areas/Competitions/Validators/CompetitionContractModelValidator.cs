using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class CompetitionContractModelValidator : AbstractValidator<CompetitionContractModel>
{
    internal const string ContractLengthMissing = "Enter a contract length";
    internal const string ExceedsLimitError = "Contract length cannot exceed {0} months";
    internal const string TooLowError = "Contract length must be greater than zero";

    public CompetitionContractModelValidator()
    {
        RuleFor(x => x.ContractLength)
            .NotNull()
            .WithMessage(ContractLengthMissing)
            .LessThanOrEqualTo((m) => m.ContractLengthLimit)
            .WithMessage((m) => string.Format(ExceedsLimitError, m.ContractLengthLimit))
            .GreaterThan(0)
            .WithMessage(TooLowError);
    }
}
