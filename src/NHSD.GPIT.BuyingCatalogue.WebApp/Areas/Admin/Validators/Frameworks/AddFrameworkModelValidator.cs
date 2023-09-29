using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Frameworks;

public sealed class AddFrameworkModelValidator : AbstractValidator<AddFrameworkModel>
{
    internal const string FundingTypeError = "Select a funding type";
    internal const string NameMissingError = "Enter a name";
    internal const string NameDuplicationError = "Name already exists. Try another name";

    public AddFrameworkModelValidator(
        IFrameworkService frameworkService)
    {
        RuleFor(x => x.FundingTypes)
            .Must(x => x.Any(y => y.Selected))
            .WithMessage(FundingTypeError)
            .OverridePropertyName($"{nameof(AddFrameworkModel.FundingTypes)}[0].Selected");

        RuleFor(x => x)
            .NotEmpty()
            .WithMessage(NameMissingError)
            .Must(x => !frameworkService.FrameworkNameExistsExcludeSelf(x.Name, x.FrameworkId).GetAwaiter().GetResult())
            .WithMessage(NameDuplicationError);
    }
}
