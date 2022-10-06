﻿using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates.Edit.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates.Edit
{
    public class EditDatesAssociatedServiceDifferentDeliveryDates : EditDates
    {
        private const int OrderId = 90021;
        private const string InternalOrgId = "CG-03F";
        private const string ItemName = "E2E Single Price Added Associated Service";
        private const string ItemType = "Associated Service";
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "S-999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrderId), $"{OrderId}" },
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
            { nameof(ItemName), ItemName },
            { nameof(ItemType), ItemType },
        };

        public EditDatesAssociatedServiceDifferentDeliveryDates(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string BackLinkMethod => nameof(DeliveryDatesController.MatchDates);

        protected override bool DisplayEditDatesLink => false;

        protected override string OnwardMethod => nameof(DeliveryDatesController.Review);
    }
}
