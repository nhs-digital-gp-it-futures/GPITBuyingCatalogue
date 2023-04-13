using System;
using System.Net;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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

        [Function(nameof(IncrementalUpdateHttpTrigger))]
        public async Task<HttpResponseData> IncrementalUpdateHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("HTTP-triggered incremental update starting at {Date}", DateTime.Now);

            HttpResponseData response;

            try
            {
                await _incrementalUpdateService.UpdateOrganisationData();

                _logger.LogInformation("HTTP-triggered incremental update completed at {Date}", DateTime.Now);

                response = req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred while processing request. {Exception}", e);

                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [Function(nameof(IncrementalUpdateTimerTrigger))]
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
