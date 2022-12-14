using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;

internal static class ServiceLevelAgreementSeedData
{
    internal static List<ServiceLevelAgreements> GetServiceLevelAgreements()
    {
        return new List<ServiceLevelAgreements>
        {
            new ServiceLevelAgreements()
            {
                SolutionId = new(99999, "001"),
                SlaType = SlaType.Type1,
            },
            new ServiceLevelAgreements()
            {
                SolutionId = new(99998, "001"),
                SlaType = SlaType.Type1,
            },
            new ServiceLevelAgreements()
            {
                SolutionId = new(99998, "002"),
                SlaType = SlaType.Type1,
            },
        };
    }

    internal static List<SlaContact> GetServiceLevelContacts()
    {
        return new List<SlaContact>
        {
            new SlaContact()
            {
                Id = 1,
                SolutionId = new CatalogueItemId(99998, "001"),
                Channel = "This is a Channel",
                ContactInformation = "This is Contact Information",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            },
            new SlaContact()
            {
                Id = 2,
                SolutionId = new CatalogueItemId(99998, "002"),
                Channel = "This is a Channel",
                ContactInformation = "This is Contact Information",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            },
            new SlaContact()
            {
                Id = 3,
                SolutionId = new CatalogueItemId(99998, "002"),
                Channel = "This is a Channel 2",
                ContactInformation = "This is Contact Information 2",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            },
            new SlaContact()
            {
                Id = 99,
                SolutionId = new CatalogueItemId(99999, "001"),
                Channel = "This is a Channel",
                ContactInformation = "This is Contact Information",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            },
        };
    }

    internal static List<SlaServiceLevel> GetServiceLevels()
    {
        return new List<SlaServiceLevel>
        {
            new()
            {
                Id = 1,
                SolutionId = new(99998, "001"),
                TypeOfService = "Type of Service 01",
                ServiceLevel = "Service level 01",
                HowMeasured = "How Measured 01",
                ServiceCredits = true,
            },
            new()
            {
                Id = 2,
                SolutionId = new(99998, "002"),
                TypeOfService = "Type of Service 02",
                ServiceLevel = "Service level 02",
                HowMeasured = "How Measured 02",
                ServiceCredits = true,
            },
            new()
            {
                Id = 3,
                SolutionId = new(99998, "002"),
                TypeOfService = "Type of Service 03",
                ServiceLevel = "Service level 03",
                HowMeasured = "How Measured 03",
                ServiceCredits = true,
            },
            new()
            {
                Id = 99,
                SolutionId = new(99999, "001"),
                HowMeasured = "Hourly",
                ServiceLevel = "Level",
                TypeOfService = "Type",
                ServiceCredits = true,
            },
        };
    }

    internal static List<ServiceAvailabilityTimes> GetServiceAvailabilityTimes()
    {
        return new List<ServiceAvailabilityTimes>
        {
            new()
            {
                Id = 1,
                ApplicableDays = "Applicable Days 01",
                Category = "Support Type 01",
                TimeFrom = DateTime.UtcNow.AddHours(-5),
                TimeUntil = DateTime.UtcNow,
                SolutionId = new CatalogueItemId(99998, "001"),
            },
            new()
            {
                Id = 2,
                ApplicableDays = "Applicable Days 02",
                Category = "Support Type 02",
                TimeFrom = DateTime.UtcNow.AddHours(-5),
                TimeUntil = DateTime.UtcNow,
                SolutionId = new CatalogueItemId(99998, "002"),
            },
            new()
            {
                Id = 3,
                ApplicableDays = "Applicable Days 03",
                Category = "Support Type 03",
                TimeFrom = DateTime.UtcNow.AddHours(-5),
                TimeUntil = DateTime.UtcNow,
                SolutionId = new CatalogueItemId(99998, "002"),
            },
            new()
            {
                Id = 99,
                ApplicableDays = "Applicable Days 01",
                Category = "Support Type 01",
                TimeFrom = DateTime.UtcNow.AddHours(-5),
                TimeUntil = DateTime.UtcNow,
                SolutionId = new CatalogueItemId(99999, "001"),
            },
        };
    }
}
