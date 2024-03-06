using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2MappingModel
{
    public Gen2MappingModel(
        IEnumerable<Gen2CapabilitiesCsvModel> capabilitiesImport,
        ICollection<Gen2EpicsCsvModel> epicsImport)
    {
        ArgumentNullException.ThrowIfNull(capabilitiesImport);
        ArgumentNullException.ThrowIfNull(epicsImport);

        var groupedSolutions = capabilitiesImport.GroupBy(x => x.SolutionId);

        foreach (var groupedSolutionCapabilities in groupedSolutions)
        {
            var solutionSpecificCapabilities = groupedSolutionCapabilities
                .Where(x => string.IsNullOrWhiteSpace(x.AdditionalServiceId))
                .ToList();

            var additionalServices = MapAdditionalServices(
                solutionSpecificCapabilities,
                groupedSolutionCapabilities,
                epicsImport);

            var solution = new Gen2SolutionMappingModel(
                groupedSolutionCapabilities.Key,
                MapSolutionCapabilities(solutionSpecificCapabilities, epicsImport).ToList(),
                additionalServices.ToList());

            Solutions.Add(solution);
        }
    }

    public IList<Gen2SolutionMappingModel> Solutions { get; set; } = new List<Gen2SolutionMappingModel>();

    private static IEnumerable<Gen2CatalogueItemMappingModel> MapAdditionalServices(
        IEnumerable<Gen2CapabilitiesCsvModel> solutionCapabilities,
        IEnumerable<Gen2CapabilitiesCsvModel> groupedSolutionCapabilities,
        IEnumerable<Gen2EpicsCsvModel> epicsImport) => groupedSolutionCapabilities.Except(solutionCapabilities)
        .GroupBy(x => $"{x.SolutionId}{x.AdditionalServiceId}")
        .Select(
            x => new Gen2CatalogueItemMappingModel(
                x.Key,
                x.Select(
                    y => new Gen2CapabilityMappingModel(
                        y.CapabilityId,
                        epicsImport.Where(
                                z => !string.IsNullOrWhiteSpace(z.AdditionalServiceId)
                                    && string.Equals(z.SolutionId, y.SolutionId)
                                    && string.Equals(z.AdditionalServiceId, y.AdditionalServiceId)
                                    && string.Equals(z.CapabilityId, y.CapabilityId))
                            .Select(z => z.EpicId)))));

    private static IEnumerable<Gen2CapabilityMappingModel> MapSolutionCapabilities(
        IEnumerable<Gen2CapabilitiesCsvModel> solutionCapabilities,
        IEnumerable<Gen2EpicsCsvModel> epicsImport) => solutionCapabilities.Select(
        x => new Gen2CapabilityMappingModel(
            x.CapabilityId,
            epicsImport.Where(
                    z => string.IsNullOrWhiteSpace(z.AdditionalServiceId)
                        && string.Equals(z.SolutionId, x.SolutionId)
                        && string.Equals(z.CapabilityId, x.CapabilityId))
                .Select(z => z.EpicId)
                .ToList()));
}
