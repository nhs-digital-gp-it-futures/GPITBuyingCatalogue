using System;
using System.Text.Json.Serialization;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Models
{
    public record CapabilityIdCsv
    {
        public int Value { get; init; }

        [JsonConstructor]
        public CapabilityIdCsv(int value)
        {
            Value = value;
        }

        public CapabilityIdCsv(string capabilityId)
        {
            ArgumentNullException.ThrowIfNull(capabilityId);

            var strippedCapabilityId = capabilityId.Replace("C", string.Empty);
            if (!int.TryParse(strippedCapabilityId, out var parsedCapabilityId))
                throw new FormatException($"Invalid Capability ID format {capabilityId}");

            Value = parsedCapabilityId;
        }
    }
}
