using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators
{
    public class SelectFrameworkModelValidator : AbstractValidator<SelectFrameworkModel>
    {
        internal const string FrameworkRequiredErrorMessage = "Select a framework";

        internal const string FrameworkExpiredErrorMessage =
            "This framework has expired so it cannot be selected. Choose a different framework";

        private readonly IFrameworkService frameworkService;

        public SelectFrameworkModelValidator(IFrameworkService frameworkService)
        {
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));

            RuleFor(m => m.SelectedFrameworkId)
                .NotNull()
                .WithMessage(FrameworkRequiredErrorMessage)
                .Must(NotExpired)
                .WithMessage(FrameworkExpiredErrorMessage);
        }

        private bool NotExpired(string frameworkId) => !frameworkService.GetFramework(frameworkId).GetAwaiter().GetResult()?.IsExpired ?? false;
    }
}
