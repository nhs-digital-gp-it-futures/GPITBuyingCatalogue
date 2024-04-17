using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.Notifications.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace BuyingCatalogueFunction.Notifications
{
    public class ContractExpiryFunction
    {
        private readonly ILogger<ContractExpiryFunction> logger;
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IContractExpiryService contractExpiryService;
        private readonly QueueServiceClient queueServiceClient;

        public ContractExpiryFunction(
            ILogger<ContractExpiryFunction> logger,
            BuyingCatalogueDbContext dbContext,
            IContractExpiryService contractExpiryService,
            QueueServiceClient queueServiceClient)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.contractExpiryService = contractExpiryService;
            this.queueServiceClient = queueServiceClient;
        }

        [Function(nameof(ContractExpiryFunction))]
        public async Task Run([TimerTrigger("0 0 10 * * *")] TimerInfo timerInfo)
        {
            var utcNow = DateTime.UtcNow;

            logger.LogInformation("Contract Expiry: Executed at {Date}", utcNow);
            
            if (timerInfo.ScheduleStatus is not null)
            {
                logger.LogInformation("Contract Expiry: Next timer schedule at {Next}", timerInfo.ScheduleStatus.Next);
            }

            await RunFor(utcNow.Date);
        }

        private async Task RunFor(DateTime today)
        {
            logger.LogInformation("Contract Expiry: Evaluating Orders for {Date}", today);
            List<Order> orders = await contractExpiryService.GetOrdersNearingExpiry(today);

            if (!orders.Any())
            {
                logger.LogInformation("Contract Expiry: No orders found nearing expiry for {Date}", today);
                return;
            }

            foreach (var order in orders)
            {
                try
                {
                    await EvaluateOrder(today, order);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Contract Expiry: Order {CallOffId}. Check inner exception", order.CallOffId);
                }
            }
        }

        private async Task EvaluateOrder(DateTime today, Order order)
        {
            var eventToRaise = order.EndDate.DetermineEventToRaise(today, order.ContractOrderNumber.OrderEvents);
            if (eventToRaise != EventTypeEnum.Nothing)
            {
                var defaultEmailPreference = await contractExpiryService.GetDefaultEmailPreference(eventToRaise);
                if (defaultEmailPreference != null)
                {
                    logger.LogInformation("Contract Expiry: Order {CallOffId}. Raising {EventToRaise}", order.CallOffId, eventToRaise);
                    await contractExpiryService.RaiseExpiry(today, order, eventToRaise, defaultEmailPreference);
                }
                else
                {
                    logger.LogWarning("Contract Expiry: Order {CallOffId}. {EventToRaise} not found or a ManagedEmailPreference is not configuired", order.CallOffId, eventToRaise);
                }
            }
            else
            {
                logger.LogInformation("Contract Expiry: Order {CallOffId}. No event to raise", order.CallOffId);
            }
        }
    }
}
