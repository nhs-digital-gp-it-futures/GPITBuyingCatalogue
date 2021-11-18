using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    [ExcludeFromCodeCoverage]
    class BaseMigrator
    {
        private readonly string ISAPIConnectionString;
        private readonly string ORDAPIConnectionString;
        protected readonly string GPITBuyingCatalogueConnectionString;

        protected List<LegacyModels.Organisation> legacyOrganisations;
        protected List<LegacyModels.RelatedOrganisation> legacyRelatedOrganisations;
        protected List<LegacyModels.AspNetUser> legacyUsers;
        protected List<LegacyModels.Supplier> legacySuppliers;
        protected List<LegacyModels.Contact> legacyContacts;
        protected List<LegacyModels.Order> validLegacyOrders;
        protected List<LegacyModels.Order> testLegacyOrders;
        protected List<LegacyModels.DefaultDeliveryDate> legacyDefaultDeliveryDates;
        protected List<LegacyModels.OrderItem> legacyOrderItems;
        protected List<LegacyModels.OrderItemRecipient> legacyOrderItemRecipients;
        protected List<LegacyModels.ServiceRecipient> legacyServiceRecipients;

        protected BaseMigrator()
        {
            ISAPIConnectionString = GetConnectionString("ISAPI");
            ORDAPIConnectionString = GetConnectionString("ORDAPI");
            GPITBuyingCatalogueConnectionString = GetConnectionString("GPITBuyingCatalogue");
        }

        protected void LoadLegacyData()
        {
            System.Diagnostics.Trace.WriteLine("Loading legacy data");
            LoadLegacyOrganisations();
            LoadLegacyRelatedOrganisations();
            LoadLegacyUsers();
            LoadLegacySuppliers();
            LoadLegacyContacts();
            LoadValidLegacyOrders();
            LoadTestLegacyOrders();
            LoadLegacyDefaultDeliveryDates();
            LoadLegacyOrderItems();
            LoadLegacyOrderItemRecipients();
            LoadValidLegacyServiceRecipients();

            // Housekeeping on the organisations we migrate accross
            var validOrganisations = new List<LegacyModels.Organisation>();
            validOrganisations.Add(legacyOrganisations.Single(x => x.Name == "NHS Digital"));
            validOrganisations.AddRange(legacyOrganisations.Where(x => legacyUsers.Any(y => y.PrimaryOrganisationId == x.OrganisationId)));
            validOrganisations.AddRange(legacyOrganisations.Where(x => validLegacyOrders.Any(y => y.OrderingPartyId == x.OrganisationId)));
            validOrganisations.AddRange(legacyOrganisations.Where(x => legacyRelatedOrganisations.Any(y => y.RelatedOrganisationId == x.OrganisationId)));
            validOrganisations.AddRange(legacyOrganisations.Where(x => legacyRelatedOrganisations.Any(y => y.OrganisationId == x.OrganisationId)));
            legacyOrganisations = validOrganisations.Distinct().ToList();
            System.Diagnostics.Trace.WriteLine($"Now {legacyOrganisations.Count} valid organisations from legacy database");
        }

        private static string GetConnectionString(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"Failed to get connection string config for {connectionStringName}");

            return connectionString;
        }

        protected EntityFramework.BuyingCatalogueDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<EntityFramework.BuyingCatalogueDbContext>()
                .UseSqlServer(GPITBuyingCatalogueConnectionString)
                .EnableSensitiveDataLogging()
                .Options;

            return new EntityFramework.BuyingCatalogueDbContext(options, new IdentityServiceStub());
        }

        protected static List<EntityFramework.Organisations.Models.Organisation> GetCurrentOrganisations(EntityFramework.BuyingCatalogueDbContext context)
        {
            var entities = context.Organisations.ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} organisations from current database");
            return entities;
        }

        protected static List<EntityFramework.Users.Models.AspNetUser> GetCurrentUsers(EntityFramework.BuyingCatalogueDbContext context)
        {
            var entities = context.AspNetUsers.ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} users from current database");
            return entities;
        }

        protected static List<EntityFramework.Ordering.Models.Order> GetCurrentOrders(EntityFramework.BuyingCatalogueDbContext context)
        {
            var entities = context.Orders
                .Include(x => x.OrderingPartyContact)
                .Include(x => x.SupplierContact)
                .Include(x => x.DefaultDeliveryDates)
                .Include(x => x.OrderItems)
                .IgnoreQueryFilters().ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} orders from current database");
            return entities;
        }

        protected static List<EntityFramework.Ordering.Models.ServiceRecipient> GetCurrentServiceRecipients(EntityFramework.BuyingCatalogueDbContext context)
        {
            var entities = context.ServiceRecipients.ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} service recipients from current database");
            return entities;
        }

        protected static bool AreAddressesEqual(EntityFramework.Addresses.Models.Address current, EntityFramework.Addresses.Models.Address legacy)
        {
            if (!(current.Line1 == legacy.Line1))
                return false;

            if (!(current.Line2 == legacy.Line2))
                return false;

            if (!(current.Line3 == legacy.Line3))
                return false;

            if (!(current.Line4 == legacy.Line4))
                return false;

            if (!(current.Line5 == legacy.Line5))
                return false;

            if (!(current.Postcode == legacy.Postcode))
                return false;

            if (!(current.Town == legacy.Town))
                return false;

            if (!(current.Country == legacy.Country))
                return false;

            if (!(current.County == legacy.County))
                return false;

            return true;
        }

        private void LoadLegacyOrganisations()
        {
            using var sqlConnection = new SqlConnection(ISAPIConnectionString);
            sqlConnection.Open();
            legacyOrganisations = sqlConnection.Query<LegacyModels.Organisation>("select * from Organisations").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyOrganisations.Count} organisations from legacy database");
        }

        private void LoadLegacyRelatedOrganisations()
        {
            using var sqlConnection = new SqlConnection(ISAPIConnectionString);
            sqlConnection.Open();
            legacyRelatedOrganisations = sqlConnection.Query<LegacyModels.RelatedOrganisation>("select * from RelatedOrganisations").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyRelatedOrganisations.Count} related organisations from legacy database");
        }


        private void LoadLegacyUsers()
        {
            using var sqlConnection = new SqlConnection(ISAPIConnectionString);
            sqlConnection.Open();
            legacyUsers = sqlConnection.Query<LegacyModels.AspNetUser>("select * from AspNetUsers").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyUsers.Count} users from legacy database");
        }

        private void LoadLegacySuppliers()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacySuppliers = sqlConnection.Query<LegacyModels.Supplier>("select * from Supplier").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacySuppliers.Count} suppliers from legacy database");
        }

        private void LoadLegacyContacts()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacyContacts = sqlConnection.Query<LegacyModels.Contact>("select * from Contact").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyContacts.Count} contacts from legacy database");
        }

        private void LoadValidLegacyOrders()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            validLegacyOrders = sqlConnection.Query<LegacyModels.Order>("select * from [Order] WHERE LastUpdatedByName NOT IN ('Jon Alsop','Roberts Bot','Ljiljana Evans','Lorraine Olowosuko','Enitan Onabamiro')").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {validLegacyOrders.Count} valid orders from legacy database");
        }

        private void LoadTestLegacyOrders()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            testLegacyOrders = sqlConnection.Query<LegacyModels.Order>("select * from [Order] WHERE LastUpdatedByName IN ('Jon Alsop','Roberts Bot','Ljiljana Evans','Lorraine Olowosuko','Enitan Onabamiro')").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {testLegacyOrders.Count} test orders from legacy database");
        }

        private void LoadLegacyDefaultDeliveryDates()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacyDefaultDeliveryDates = sqlConnection.Query<LegacyModels.DefaultDeliveryDate>("select * from DefaultDeliveryDate").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyDefaultDeliveryDates.Count} default delivery dates from legacy database");
        }

        private void LoadLegacyOrderItems()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacyOrderItems = sqlConnection.Query<LegacyModels.OrderItem>("select * from OrderItem").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyOrderItems.Count} order items from legacy database");
        }

        private void LoadLegacyOrderItemRecipients()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacyOrderItemRecipients = sqlConnection.Query<LegacyModels.OrderItemRecipient>("select * from OrderItemRecipients").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyOrderItemRecipients.Count} order item recipients from legacy database");
        }

        private void LoadValidLegacyServiceRecipients()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacyServiceRecipients = sqlConnection.Query<LegacyModels.ServiceRecipient>(
                @"select * from ServiceRecipient where OdsCode in (select distinct oir.OdsCode from OrderItemRecipients oir
                join[Order] o on o.Id = oir.OrderId
                WHERE o.LastUpdatedByName NOT IN('Jon Alsop', 'Roberts Bot', 'Ljiljana Evans', 'Lorraine Olowosuko', 'Enitan Onabamiro'))").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyServiceRecipients.Count} service recipients from legacy database");
        }
    }    
}
