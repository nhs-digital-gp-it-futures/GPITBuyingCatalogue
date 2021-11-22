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
    internal class BaseMigrator
    {
        private readonly string ISAPIConnectionString;
        private readonly string ORDAPIConnectionString;
        protected readonly string GPITBuyingCatalogueConnectionString;
        private readonly string TestUsers;

        protected List<LegacyModels.Organisation> legacyOrganisations;
        protected List<LegacyModels.RelatedOrganisation> legacyRelatedOrganisations;
        protected List<LegacyModels.AspNetUser> legacyUsers;
        protected List<LegacyModels.Supplier> legacySuppliers;
        protected List<LegacyModels.Contact> legacyContacts;
        protected List<LegacyModels.Order> validLegacyOrders;
        protected List<LegacyModels.Order> testLegacyOrders;
        protected List<LegacyModels.DefaultDeliveryDate> legacyDefaultDeliveryDates;
        protected List<LegacyModels.OrderItem> validLegacyOrderItems;
        protected List<LegacyModels.OrderItemRecipient> validLegacyOrderItemRecipients;
        protected List<LegacyModels.ServiceRecipient> legacyServiceRecipients;

        protected BaseMigrator()
        {
            ISAPIConnectionString = GetConnectionString("ISAPI");
            ORDAPIConnectionString = GetConnectionString("ORDAPI");
            GPITBuyingCatalogueConnectionString = GetConnectionString("GPITBuyingCatalogue");
            TestUsers = ConfigurationManager.AppSettings.Get("TestUsers");

            if (string.IsNullOrEmpty(TestUsers))
                throw new ConfigurationErrorsException("TestUsers are not specified in config");
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
            LoadValidLegacyOrderItems();
            LoadValidLegacyOrderItemRecipients();
            LoadValidLegacyServiceRecipients();

            // Housekeeping on the organisations we migrate accross
            var validOrganisations = new List<LegacyModels.Organisation>
            {
                legacyOrganisations.Single(x => x.Name == "NHS Digital")
            };
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
                throw new ConfigurationErrorsException($"Failed to get connection string config for {connectionStringName}");

            return connectionString;
        }

        protected EntityFramework.BuyingCatalogueDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<EntityFramework.BuyingCatalogueDbContext>()
                .UseSqlServer(GPITBuyingCatalogueConnectionString)
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
                .Include(x => x.OrderItems).ThenInclude(x => x.OrderItemRecipients)
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

        protected static void CompareCount(string table, int currentCount, int legacyCount)
        {
            if (currentCount != legacyCount)
                System.Diagnostics.Trace.WriteLine($"Warning!!!. {table} record count mismatch. Current {currentCount}, Legacy {legacyCount}");
            else
                System.Diagnostics.Trace.WriteLine($"{table} table record count matches: {currentCount}");
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
            validLegacyOrders = sqlConnection.Query<LegacyModels.Order>($"select * from [Order] WHERE LastUpdatedByName NOT IN ({TestUsers})").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {validLegacyOrders.Count} valid orders from legacy database");
        }

        private void LoadTestLegacyOrders()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            testLegacyOrders = sqlConnection.Query<LegacyModels.Order>($"select * from [Order] WHERE LastUpdatedByName IN ({TestUsers})").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {testLegacyOrders.Count} test orders from legacy database");
        }

        private void LoadLegacyDefaultDeliveryDates()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacyDefaultDeliveryDates = sqlConnection.Query<LegacyModels.DefaultDeliveryDate>("select * from DefaultDeliveryDate").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyDefaultDeliveryDates.Count} default delivery dates from legacy database");
        }

        private void LoadValidLegacyOrderItems()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();

            // Comment from SSIS package...
            // There is a price with ID 1012 that has been deleted so the query in Get Order Items replaces that ID with the ID of the replacement price (1766).
            validLegacyOrderItems = sqlConnection.Query<LegacyModels.OrderItem>($@"SELECT OrderId,CatalogueItemId,ProvisioningTypeId,CataloguePriceTypeId,PricingUnitName,TimeUnitId,EstimationPeriodId,
                CASE WHEN PriceId = 1012 THEN 1766 ELSE PriceId END AS PriceId,CurrencyCode,Price,DefaultDeliveryDate,Created,LastUpdated
                FROM OrderItem
                where OrderId in (select Id from [Order] WHERE LastUpdatedByName NOT IN ({TestUsers}))").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {validLegacyOrderItems.Count} order items from legacy database");
        }

        private void LoadValidLegacyOrderItemRecipients()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            validLegacyOrderItemRecipients = sqlConnection.Query<LegacyModels.OrderItemRecipient>($@"select * from OrderItemRecipients
                where OrderId in (select Id from [Order] WHERE LastUpdatedByName NOT IN ({TestUsers}))").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {validLegacyOrderItemRecipients.Count} order item recipients from legacy database");
        }

        private void LoadValidLegacyServiceRecipients()
        {
            using var sqlConnection = new SqlConnection(ORDAPIConnectionString);
            sqlConnection.Open();
            legacyServiceRecipients = sqlConnection.Query<LegacyModels.ServiceRecipient>(
                $@"select * from ServiceRecipient where OdsCode in (select distinct oir.OdsCode from OrderItemRecipients oir
                join[Order] o on o.Id = oir.OrderId
                WHERE o.LastUpdatedByName NOT IN({TestUsers}))").ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {legacyServiceRecipients.Count} service recipients from legacy database");
        }
    }
}
