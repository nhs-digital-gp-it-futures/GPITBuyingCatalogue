using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Services;

public class StubbedGpPracticeProvider : IGpPracticeProvider
{
    private static Random random = new Random();

    public Task<IEnumerable<GpPractice>> GetGpPractices(Uri csvUri)
        => Task.FromResult(ServiceRecipientsSeedData.GetServiceRecipients.Select(
                x => new GpPractice
                {
                    CODE = x.OrgId, EXTRACT_DATE = DateTime.UtcNow, NUMBER_OF_PATIENTS = random.Next(int.MaxValue),
                }));
}
