using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ImportModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Import
{
    public sealed class ImportGpPracticeListModelValidator : AbstractValidator<ImportGpPracticeListModel>
    {
        public ImportGpPracticeListModelValidator(IUrlValidator urlValidator)
        {
            RuleFor(x => x.CsvUrl)
                .NotEmpty()
                .WithMessage("Please enter the url of the CSV file to import.");

            RuleFor(m => m.CsvUrl)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.CsvUrl));
        }
    }
}
