using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ImportServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.ServiceRecipients;

public class ImportServiceRecipientModelValidator : AbstractValidator<ImportServiceRecipientModel>
{
    internal const string NoFileSpecified = "Select a CSV file to upload";
    internal const string InvalidFileType = "The selected file must be a CSV";
    internal static readonly string InvalidFileSize = $"The selected file must be smaller than {MaxMbSize}MB";

    private const int MaxMbSize = 1;
    private const string AllowedFileExtension = ".csv";

    private const long Kb = 1024;
    private const long Mb = MaxMbSize * (Kb * Kb);

    public ImportServiceRecipientModelValidator()
    {
        RuleFor(m => m.File)
            .NotNull()
            .WithMessage(NoFileSpecified)
            .Must(m => m.FileName.EndsWith(AllowedFileExtension))
            .WithMessage(InvalidFileType)
            .Must(m => m.Length < Mb)
            .WithMessage(InvalidFileSize);
    }
}
