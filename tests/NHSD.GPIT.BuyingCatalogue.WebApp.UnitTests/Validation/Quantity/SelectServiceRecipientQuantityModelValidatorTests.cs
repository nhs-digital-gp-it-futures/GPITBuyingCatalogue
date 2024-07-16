using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Quantity
{
    public class SelectServiceRecipientQuantityModelValidatorTests
    {
        private readonly SelectServiceRecipientQuantityModelValidator validator;

        public SelectServiceRecipientQuantityModelValidatorTests()
        {
            validator = new SelectServiceRecipientQuantityModelValidator();
            validator.RuleForEach(x => x.ServiceRecipients)
                     .Cascade(CascadeMode.Continue)
                     .SetValidator(new ServiceRecipientQuantityModelValidator());
        }

        [Theory]
        [MockInlineAutoData("-1", false)]
        [MockInlineAutoData("0.5", false)]
        [MockInlineAutoData("a", false)]
        [MockInlineAutoData("0", false)]
        [MockInlineAutoData("1", true)]
        public void Validate__ShouldValidateServiceRecipients_CompareResults(string quantity, bool expected)
        {
#pragma warning disable SA1413
            var model = new SelectServiceRecipientQuantityModel
            {
                ServiceRecipients = new ServiceRecipientQuantityModel[]
                {
                    new ServiceRecipientQuantityModel
                    {
                        InputQuantity = quantity
                    }
                }
            };
#pragma warning restore SA1413

            var result = validator.Validate(model);

            Assert.Equal(expected, result.IsValid);
        }
    }
}
