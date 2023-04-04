using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal static class OdsOrganisationsSeedData
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const string GpRoleType = "RO177";
        private const string IcbRoleType = "RO318";
        private const string SubLocationRoleType = "RO319";

        private const string CommissionedByRelationshipType = "RE4";
        private const string LocatedInRelationshipType = "RE5";

        internal static void Initialize(BuyingCatalogueDbContext context)
        {
            AddDefaultData(context);
        }

        private static void AddDefaultData(BuyingCatalogueDbContext context)
        {
            var relationshipTypes = new List<RelationshipType>
            {
                new() { Id = CommissionedByRelationshipType, Description = "IS COMMISSIONED BY" },
                new() { Id = LocatedInRelationshipType, Description = "IS LOCATED IN THE GEOGRAPHY OF" },
            };

            context.AddRange(relationshipTypes);
            context.SaveChanges();

            List<RoleType> roles = new()
            {
                new() { Id = "RO177", Description = "PRESCRIBING COST CENTRE" },
                new() { Id = "RO213", Description = "COMMISSIONING SUPPORT UNIT" },
                new() { Id = IcbRoleType, Description = "INTEGRATED CARE BOARD" },
                new() { Id = SubLocationRoleType, Description = "SUB ICB LOCATION" },
            };

            context.InsertRangeWithIdentity(roles);
            context.SaveChanges();

            context.AddRange(roles);
            context.SaveChanges();

                Roles = new HashSet<OrganisationRole>
                {
                    new OrganisationRole { OrganisationId = "Y03508", RoleId = "RO177", IsPrimaryRole = true, },
                },
            };

            var gpPracticeOrganisation2 = new OdsOrganisation
            {
                Id = "Y07021",
                Name = "BEVAN LIMITED",
                AddressLine1 = "BRANSHOLME HEALTH CENTRE",
                AddressLine2 = "GOODHART ROAD",
                Town = "HULL",
                Postcode = "HU7 4DW",
                Country = "ENGLAND",
                IsActive = true,

                Roles = new HashSet<OrganisationRole>
                {
                    new OrganisationRole { OrganisationId = "Y07021", RoleId = "RO177", IsPrimaryRole = true, },
                },
            };

            var bobOdsOrganisation = new OdsOrganisation
            {
                Id = "X26",
                Name = "NHS ENGLAND - X26",
                AddressLine1 = "THE LEEDS GOVERNMENT HUB",
                AddressLine2 = "7-8 WELLINGTON PLACE",
                Town = "LEEDS",
                Postcode = "LS1 4AP",
                Country = "ENGLAND",
                IsActive = true,
            };

            var sueOdsOrganisation = new OdsOrganisation
            {
                Id = "03F",
                Name = "NHS HUMBER AND NORTH YORKSHIRE ICB - 03F",
                AddressLine1 = "WILBERFORCE COURT",
                AddressLine2 = "ALFRED GELDER STREET",
                Town = "HULL",
                Postcode = "HU1 1UY",
                Country = "ENGLAND",
                IsActive = true,
                Roles = new HashSet<OrganisationRole> { new() { RoleId = IcbRoleType, IsPrimaryRole = false } },
                Related = GetSubLocationOrgs()
                    .Select(
                        x => new OrganisationRelationship
                {
                            RelationshipTypeId = LocatedInRelationshipType, TargetOrganisation = x,
                        })
                    .ToList(),
            };

            var aliceOdsOrganisation = new OdsOrganisation
            {
                Id = "15F",
                Name = "NHS WEST YORKSHIRE ICB - 15F",
                AddressLine1 = "SUITES 2 - 4 WIRA HOUSE",
                AddressLine2 = "RING ROAD",
                Town = "LEEDS",
                Postcode = "LS16 6EB",
                Country = "ENGLAND",
                IsActive = true,
                Roles = new HashSet<OrganisationRole> { new() { RoleId = IcbRoleType, IsPrimaryRole = false } },
                Related = GetSubLocationOrgs()
                    .Select(
                        x => new OrganisationRelationship
                {
                            RelationshipTypeId = LocatedInRelationshipType, TargetOrganisation = x,
                        })
                    .ToList(),
            };

            var daveOdsOrganisation = new OdsOrganisation
            {
                Id = "15H",
                Name = "SOUTH WEST NORTH COMMISSIONING HUB",
                AddressLine1 = "NHS ENGLAND",
                AddressLine2 = "QUARRY HOUSE",
                Town = "LEEDS",
                Postcode = "LS2 7UE",
                Country = "ENGLAND",
                IsActive = true,
                Roles = new HashSet<OrganisationRole> { new() { RoleId = IcbRoleType, IsPrimaryRole = false } },
                Related = GetSubLocationOrgs()
                    .Select(
                        x => new OrganisationRelationship
                {
                            RelationshipTypeId = LocatedInRelationshipType, TargetOrganisation = x,
                        })
                    .ToList(),
            };

            var sueChildIcb = sueOdsOrganisation.Related.First().TargetOrganisation;
            sueChildIcb.Related.AddRange(
                GetKnownOrgs()
                    .Select(
                        x => new OrganisationRelationship
                {
                            TargetOrganisation = x, RelationshipTypeId = CommissionedByRelationshipType,
                        }));

            // Organisations
            List<OdsOrganisation> organisations = new()
            {
                bobOdsOrganisation,
                sueOdsOrganisation,
                aliceOdsOrganisation,
                daveOdsOrganisation,
            };

            context.AddRange(organisations);
            context.SaveChanges();
        }

        private static IEnumerable<OdsOrganisation> GetKnownOrgs() => new List<OdsOrganisation>
        {
            new()
            {
                Id = "Y03508",
                Name = "ASSURA ER LLP HULL DERMATOLOGY",
                AddressLine1 = "PARK PRIMARY HEALTH CARE CENTRE",
                AddressLine2 = "700 HOLDERNESS ROAD",
                Town = "HULL",
                Postcode = "HU9 3JA",
                IsActive = true,
                Roles = new HashSet<OrganisationRole> { new() { RoleId = GpRoleType, IsPrimaryRole = false } },
            },
            new()
            {
                Id = "Y07021",
                Name = "BEVAN LIMITED",
                AddressLine1 = "BRANSHOLME HEALTH CENTRE",
                AddressLine2 = "GOODHART ROAD",
                Town = "HULL",
                Postcode = "HU7 4DW",
                IsActive = true,
                Roles = new HashSet<OrganisationRole> { new() { RoleId = GpRoleType, IsPrimaryRole = false } },
            },
        };

        private static IEnumerable<OdsOrganisation> GetSubLocationOrgs()
        {
            static IEnumerable<OdsOrganisation> GetChildOrgs()
            {
                var recipients = Enumerable.Range(0, 20)
                    .Select(
                        x => new OdsOrganisation()
                        {
                            Id = GetRandomOrgId(),
                            Name = $"RECIPIENT {x}",
                            AddressLine1 = "RECIPIENT TOWN",
                            AddressLine2 = "RECIPIENT ROAD",
                            Postcode = "SR 1SR",
                            Country = "England",
                            IsActive = true,
                            Roles = new HashSet<OrganisationRole>
                            {
                                new() { RoleId = GpRoleType, IsPrimaryRole = false },
                            },
                        });

                return recipients;
            }


            return Enumerable.Range(0, 3)
                .Select(
                    x => new OdsOrganisation
                    {
                        Id = GetRandomOrgId(),
                        Name = $"SUB-LOCATION {x}",
                        AddressLine1 = "93 TEST TOWN",
                        AddressLine2 = "TEST ROAD",
                        Postcode = "X9 9LF",
                        Country = "ENGLAND",
                        IsActive = true,
                        Roles = new HashSet<OrganisationRole>
                        {
                            new() { RoleId = SubLocationRoleType, IsPrimaryRole = false },
                        },
                        Related = GetChildOrgs()
                            .Select(
                                x => new OrganisationRelationship
                                {
                                    TargetOrganisation = x, RelationshipTypeId = CommissionedByRelationshipType,
                                })
                            .ToList(),
                    });
        }

        private static string GetRandomOrgId() =>
            string.Join(string.Empty, Guid.NewGuid().ToString().Take(8));
    }
}
