using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Notifications.ContractExpiry.Interfaces;
using BuyingCatalogueFunction.Notifications.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace BuyingCatalogueFunction.Notifications.ContractExpiry
{
    public class ContractExpiryFunction
    {
        private readonly ILogger<ContractExpiryFunction> logger;
        private readonly IContractExpiryService contractExpiryService;
        private readonly IEmailPreferenceService emailPreferenceService;

        public ContractExpiryFunction(
            ILogger<ContractExpiryFunction> logger,
            IContractExpiryService contractExpiryService,
            IEmailPreferenceService emailPreferenceService)
        {
            this.logger = logger;
            this.contractExpiryService = contractExpiryService;
            this.emailPreferenceService = emailPreferenceService;
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

            var defaultEmailPreference = await emailPreferenceService.GetDefaultEmailPreference(EmailPreferenceTypeEnum.ContractExpiry);
            if (defaultEmailPreference == null)
            {
                logger.LogWarning(
                    "Contract Expiry: {EmailPreferenceType} not found or a ManagedEmailPreference is not configured",
                    EmailPreferenceTypeEnum.ContractExpiry);

                return;
            }

            List<Order> orders = await contractExpiryService.GetOrdersNearingExpiry(today);

            if (orders.Count == 0)
            {
                logger.LogInformation("Contract Expiry: No orders found nearing expiry for {Date}", today);
                return;
            }

            foreach (var order in orders)
            {
                try
                {
                    await EvaluateOrder(today, order, defaultEmailPreference);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Contract Expiry: Order {CallOffId}. Check inner exception", order.CallOffId);
                }
            }
        }

        private async Task EvaluateOrder(DateTime today, Order order, EmailPreferenceType defaultEmailPreference)
        {
            var eventToRaise = order.EndDate.DetermineEventToRaise(today, order.ContractOrderNumber.OrderEvents);
            if (eventToRaise == OrderExpiryEventTypeEnum.Nothing)
            {
                logger.LogInformation("Contract Expiry: Order {CallOffId}. No event to raise", order.CallOffId);
                return;
            }

            if (defaultEmailPreference.SupportedEventTypes.All(x => x.Id != (int)eventToRaise))
            {
                logger.LogWarning(
                    "Contract Expiry: Order {CallOffId}. Mismatched email preference {EmailPreferenceType} and event {EventToRaise} types",
                    order.CallOffId, defaultEmailPreference.Id, eventToRaise);

                return;
            }

            logger.LogInformation("Contract Expiry: Order {CallOffId}. Raising {EventToRaise}", order.CallOffId,
                eventToRaise);

            await contractExpiryService.RaiseExpiry(today, order, eventToRaise, defaultEmailPreference);
        }
    }
}
