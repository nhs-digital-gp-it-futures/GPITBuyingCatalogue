using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class SaveCompetitionModelValidator : AbstractValidator<SaveCompetitionModel>
{
    internal const string NameMissingError = "Enter a competition name";
    internal const string DuplicateNameError = "You already have a competition with that name. Try a different one";
    internal const string DescriptionMissingError = "Enter a competition description";

    public SaveCompetitionModelValidator(
        ICompetitionsService competitionsService)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(NameMissingError)
            .Must((model, name) => !competitionsService.Exists(model.InternalOrgId, name).GetAwaiter().GetResult())
            .WithMessage(DuplicateNameError);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(DescriptionMissingError);
    }
}
