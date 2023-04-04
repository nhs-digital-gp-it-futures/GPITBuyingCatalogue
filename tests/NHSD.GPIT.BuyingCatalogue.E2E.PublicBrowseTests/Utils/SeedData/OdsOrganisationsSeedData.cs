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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal static class OdsOrganisationsSeedData
    {
        internal static void Initialize(BuyingCatalogueDbContext context)
        {
            AddDefaultData(context);
        }

        private static void AddDefaultData(BuyingCatalogueDbContext context)
        {
            List<RelationshipType> relationships = new()
            {
                new RelationshipType() { Id = "RE2", Description = "Test" },
                new RelationshipType() { Id = "RE3", Description = "Test" },
                new RelationshipType() { Id = "RE4", Description = "Test" },
                new RelationshipType() { Id = "RE5", Description = "Test" },
                new RelationshipType() { Id = "RE6", Description = "Test" },
            };
            context.InsertRangeWithIdentity(relationships);

            List<RoleType> roles = new()
            {
                new RoleType() { Id = "RO98", Description = "Test" },
                new RoleType() { Id = "RO218", Description = "Test" },
                new RoleType() { Id = "RO116", Description = "Test" },
                new RoleType() { Id = "RO319", Description = "Test" },
                new RoleType() { Id = "RO177", Description = "Test" },
            };
            context.InsertRangeWithIdentity(roles);

            var gpPracticeOrganisation1 = new OdsOrganisation
            {
                Id = "Y03508",
                Name = "ASSURA ER LLP HULL DERMATOLOGY",
                AddressLine1 = "PARK PRIMARY HEALTH CARE CENTRE",
                AddressLine2 = "700 HOLDERNESS ROAD",
                Town = "HULL",
                Postcode = "HU9 3JA",
                Country = "ENGLAND",
                IsActive = true,

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

                Parents = new HashSet<OrganisationRelationship>
                {
                    new OrganisationRelationship { RelationshipTypeId = "RE3", TargetOrganisationId = "X26", OwnerOrganisationId = "XDH", },
                },

                Related = new HashSet<OrganisationRelationship>
                {
                    new OrganisationRelationship { RelationshipTypeId = "RE6", TargetOrganisationId = "X2601", OwnerOrganisationId = "X26", },
                    new OrganisationRelationship { RelationshipTypeId = "RE2", TargetOrganisationId = "X26011", OwnerOrganisationId = "X26", },
                    new OrganisationRelationship { RelationshipTypeId = "RE4", TargetOrganisationId = "Y03508", OwnerOrganisationId = "X26", },
                    new OrganisationRelationship { RelationshipTypeId = "RE4", TargetOrganisationId = "Y07021", OwnerOrganisationId = "X26", },
                },

                Roles = new HashSet<OrganisationRole>
                {
                    new OrganisationRole { OrganisationId = "X26", RoleId = "RO116", IsPrimaryRole = true, },
                },
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

                Parents = new HashSet<OrganisationRelationship>
                {
                    new OrganisationRelationship { RelationshipTypeId = "RE5", TargetOrganisationId = "03F", OwnerOrganisationId = "QOQ", },
                },

                Related = new HashSet<OrganisationRelationship>
                {
                    new OrganisationRelationship { RelationshipTypeId = "RE6", TargetOrganisationId = "03FAA", OwnerOrganisationId = "03F", },
                    new OrganisationRelationship { RelationshipTypeId = "RE4", TargetOrganisationId = "B81008", OwnerOrganisationId = "03F", },
                    new OrganisationRelationship { RelationshipTypeId = "RE4", TargetOrganisationId = "Y03508", OwnerOrganisationId = "03F", },
                    new OrganisationRelationship { RelationshipTypeId = "RE4", TargetOrganisationId = "Y07021", OwnerOrganisationId = "03F", },
                },

                Roles = new HashSet<OrganisationRole>
                {
                    new OrganisationRole { OrganisationId = "03F", RoleId = "RO98", IsPrimaryRole = true, },
                    new OrganisationRole { OrganisationId = "03F", RoleId = "RO319", IsPrimaryRole = false, },
                },
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

                Parents = new HashSet<OrganisationRelationship>
                {
                    new OrganisationRelationship { RelationshipTypeId = "RE5", TargetOrganisationId = "15F", OwnerOrganisationId = "QWO", },
                },

                Related = new HashSet<OrganisationRelationship>
                {
                    new OrganisationRelationship { RelationshipTypeId = "RE4", TargetOrganisationId = "Y04205", OwnerOrganisationId = "15F", },
                    new OrganisationRelationship { RelationshipTypeId = "RE5", TargetOrganisationId = "VLRE8", OwnerOrganisationId = "15F", },
                },

                Roles = new HashSet<OrganisationRole>
                {
                    new OrganisationRole { OrganisationId = "15F", RoleId = "RO98", IsPrimaryRole = true, },
                    new OrganisationRole { OrganisationId = "15F", RoleId = "RO319", IsPrimaryRole = false, },
                },
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

                Parents = new HashSet<OrganisationRelationship>
                {
                    new OrganisationRelationship { RelationshipTypeId = "RE5", TargetOrganisationId = "15H", OwnerOrganisationId = "Q86", },
                },

                Roles = new HashSet<OrganisationRole>
                {
                    new OrganisationRole { OrganisationId = "15H", RoleId = "RO98", IsPrimaryRole = true, },
                    new OrganisationRole { OrganisationId = "15H", RoleId = "RO218", IsPrimaryRole = false, },
                },
            };

            // Organisations
            List<OdsOrganisation> organisations = new()
            {
                new OdsOrganisation() { Id = "XDH", Name = "DEPARTMENT OF HEALTH AND SOCIAL CARE", AddressLine1 = "39 VICTORIA STREET", Town = "LONDON", Postcode = "SW1H 0EU", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "X2601", Name = "NHS ENGLAND - X26 - LEEDS GOVERNMENT HUB", AddressLine1 = "7-8 WELLINGTON PLACE", Town = "LEEDS", Postcode = "LS1 4AP", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "X26011", Name = "NHS LONDON SMSP ADAPTER", AddressLine1 = "1 WHITEHALL QUAY", Town = "LEEDS", Postcode = "LS1 4HR", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "QOQ", Name = "NHS HUMBER AND NORTH YORKSHIRE INTEGRATED CARE BOARD", AddressLine1 = "2ND FLOOR", Town = "HULL", Postcode = "HU1 1UY", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "03FAA", Name = "WILBERFORCE COURT", AddressLine1 = "HIGH STREET", Town = "HULL", Postcode = "HU1 1UY", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "B81008", Name = "EAST HULL FAMILY PRACTICE", AddressLine1 = "MORRILL STREET HEALTH CENTRE", Town = "HULL", Postcode = "HU9 2LJ", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "Q86", Name = "NHS ENGLAND SOUTH WEST (SOUTH WEST NORTH)", AddressLine1 = "BEWLEY HOUSE", Town = "CHIPPENHAM", Postcode = "SN15 1JW", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "QWO", Name = "NHS WEST YORKSHIRE INTEGRATED CARE BOARD", AddressLine1 = "WHITE ROSE HOUSE", Town = "WAKEFIELD", Postcode = "WF1 1LT", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "Y04205", Name = "DR LOCAL CARE DIRECT OOH", AddressLine1 = "UNIT 2", Town = "HUDDERSFIELD", Postcode = "HD2 1GQ", Country = "ENGLAND", IsActive = true, },
                new OdsOrganisation() { Id = "VLRE8", Name = "ST PHILIPS CLOSE (CARE HOME)", AddressLine1 = "MIDDLETON", Town = "LEEDS", Postcode = "LS10 3TR", Country = "ENGLAND", IsActive = true, },
                gpPracticeOrganisation1,
                gpPracticeOrganisation2,
                bobOdsOrganisation,
                sueOdsOrganisation,
                aliceOdsOrganisation,
                daveOdsOrganisation,
            };

            context.InsertRangeWithIdentity(organisations);
        }
    }
}
