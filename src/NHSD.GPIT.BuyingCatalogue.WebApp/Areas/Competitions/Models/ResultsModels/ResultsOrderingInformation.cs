using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

[ExcludeFromCodeCoverage]
public class ResultsOrderingInformation
{
    public ResultsOrderingInformation(
        bool hasMultipleWinners, string pdfUrl)
    {
        HasMultipleWinners = hasMultipleWinners;
        PdfUrl = pdfUrl;
    }

    public bool HasMultipleWinners { get; set; }

    public string PdfUrl { get; set; }
}
