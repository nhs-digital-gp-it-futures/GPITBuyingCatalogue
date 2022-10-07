﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IDeliveryDateService
    {
        public Task SetDeliveryDate(string internalOrgId, CallOffId callOffId, DateTime deliveryDate);

        public Task SetDeliveryDates(int orderId, CatalogueItemId catalogueItemId, List<RecipientDeliveryDateDto> deliveryDates);

        public Task MatchDeliveryDates(int orderId, CatalogueItemId solutionId, CatalogueItemId serviceId);
    }
}
