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

            logger.LogInformation($"Contract Expiry: Executed at { utcNow }");
            
            if (timerInfo.ScheduleStatus is not null)
            {
                logger.LogInformation($"Contract Expiry: Next timer schedule at { timerInfo.ScheduleStatus.Next }");
            }

            await RunFor(utcNow.Date);
        }

        private async Task RunFor(DateTime today)
        {
            logger.LogInformation($"Contract Expiry: Evaluating Orders for {today}");
            List<Order> orders = await contractExpiryService.GetOrdersNearingExpiry(today);

            if (!orders.Any())
            {
                logger.LogInformation($"Contract Expiry: No orders found nearing expiry for {today}");
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
                    logger.LogError(e, $"Contract Expiry: Order {order.CallOffId}. Check inner exception");
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
                    logger.LogInformation($"Contract Expiry: Order {order.CallOffId}. Raising {eventToRaise}");
                    await contractExpiryService.RaiseExpiry(today, order, eventToRaise, defaultEmailPreference);
                }
                else
                {
                    logger.LogWarning($"Contract Expiry: Order {order.CallOffId}. {eventToRaise} not found or a ManagedEmailPreference is not configuired");
                }
            }
            else
            {
                logger.LogInformation($"Contract Expiry: Order {order.CallOffId}. No event to raise");
            }
        }
    }
}
