using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Frameworks;

public sealed class AddFrameworkModelValidator : AbstractValidator<AddEditFrameworkModel>
{
    internal const string FundingTypeError = "Select a funding type";
    internal const string NameMissingError = "Enter a name";
    internal const string NameDuplicationError = "Name already exists. Try another name";
    internal const string MaximumDurationMissingError = "Enter a maximum duration";
    internal const string MaximumDurationMustBeANumberError = "Maximum duration must be a number";
    internal const string MaximumDurationGreaterThanZeroError = "Maximum duration must be greater than zero";

    public AddFrameworkModelValidator(
        IFrameworkService frameworkService)
    {
        RuleFor(x => x.FundingTypes)
            .Must(x => x.Any(y => y.Selected))
            .WithMessage(FundingTypeError)
            .OverridePropertyName($"{nameof(AddEditFrameworkModel.FundingTypes)}[0].Selected");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(NameMissingError)
            .Must((model, name) => !frameworkService.FrameworkNameExistsExcludeSelf(name, model.FrameworkId).GetAwaiter().GetResult())
            .WithMessage(NameDuplicationError);

        RuleFor(x => x.MaximumTerm)
             .NotEmpty()
             .WithMessage(MaximumDurationMissingError)
             .Must(x => int.TryParse(x, out _))
             .WithMessage(MaximumDurationMustBeANumberError)
             .Must(x => int.Parse(x) > 0)
             .WithMessage(MaximumDurationGreaterThanZeroError);
    }
}
