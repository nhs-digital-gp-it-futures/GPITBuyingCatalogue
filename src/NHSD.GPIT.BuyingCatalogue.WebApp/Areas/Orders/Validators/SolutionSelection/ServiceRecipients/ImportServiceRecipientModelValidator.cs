using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients;

public class ImportServiceRecipientModelValidator : AbstractValidator<ImportServiceRecipientModel>
{
    internal const long Mb = MaxMbSize * (Kb * Kb);
    internal const int MaxMbSize = 1;
    internal static readonly string InvalidFileSize = $"The selected file must be smaller than {MaxMbSize}MB";

    private const long Kb = 1024;

    public ImportServiceRecipientModelValidator()
    {
        RuleFor(m => m.File)
            .IsValidCsv()
            .Must(m => m.Length <= Mb)
            .WithMessage(InvalidFileSize);
    }
}
