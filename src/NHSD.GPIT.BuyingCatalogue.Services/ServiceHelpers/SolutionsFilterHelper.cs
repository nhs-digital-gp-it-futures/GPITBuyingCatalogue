using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers
{
    public static class SolutionsFilterHelper
    {
        public static ICollection<int> ParseCapabilityIds(string capabilityIds) =>
            capabilityIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList() ?? new List<int>();

        public static Dictionary<int, string[]> ParseCapabilityAndEpicIds(string capabilityAndEpicsFilterString)
        {
            var capabilityAndEpics = capabilityAndEpicsFilterString?
                .Split(FilterConstants.GroupDelimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            return new Dictionary<int, string[]>(capabilityAndEpics
                .Select(x => x.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries))
                .Where(x => int.TryParse(x[0], out _))
                .Select(x => new KeyValuePair<int, string[]>(int.Parse(x[0], CultureInfo.InvariantCulture), x.Skip(1).ToArray())));
        }

        public static ICollection<ApplicationType> ParseApplicationTypeIds(string applicationTypeIds) =>
            applicationTypeIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(ApplicationType), x, out var catValue) && Enum.IsDefined(typeof(ApplicationType), catValue))
                .Select(t => (ApplicationType)Enum.Parse(typeof(ApplicationType), t))
                .ToList() ?? new List<ApplicationType>();

        public static ICollection<HostingType> ParseHostingTypeIds(string hostingTypeIds) =>
            hostingTypeIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(HostingType), x, out var hostingValue) && Enum.IsDefined(typeof(HostingType), hostingValue))
                .Select(t => (HostingType)Enum.Parse(typeof(HostingType), t))
                .ToList() ?? new List<HostingType>();

        public static ICollection<InteropIm1IntegrationType> ParseInteropIm1IntegrationsIds(string interopIm1IntegrationsIds) =>
            interopIm1IntegrationsIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(InteropIm1IntegrationType), x, out var hostingValue) && Enum.IsDefined(typeof(InteropIm1IntegrationType), hostingValue))
                .Select(t => (InteropIm1IntegrationType)Enum.Parse(typeof(InteropIm1IntegrationType), t))
                .ToList() ?? new List<InteropIm1IntegrationType>();

        public static ICollection<InteropGpConnectIntegrationType> ParseInteropGpConnectIntegrationsIds(string selectedGPConnectIntegrationsIds) =>
            selectedGPConnectIntegrationsIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(InteropGpConnectIntegrationType), x, out var hostingValue) && Enum.IsDefined(typeof(InteropGpConnectIntegrationType), hostingValue))
                .Select(t => (InteropGpConnectIntegrationType)Enum.Parse(typeof(InteropGpConnectIntegrationType), t))
                .ToList() ?? new List<InteropGpConnectIntegrationType>();

        public static ICollection<InteropIntegrationType> ParseInteropIntegrationTypeIds(string selectedInteroperabilityIds) =>
            selectedInteroperabilityIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(InteropIntegrationType), x, out var hostingValue) && Enum.IsDefined(typeof(InteropIntegrationType), hostingValue))
                .Select(t => (InteropIntegrationType)Enum.Parse(typeof(InteropIntegrationType), t))
                .ToList() ?? new List<InteropIntegrationType>();
    }
}
