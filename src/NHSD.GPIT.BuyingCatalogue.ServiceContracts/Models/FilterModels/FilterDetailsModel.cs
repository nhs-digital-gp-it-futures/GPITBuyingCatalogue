﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

public class FilterDetailsModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string FrameworkName { get; set; }

    public bool Invalid { get; set; }

    public List<HostingType> HostingTypes { get; set; } = Enumerable.Empty<HostingType>().ToList();

    public List<ApplicationType> ApplicationTypes { get; set; } = Enumerable.Empty<ApplicationType>().ToList();

    public List<InteropIntegrationType> InteropIntegrationTypes { get; set; } = Enumerable.Empty<InteropIntegrationType>().ToList();

    public List<InteropIm1IntegrationType> InteropIm1IntegrationsTypes { get; set; } = Enumerable.Empty<InteropIm1IntegrationType>().ToList();

    public List<InteropGpConnectIntegrationType> InteropGpConnectIntegrationsTypes { get; set; } = Enumerable.Empty<InteropGpConnectIntegrationType>().ToList();

    public List<KeyValuePair<string, List<string>>> Capabilities { get; set; }
}
