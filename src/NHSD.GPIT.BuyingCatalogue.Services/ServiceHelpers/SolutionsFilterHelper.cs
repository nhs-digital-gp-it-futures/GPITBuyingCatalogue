﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public static ICollection<string> ParseEpicIds(string epicIds) =>
            epicIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
        .ToList() ?? new List<string>();

        public static ICollection<ClientApplicationType> ParseClientApplicationTypeIds(string clientApplicationTypeIds) =>
            clientApplicationTypeIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(ClientApplicationType), x, out var catValue) && Enum.IsDefined(typeof(ClientApplicationType), catValue))
                .Select(t => (ClientApplicationType)Enum.Parse(typeof(ClientApplicationType), t))
                .ToList() ?? new List<ClientApplicationType>();

        public static ICollection<HostingType> ParseHostingTypeIds(string hostingTypeIds) =>
            hostingTypeIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(HostingType), x, out var hostingValue) && Enum.IsDefined(typeof(HostingType), hostingValue))
                .Select(t => (HostingType)Enum.Parse(typeof(HostingType), t))
                .ToList() ?? new List<HostingType>();
    }
}
