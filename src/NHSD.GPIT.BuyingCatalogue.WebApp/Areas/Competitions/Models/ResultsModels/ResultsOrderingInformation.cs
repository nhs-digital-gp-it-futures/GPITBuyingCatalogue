﻿using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

[ExcludeFromCodeCoverage]
public class ResultsOrderingInformation
{
    public ResultsOrderingInformation(
        bool hasMultipleWinners, bool isDirectAward, string pdfUrl, string internalOrgId, int competitionId)
    {
        HasMultipleWinners = hasMultipleWinners;
        IsDirectAward = isDirectAward;
        PdfUrl = pdfUrl;
        InternalOrgId = internalOrgId;
        CompetitionId = competitionId;
    }

    public bool HasMultipleWinners { get; set; }

    public bool IsDirectAward { get; set; }

    public string PdfUrl { get; set; }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }
}
