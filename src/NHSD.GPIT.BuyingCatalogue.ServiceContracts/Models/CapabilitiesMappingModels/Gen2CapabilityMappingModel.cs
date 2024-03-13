using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2CapabilityMappingModel
{
    public Gen2CapabilityMappingModel(
        string capabilityReference,
        IEnumerable<string> epics)
    {
        if (!int.TryParse(capabilityReference.AsSpan()[1..], out var parsedCapabilityId))
            throw new ArgumentException("Not a valid Capability Reference", nameof(capabilityReference));

        CapabilityId = parsedCapabilityId;
        Epics = epics.Distinct().ToList();
    }

    public int CapabilityId { get; set; }

    public string CapabilityRef => $"C{CapabilityId}";

    public ICollection<string> Epics { get; set; }
}
