using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;

// TODO - Following tables
//--DefaultDeliveryDates
//-- OrderItemRecipients
//-- OrderItems
//-- ServiceRecipients
// Ideally only update new items if the copied accross values have actually changed

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    [ExcludeFromCodeCoverage]
    sealed class Migrator
    {        
        private readonly string ISAPIConnectionString;
        private readonly string ORDAPIConnectionString;
        private readonly string GPITBuyingCatalogueConnectionString;
        
        private List<LegacyModels.Organisation> legacyOrganisations;
        private List<LegacyModels.AspNetUser> legacyUsers;
        private List<LegacyModels.Supplier> legacySuppliers;
        private List<LegacyModels.Contact> legacyContacts;
        private List<LegacyModels.Order> legacyOrders;

        public Migrator()
        {     
            ISAPIConnectionString = GetConnectionString("ISAPI");
            ORDAPIConnectionString = GetConnectionString("ORDAPI");
            GPITBuyingCatalogueConnectionString = GetConnectionString("GPITBuyingCatalogue");
        }

        public void RunMigration()
        {
            System.Diagnostics.Trace.WriteLine("Loading legacy data");
            LoadLegacyOrganisations();
            LoadLegacyUsers();
            LoadLegacySuppliers();
            LoadLegacyContacts();
            LoadLegacyOrders();

            System.Diagnostics.Trace.WriteLine("Migrating...");
            MigrateOrganisations();
            MigrateUsers();
            MigrateOrders();
        }

        private void MigrateOrganisations()
        {
            System.Diagnostics.Trace.WriteLine("Migrating organisations start");

            using (var context = GetContext())
            {
                var currentOrganisations = GetCurrentOrganisations(context);

                foreach (var legacyOrganisation in legacyOrganisations)
                {
                    if (!currentOrganisations.Any(x => x.OdsCode.Equals(legacyOrganisation.OdsCode, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        System.Diagnostics.Trace.WriteLine($"Adding missing organisation with ODS code {legacyOrganisation.OdsCode}");

                        var newOrganisation = new EntityFramework.Organisations.Models.Organisation
                        {
                            Name = legacyOrganisation.Name,
                            Address = JsonDeserializer.Deserialize<EntityFramework.Addresses.Models.Address>(legacyOrganisation.Address),
                            OdsCode = legacyOrganisation.OdsCode,
                            PrimaryRoleId = legacyOrganisation.PrimaryRoleId,
                            CatalogueAgreementSigned = legacyOrganisation.CatalogueAgreementSigned,
                            LastUpdated = legacyOrganisation.LastUpdated,
                        };

                        context.Organisations.Add(newOrganisation);
                        context.SaveChanges();
                        legacyOrganisation.NewId = newOrganisation.Id;
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"Updating organisation with ODS code {legacyOrganisation.OdsCode}");

                        var currentOrganisation = currentOrganisations.Single(x => x.OdsCode.Equals(legacyOrganisation.OdsCode, StringComparison.CurrentCultureIgnoreCase));

                        currentOrganisation.Name = legacyOrganisation.Name;
                        currentOrganisation.Address = JsonDeserializer.Deserialize<EntityFramework.Addresses.Models.Address>(legacyOrganisation.Address);
                        currentOrganisation.OdsCode = legacyOrganisation.OdsCode;
                        currentOrganisation.PrimaryRoleId = legacyOrganisation.PrimaryRoleId;
                        currentOrganisation.CatalogueAgreementSigned = legacyOrganisation.CatalogueAgreementSigned;
                        currentOrganisation.LastUpdated = legacyOrganisation.LastUpdated;

                        context.SaveChanges();
                        legacyOrganisation.NewId = currentOrganisation.Id;
                    }
                }

                if (legacyOrganisations.Any(x => x.NewId == 0))
                    throw new InvalidOperationException("LegacyOrganisations contains at least one unmatched organisation");
            }

            System.Diagnostics.Trace.WriteLine("Migrating organisations end");
        }

        private void MigrateUsers()
        {
            System.Diagnostics.Trace.WriteLine("Migrating users start");

            using (var context = GetContext())
            {
                var currentUsers = GetCurrentUsers(context);

                foreach (var legacyUser in legacyUsers)
                {
                    if (!currentUsers.Any(x => x.UserName.Equals(legacyUser.Email, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        System.Diagnostics.Trace.WriteLine($"Adding missing user with UserName {legacyUser.UserName}");

                        var newUser = new EntityFramework.Users.Models.AspNetUser
                        {
                            UserName = legacyUser.UserName,
                            NormalizedUserName = legacyUser.NormalizedUserName,
                            Email = legacyUser.Email,
                            NormalizedEmail = legacyUser.NormalizedEmail,
                            EmailConfirmed = legacyUser.EmailConfirmed,
                            PasswordHash = legacyUser.PasswordHash,
                            SecurityStamp = legacyUser.PasswordHash,
                            ConcurrencyStamp = legacyUser.ConcurrencyStamp,
                            PhoneNumber = legacyUser.PhoneNumber,
                            PhoneNumberConfirmed = legacyUser.PhoneNumberConfirmed,
                            TwoFactorEnabled = legacyUser.TwoFactorEnabled,
                            LockoutEnd = legacyUser.LockoutEnd,
                            LockoutEnabled = legacyUser.LockoutEnabled,
                            AccessFailedCount = legacyUser.AccessFailedCount,
                            PrimaryOrganisationId = legacyOrganisations.Single(x => x.OrganisationId == legacyUser.PrimaryOrganisationId).NewId,
                            OrganisationFunction = legacyUser.OrganisationFunction,
                            Disabled = legacyUser.Disabled,
                            CatalogueAgreementSigned = legacyUser.CatalogueAgreementSigned,
                            FirstName = legacyUser.FirstName,
                            LastName = legacyUser.LastName,
                        };

                        context.AspNetUsers.Add(newUser);
                        context.SaveChanges();

                        legacyUser.NewId = newUser.Id;
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"Updating user with UserName {legacyUser.UserName}");

                        var currentUser = currentUsers.Single(x => x.UserName.Equals(legacyUser.UserName, StringComparison.CurrentCultureIgnoreCase));

                        currentUser.UserName = legacyUser.UserName;
                        currentUser.NormalizedUserName = legacyUser.NormalizedUserName;
                        currentUser.Email = legacyUser.Email;
                        currentUser.NormalizedEmail = legacyUser.NormalizedEmail;
                        currentUser.EmailConfirmed = legacyUser.EmailConfirmed;
                        currentUser.PasswordHash = legacyUser.PasswordHash;
                        currentUser.SecurityStamp = legacyUser.PasswordHash;
                        currentUser.ConcurrencyStamp = legacyUser.ConcurrencyStamp;
                        currentUser.PhoneNumber = legacyUser.PhoneNumber;
                        currentUser.PhoneNumberConfirmed = legacyUser.PhoneNumberConfirmed;
                        currentUser.TwoFactorEnabled = legacyUser.TwoFactorEnabled;
                        currentUser.LockoutEnd = legacyUser.LockoutEnd;
                        currentUser.LockoutEnabled = legacyUser.LockoutEnabled;
                        currentUser.AccessFailedCount = legacyUser.AccessFailedCount;
                        currentUser.PrimaryOrganisationId = legacyOrganisations.Single(x => x.OrganisationId == legacyUser.PrimaryOrganisationId).NewId;
                        currentUser.OrganisationFunction = legacyUser.OrganisationFunction;
                        currentUser.Disabled = legacyUser.Disabled;
                        currentUser.CatalogueAgreementSigned = legacyUser.CatalogueAgreementSigned;
                        currentUser.FirstName = legacyUser.FirstName;
                        currentUser.LastName = legacyUser.LastName;
                        
                        context.SaveChanges();                        

                        legacyUser.NewId = currentUser.Id;
                    }
                }
            }

            if (legacyUsers.Any(x => x.NewId == 0))
                throw new InvalidOperationException("LegacyUsers contains at least one unmatched user");

            System.Diagnostics.Trace.WriteLine("Migrating users end");
        }

        private void MigrateOrders()
        {
            System.Diagnostics.Trace.WriteLine("Migrating orders start");

            using (var context = GetContext())
            {
                var currentOrders = GetCurrentOrders(context);

                foreach (var legacyOrder in legacyOrders)
                {
                    if (!currentOrders.Any(x => x.Id == legacyOrder.Id))
                    {
                        System.Diagnostics.Trace.WriteLine($"Adding missing order with Description {legacyOrder.Description}");

                        var newOrder = new EntityFramework.Ordering.Models.Order
                        {
                            Id = legacyOrder.Id,
                            Description = legacyOrder.Description,
                            OrderingPartyId = legacyOrganisations.Single(x => x.OrganisationId == legacyOrder.OrderingPartyId).NewId,                            
                            OrderingPartyContact = legacyOrder.OrderingPartyContactId == null ? null : new EntityFramework.Ordering.Models.Contact
                            {
                                FirstName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).FirstName,
                                LastName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).LastName,
                                Email = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Email,
                                Phone = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Phone
                            },
                            SupplierId = legacyOrder.SupplierId == null ? null : Convert.ToInt32(legacySuppliers.Single(x => x.Id.Equals(legacyOrder.SupplierId, StringComparison.CurrentCultureIgnoreCase)).Id),                            
                            SupplierContact = legacyOrder.SupplierContactId == null ? null : new EntityFramework.Ordering.Models.Contact
                            {
                                FirstName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).FirstName,
                                LastName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).LastName,
                                Email = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Email,
                                Phone = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Phone
                            },
                            CommencementDate = legacyOrder.CommencementDate,
                            FundingSourceOnlyGms = legacyOrder.FundingSourceOnlyGms,
                            Created = legacyOrder.Created,
                            LastUpdated = legacyOrder.LastUpdated,
                            OrderStatus = legacyOrder.OrderStatusId == 1 ? EntityFramework.Ordering.Models.OrderStatus.Complete : EntityFramework.Ordering.Models.OrderStatus.Incomplete,
                            IsDeleted = legacyOrder.IsDeleted
                        };

                        context.Orders.Add(newOrder);
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ordering.Orders ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ordering.Orders OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }

                        legacyOrder.NewId = newOrder.Id;

                        if (legacyOrder.Completed.HasValue)
                            UpdateOrderCompletedDate(newOrder.Id, legacyOrder.Completed.Value);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"Updating order with Description {legacyOrder.Description}");

                        var currentOrder = currentOrders.Single(x => x.Id == legacyOrder.Id);

                        currentOrder.Description = legacyOrder.Description;
                        currentOrder.OrderingPartyId = legacyOrganisations.Single(x => x.OrganisationId == legacyOrder.OrderingPartyId).NewId;                        
                        currentOrder.OrderingPartyContact = legacyOrder.OrderingPartyContactId == null ? null : new EntityFramework.Ordering.Models.Contact
                        {
                            FirstName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).FirstName,
                            LastName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).LastName,
                            Email = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Email,
                            Phone = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Phone
                        };
                        currentOrder.SupplierId = legacyOrder.SupplierId == null ? null : Convert.ToInt32(legacySuppliers.Single(x => x.Id.Equals(legacyOrder.SupplierId, StringComparison.CurrentCultureIgnoreCase)).Id);                        
                        currentOrder.SupplierContact = legacyOrder.SupplierContactId == null ? null : new EntityFramework.Ordering.Models.Contact
                        {
                            FirstName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).FirstName,
                            LastName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).LastName,
                            Email = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Email,
                            Phone = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Phone
                        };
                        currentOrder.CommencementDate = legacyOrder.CommencementDate;
                        currentOrder.FundingSourceOnlyGms = legacyOrder.FundingSourceOnlyGms;
                        currentOrder.Created = legacyOrder.Created;
                        currentOrder.LastUpdated = legacyOrder.LastUpdated;
                        currentOrder.OrderStatus = legacyOrder.OrderStatusId == 1 ? EntityFramework.Ordering.Models.OrderStatus.Complete : EntityFramework.Ordering.Models.OrderStatus.Incomplete;
                        currentOrder.IsDeleted = legacyOrder.IsDeleted;

                        context.SaveChanges();

                        legacyOrder.NewId = currentOrder.Id;
                    }
                }
            }

            if (legacyOrders.Any(x => x.NewId == 0))
                throw new InvalidOperationException("LegacyOrders contains at least one unmatched order");

            System.Diagnostics.Trace.WriteLine("Migrating orders end");
        }              

        private void LoadLegacyOrganisations()
        {
            using (var sqlConnection = new SqlConnection(ISAPIConnectionString))
            {
                sqlConnection.Open();
                legacyOrganisations = sqlConnection.Query<LegacyModels.Organisation>("select * from Organisations").ToList();
                System.Diagnostics.Trace.WriteLine($"Loaded {legacyOrganisations.Count} organisations from legacy database");
            }
        }

        private void LoadLegacyUsers()
        {
            using (var sqlConnection = new SqlConnection(ISAPIConnectionString))
            {
                sqlConnection.Open();                                
                legacyUsers = sqlConnection.Query<LegacyModels.AspNetUser>("select * from AspNetUsers").ToList();              
                System.Diagnostics.Trace.WriteLine($"Loaded {legacyUsers.Count} users from legacy database");
            }
        }

        private void LoadLegacySuppliers()
        {
            using (var sqlConnection = new SqlConnection(ORDAPIConnectionString))
            {
                sqlConnection.Open();
                legacySuppliers = sqlConnection.Query<LegacyModels.Supplier>("select * from Supplier").ToList();
                System.Diagnostics.Trace.WriteLine($"Loaded {legacySuppliers.Count} suppliers from legacy database");
            }
        }

        private void LoadLegacyContacts()
        {
            using (var sqlConnection = new SqlConnection(ORDAPIConnectionString))
            {
                sqlConnection.Open();
                legacyContacts = sqlConnection.Query<LegacyModels.Contact>("select * from Contact").ToList();
                System.Diagnostics.Trace.WriteLine($"Loaded {legacyContacts.Count} contacts from legacy database");
            }
        }

        private void LoadLegacyOrders()
        {
            // MJRTODO - Filter out any junk/test orders. See SSIS for details.
            using (var sqlConnection = new SqlConnection(ORDAPIConnectionString))
            {
                sqlConnection.Open();
                legacyOrders = sqlConnection.Query<LegacyModels.Order>("select * from [Order]").ToList();
                System.Diagnostics.Trace.WriteLine($"Loaded {legacyOrders.Count} orders from legacy database");
            }
        }

        private List<EntityFramework.Organisations.Models.Organisation> GetCurrentOrganisations(EntityFramework.BuyingCatalogueDbContext context)
        {
            var entities = context.Organisations.ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} organisations from current database");
            return entities;
        }

        private List<EntityFramework.Users.Models.AspNetUser> GetCurrentUsers(EntityFramework.BuyingCatalogueDbContext context)
        {
            var entities = context.AspNetUsers.ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} users from current database");
            return entities;
        }

        private List<EntityFramework.Ordering.Models.Order> GetCurrentOrders(EntityFramework.BuyingCatalogueDbContext context)
        {
            var entities = context.Orders.IgnoreQueryFilters().ToList();
            System.Diagnostics.Trace.WriteLine($"Loaded {entities.Count} orders from current database");
            return entities;
        }

        private EntityFramework.BuyingCatalogueDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<EntityFramework.BuyingCatalogueDbContext>()
                .UseSqlServer(GPITBuyingCatalogueConnectionString)
                .EnableSensitiveDataLogging()
                .Options;

            return new EntityFramework.BuyingCatalogueDbContext(options, new IdentityServiceStub());
        }

        private string GetConnectionString(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"Failed to get connection string config for {connectionStringName}");

            return connectionString;
        }

        private void UpdateOrderCompletedDate(int orderId, DateTime completedDate)
        {
            using (var sqlConnection = new SqlConnection(GPITBuyingCatalogueConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute("update ordering.Orders set Completed = @val where Id = @id", new { val = completedDate, id = orderId });
            }
        }
    }
}
