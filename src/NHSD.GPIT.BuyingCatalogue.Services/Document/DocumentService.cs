using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;

namespace NHSD.GPIT.BuyingCatalogue.Services.Document
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogWrapper<DocumentService> logger;
        private readonly IAzureBlobDocumentRepository documentRepository;

        public DocumentService(
            ILogWrapper<DocumentService> logger,
            IAzureBlobDocumentRepository documentRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        }

        public async Task<FileStreamResult> DownloadDocumentAsync(string name)
        {
            try
            {
                logger.LogInformation($"Attempting to download document '{name}'");
                var downloadInfo = await documentRepository.DownloadAsync(name);
                return new FileStreamResult(downloadInfo.Content, downloadInfo.ContentType);
            }
            catch (DocumentRepositoryException e)
            {
                logger.LogError(e, null);
                return null;
            }
        }

        public async Task<FileStreamResult> DownloadSolutionDocumentAsync(string id, string name)
        {
            try
            {
                logger.LogInformation($"Attempting to download document '{id} {name}'");
                var downloadInfo = await documentRepository.DownloadAsync(id, name);
                return new FileStreamResult(downloadInfo.Content, downloadInfo.ContentType);
            }
            catch (DocumentRepositoryException e)
            {
                logger.LogError(e, null);
                return null;
            }
        }

        public IAsyncEnumerable<string> GetDocumentsBySolutionId(string id)
        {
            logger.LogInformation($"Getting document list for {id}");
            return documentRepository.GetFileNamesAsync(id);
        }
    }
}
