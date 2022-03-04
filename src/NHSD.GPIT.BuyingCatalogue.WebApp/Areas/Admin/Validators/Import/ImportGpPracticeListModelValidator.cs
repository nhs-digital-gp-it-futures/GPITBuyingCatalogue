using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ImportModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Import
{
    public sealed class ImportGpPracticeListModelValidator : AbstractValidator<ImportGpPracticeListModel>
    {
        public const string CSvUrlErrorMessage = "Please enter the url of the csv file to import.";

        public ImportGpPracticeListModelValidator(IUrlValidator urlValidator)
        {
            RuleFor(x => x.CsvUrl)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(CSvUrlErrorMessage);

            RuleFor(m => m.CsvUrl)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.CsvUrl));
        }
    }
}
