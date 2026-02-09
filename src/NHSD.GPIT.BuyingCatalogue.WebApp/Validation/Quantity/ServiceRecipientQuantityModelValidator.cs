using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity
{
    [ChildValidator]
    public class ServiceRecipientQuantityModelValidator : AbstractValidator<ServiceRecipientQuantityModel>
    {
        public const string ValueNotNumericErrorMessage = "Quantity must be a number";
        public const string ValueGreaterThanZeroErrorMessage = "Quantity must be greater than zero";
        public const string ValueNegativeErrorMessage = "Quantity must be greater than or equal to zero";

        public const string PracticeValueNotNumericErrorMessage = "Practice list size for {0} must be a number";
        public const string PracticeValueGreaterThanZeroErrorMessage = "Practice list size for {0} must be greater than zero";
        public const string PracticeValueNegativeErrorMessage = "Practice list size for {0} must be greater than or equal to zero";

        public ServiceRecipientQuantityModelValidator(
            ProvisioningType provisioningType,
            CatalogueItemType catalogueItemType)
        {
            RuleFor(x => x.InputQuantity)
                .Cascade(CascadeMode.Stop)
                .Must(HaveAnIntegerValue)
                .WithMessage(m => GetErrorMessage(
                    provisioningType,
                    string.Format(PracticeValueNotNumericErrorMessage, m.Name),
                    ValueNotNumericErrorMessage))
                .Must(m => HaveAPositiveValue(m, catalogueItemType))
                .WithMessage(m => catalogueItemType is CatalogueItemType.Solution
                    ? GetErrorMessage(
                        provisioningType,
                        string.Format(PracticeValueGreaterThanZeroErrorMessage, m.Name),
                        ValueGreaterThanZeroErrorMessage)
                    : GetErrorMessage(
                        provisioningType,
                        string.Format(PracticeValueNegativeErrorMessage, m.Name),
                        ValueNegativeErrorMessage))
                .When(x => !string.IsNullOrEmpty(x.InputQuantity));
        }

        private static string GetErrorMessage(
            ProvisioningType provisioningType,
            string patientErrorMessage,
            string valueErrorMessage) =>
            provisioningType is ProvisioningType.Patient ? patientErrorMessage : valueErrorMessage;

        private static bool HaveAnIntegerValue(string inputQuantity) =>
            (!string.IsNullOrEmpty(inputQuantity) && int.TryParse(inputQuantity, out _))
            || string.IsNullOrEmpty(inputQuantity);

        private static bool
            HaveAPositiveValue(string inputQuantity, CatalogueItemType catalogueItemType) =>
            int.Parse(inputQuantity) > catalogueItemType switch
            {
                CatalogueItemType.Solution => 0,
                _ => -1,
            };
    }
}
