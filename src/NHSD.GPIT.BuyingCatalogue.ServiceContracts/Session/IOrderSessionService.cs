﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session
{
    public interface IOrderSessionService
    {
        public CreateOrderItemModel GetOrderStateFromSession(CallOffId callOffId);

        public void SetOrderStateToSession(CreateOrderItemModel model);

        public CreateOrderItemModel InitialiseStateForCreate(Order order, CatalogueItemType catalogueItemType, IEnumerable<CatalogueItemId> solutionIds, OrderItemRecipientModel associatedOrderRecipient);

        public Task<CreateOrderItemModel> InitialiseStateForEdit(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId);

        public CreateOrderItemModel SetPrice(CallOffId callOffId, CataloguePrice cataloguePrice);

        public void ClearSession(CallOffId callOffId);
    }
}
