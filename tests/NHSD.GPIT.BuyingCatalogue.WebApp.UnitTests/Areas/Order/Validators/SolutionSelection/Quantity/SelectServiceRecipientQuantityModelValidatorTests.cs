using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Quantity
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
        [CommonInlineAutoData("-1", false)]
        [CommonInlineAutoData("0.5", false)]
        [CommonInlineAutoData("a", false)]
        [CommonInlineAutoData("0", false)]
        [CommonInlineAutoData("1", true)]
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
