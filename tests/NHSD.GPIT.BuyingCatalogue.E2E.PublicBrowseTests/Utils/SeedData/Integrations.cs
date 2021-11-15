using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    public static class Integrations
    {
        public static List<Integration> GetIntegrations => new()
        {
            new Integration
            {
                Id = Guid.Parse("63762376-e1e6-4e9c-873b-82fe8802be88"),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
                Description = "Test IM1 Integration",
                IntegratesWith = "Something to integrate with",
                IsConsumer = true,
                Qualifier = Im1IntegrationTypes[0],
                AdditionalInformation = Strings.RandomString(100),
            },
            new Integration
            {
                Id = Guid.Parse("74c9bdfd-9975-41d9-8177-faa7b9ed6392"),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
                Description = "Test IM1 Integration 2",
                IntegratesWith = "Something else to integrate with",
                IsConsumer = true,
                Qualifier = Im1IntegrationTypes[1],
                AdditionalInformation = Strings.RandomString(100),
            },
            new Integration
            {
                Id = Guid.Parse("bb20cc63-bf51-44c1-8a1e-55285c199855"),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
                Description = "Test IM1 Integration 3",
                IntegratesWith = "Something further to integrate with",
                IsConsumer = false,
                Qualifier = Im1IntegrationTypes[2],
                AdditionalInformation = Strings.RandomString(100),
            },
            new Integration
            {
                Id = Guid.Parse("7933ca05-7e2a-44f5-8a2c-c2315ff41a95"),
                IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType,
                Description = "Test GPC Integration",
                IntegratesWith = "Something to integrate with",
                IsConsumer = true,
                Qualifier = GpcIntegrationTypes[0],
                AdditionalInformation = Strings.RandomString(100),
            },
            new Integration
            {
                Id = Guid.Parse("8c582ed2-45a9-448d-b9b1-3f9596557992"),
                IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType,
                Description = "Test GPC Integration 2",
                IntegratesWith = "Something else to integrate with",
                IsConsumer = true,
                Qualifier = GpcIntegrationTypes[1],
                AdditionalInformation = Strings.RandomString(100),
            },
            new Integration
            {
                Id = Guid.Parse("e1e68aa6-b7ab-414a-934f-91692088749c"),
                IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType,
                Description = "Test GPC Integration 3",
                IntegratesWith = "Something further to integrate with",
                IsConsumer = false,
                Qualifier = GpcIntegrationTypes[2],
                AdditionalInformation = Strings.RandomString(100),
            },
        };

        private static List<string> Im1IntegrationTypes => new()
        {
            "Bulk",
            "Transactional",
            "Patient Facing",
        };

        private static List<string> GpcIntegrationTypes => new()
        {
            "HTML View",
            "Appointment Booking",
            "Structured Record",
        };
    }
}
