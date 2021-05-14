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
        private readonly ILogWrapper<DocumentService> _logger;
        private readonly IAzureBlobDocumentRepository _documentRepository;

        public DocumentService(ILogWrapper<DocumentService> logger,
            IAzureBlobDocumentRepository documentRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        }

        public async Task<FileStreamResult> DownloadDocumentAsync(string name)
        {
            IDocument downloadInfo;

            try
            {
                downloadInfo = await _documentRepository.DownloadAsync(name);
            }
            catch (DocumentRepositoryException e)
            {
                _logger.LogError(e, null);
                return null; // TODO - handle error in new way
                //return StatusCode(e.HttpStatusCode);
            }

            return new FileStreamResult(downloadInfo.Content, downloadInfo.ContentType);
        }

        public async Task<FileStreamResult> DownloadSolutionDocumentAsync(string id, string name)
        {
            IDocument downloadInfo;

            try
            {
                downloadInfo = await _documentRepository.DownloadAsync(id, name);
            }
            catch (DocumentRepositoryException e)
            {
                _logger.LogError(e, null);

                //return StatusCode(e.HttpStatusCode);
                return null; // TODO - handle error in new way
            }

            return new FileStreamResult(downloadInfo.Content, downloadInfo.ContentType);
        }

        public IAsyncEnumerable<string> GetDocumentsBySolutionId(string id)
        {
            return _documentRepository.GetFileNamesAsync(id);
        }
    }
}
