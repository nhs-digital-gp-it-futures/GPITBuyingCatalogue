using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    public static class ServiceRecipientsSeedData
    {
        public static List<ServiceRecipient> GetServiceRecipients =>
            new List<ServiceRecipient>
            {
                new()
                {
                   Name = "Test Service Recipient One",
                   OrgId = "Y12345",
                   PrimaryRoleId = "RO177",
                   Status = "Active",
                },
                new()
                {
                    Name = "Test Service Recipient Two",
                    OrgId = "Y98765",
                    PrimaryRoleId = "RO177",
                    Status = "Active",
                },
                new()
                {
                    Name = "Test Service Recipient Three",
                    OrgId = "Y45678",
                    PrimaryRoleId = "RO177",
                    Status = "Active",
                },
            };
    }
}
