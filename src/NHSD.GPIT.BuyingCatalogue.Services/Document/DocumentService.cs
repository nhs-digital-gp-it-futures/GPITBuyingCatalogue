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
            try
            {
                var downloadInfo = await _documentRepository.DownloadAsync(name);
                return new FileStreamResult(downloadInfo.Content, downloadInfo.ContentType);
            }
            catch (DocumentRepositoryException e)
            {
                _logger.LogError(e, null);
                return null;                 
            }            
        }

        public async Task<FileStreamResult> DownloadSolutionDocumentAsync(string id, string name)
        {            
            try
            {
                var downloadInfo = await _documentRepository.DownloadAsync(id, name);
                return new FileStreamResult(downloadInfo.Content, downloadInfo.ContentType);
            }
            catch (DocumentRepositoryException e)
            {
                _logger.LogError(e, null);         
                return null;
            }            
        }

        public IAsyncEnumerable<string> GetDocumentsBySolutionId(string id)
        {
            return _documentRepository.GetFileNamesAsync(id);
        }
    }
}
