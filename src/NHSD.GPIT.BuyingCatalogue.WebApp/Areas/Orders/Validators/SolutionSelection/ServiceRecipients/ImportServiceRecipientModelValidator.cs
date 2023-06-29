using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients;

public class ImportServiceRecipientModelValidator : AbstractValidator<ImportServiceRecipientModel>
{
    internal const long Mb = MaxMbSize * (Kb * Kb);
    internal const int MaxMbSize = 1;
    internal const string NoFileSpecified = "Select a CSV file to upload";
    internal const string InvalidFileType = "The selected file must be a CSV";
    internal static readonly string InvalidFileSize = $"The selected file must be smaller than {MaxMbSize}MB";

    private const long Kb = 1024;
    private const string AllowedFileExtension = ".csv";

    public ImportServiceRecipientModelValidator()
    {
        RuleFor(m => m.File)
            .NotNull()
            .WithMessage(NoFileSpecified)
            .Must(m => m.FileName.EndsWith(AllowedFileExtension))
            .WithMessage(InvalidFileType)
            .Must(m => m.Length <= Mb)
            .WithMessage(InvalidFileSize);
    }
}
