using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Quantity
{
    public static class SelectServiceRecipientQuantityModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_FirstInputQuantityNotEntered_ValidationErrors(
           SelectServiceRecipientQuantityModel model,
           SelectServiceRecipientQuantityModelValidator validator)
        {
            var serviceRecipient = model.ServiceRecipients.FirstOrDefault();
            serviceRecipient.Quantity = 0;
            serviceRecipient.InputQuantity = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("ServiceRecipients[0].InputQuantity")
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueNotEnteredErrorMessage);

            for (int i = 1; i < model.ServiceRecipients.Count(); i++)
            {
                result.ShouldNotHaveValidationErrorFor($"ServiceRecipients[{i}].InputQuantity");
            }
        }
    }
}
