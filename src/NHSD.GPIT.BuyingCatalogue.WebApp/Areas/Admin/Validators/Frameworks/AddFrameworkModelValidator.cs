using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Frameworks;

public sealed class AddFrameworkModelValidator : AbstractValidator<AddFrameworkModel>
{
    internal const string FundingTypeError = "Select a funding type";
    internal const string NameMissingError = "Enter a name";
    internal const string NameDuplicationError = "Name already exists. Try another name";

    public AddFrameworkModelValidator(
        IFrameworkService frameworkService)
    {
        RuleFor(x => x.IsLocalFundingOnly)
            .NotNull()
            .WithMessage(FundingTypeError);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(NameMissingError)
            .Must(x => !frameworkService.FrameworkNameExists(x).GetAwaiter().GetResult())
            .WithMessage(NameDuplicationError);
    }
}
