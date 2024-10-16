﻿using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionOrderService
{
    Task<CallOffId> CreateDirectAwardOrder(string internalOrgId, int competitionId, CatalogueItemId solutionId);

    Task<CallOffId> CreateOrder(string internalOrgId, int competitionId, CatalogueItemId solutionId);
}
