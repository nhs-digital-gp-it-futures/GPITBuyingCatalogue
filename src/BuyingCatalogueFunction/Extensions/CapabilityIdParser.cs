using System;

namespace BuyingCatalogueFunction.Extensions;

public static class CapabilityExtensions
{
    public static int ParseCapabilityId(string capabilityId)
    {
        var strippedCapabilityId = capabilityId.Replace("C", string.Empty);
        if (!int.TryParse(strippedCapabilityId, out var parsedCapabilityId))
            throw new FormatException($"Invalid Capability ID format {capabilityId}");

        return parsedCapabilityId;
    }
}
