using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeStyle;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared
{
    public class RemoveServiceModel : NavBaseModel
    {
        private readonly string yesOption;

        private readonly string noOption;

        public RemoveServiceModel()
        {
        }

        public RemoveServiceModel(
            Order order,
            CatalogueItem service,
            bool isAdditionalService = true)
        {
            CallOffId = order.CallOffId;
            Service = service;
            ServiceType = isAdditionalService ? "Additional Service" : "Associated Service";
            yesOption = $"Yes, I confirm I want to remove {Service.Name}";
            noOption = $"No, I want to keep my current {ServiceType}s";
        }

        public IList<SelectOption<bool>> RemoveServiceOptions => new List<SelectOption<bool>>
        {
            new(yesOption, true),
            new(noOption, false),
        };

        public string ServiceType { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItem Service { get; set; }

        public bool? ConfirmRemoveService { get; set; }
    }
}
