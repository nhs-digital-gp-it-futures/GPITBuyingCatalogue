using System;
using System.Threading.Tasks;
using System.Web.Http;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.Functions
{
    public class IncrementalUpdateFunctions
    {
        private readonly ILogger<IncrementalUpdateFunctions> _logger;
        private readonly IIncrementalUpdateService _incrementalUpdateService;

        public IncrementalUpdateFunctions(
            ILogger<IncrementalUpdateFunctions> logger,
            IIncrementalUpdateService incrementalUpdateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _incrementalUpdateService = incrementalUpdateService ?? throw new ArgumentNullException(nameof(incrementalUpdateService));
        }

        [FunctionName(nameof(IncrementalUpdateHttpTrigger))]
        public async Task<IActionResult> IncrementalUpdateHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest request)
        {
            _logger.LogInformation("HTTP-triggered incremental update starting at {Date}", DateTime.Now);

            try
            {
                await _incrementalUpdateService.UpdateOrganisationData();

                _logger.LogInformation("HTTP-triggered incremental update completed at {Date}", DateTime.Now);

                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred while processing request. {Exception}", e);

                return new InternalServerErrorResult();
            }
        }

        [FunctionName(nameof(IncrementalUpdateTimerTrigger))]
        public async Task IncrementalUpdateTimerTrigger(
            [TimerTrigger("0 0 4 * * *")] TimerInfo info)
        {
            _logger.LogInformation("Timer-triggered incremental update starting at {Date}", DateTime.Now);

            try
            {
                await _incrementalUpdateService.UpdateOrganisationData();

                _logger.LogInformation("Timer-triggered incremental update completed at {Date}", DateTime.Now);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred while processing request. {Exception}", e);
            }
        }
    }
}
