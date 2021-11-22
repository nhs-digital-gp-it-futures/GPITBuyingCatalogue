using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    [ExcludeFromCodeCoverage]
    internal sealed class Migrator : BaseMigrator
    {
        public Migrator() : base()
        {
        }

        public void RunMigration()
        {
            System.Diagnostics.Trace.WriteLine("####### STARTING MIGRATION #######");

            LoadLegacyData();

            System.Diagnostics.Trace.WriteLine("Migrating...");
            MigrateOrganisations();
            MigrateRelatedOrganisations();
            MigrateUsers();
            MigrateOrders();
            MigrateDefaultDeliveryDates();
            MigrateServiceRecipients();
            MigrateOrderItemsAndRecipients();
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
                            LastUpdated = legacyOrganisation.LastUpdated
                        };

                        context.Organisations.Add(newOrganisation);
                        context.SaveChangesWithoutAudit();
                        legacyOrganisation.NewId = newOrganisation.Id;
                    }
                    else
                    {
                        var currentOrganisation = currentOrganisations.Single(x => x.OdsCode.Equals(legacyOrganisation.OdsCode, StringComparison.CurrentCultureIgnoreCase));

                        currentOrganisation.Name = legacyOrganisation.Name;
                        currentOrganisation.OdsCode = legacyOrganisation.OdsCode;
                        currentOrganisation.PrimaryRoleId = legacyOrganisation.PrimaryRoleId;
                        currentOrganisation.CatalogueAgreementSigned = legacyOrganisation.CatalogueAgreementSigned;

                        var legacyAddress = JsonDeserializer.Deserialize<EntityFramework.Addresses.Models.Address>(legacyOrganisation.Address);

                        if (!AreAddressesEqual(currentOrganisation.Address, legacyAddress))
                            currentOrganisation.Address = legacyAddress;

                        if (context.SaveChangesWithoutAudit() > 0)
                            System.Diagnostics.Trace.WriteLine($"Updated organisation with ODS code {legacyOrganisation.OdsCode}");

                        legacyOrganisation.NewId = currentOrganisation.Id;
                    }
                }

                if (legacyOrganisations.Any(x => x.NewId == 0))
                    throw new InvalidOperationException("LegacyOrganisations contains at least one unmatched organisation");
            }

            System.Diagnostics.Trace.WriteLine("Migrating organisations end");
        }

        private void MigrateRelatedOrganisations()
        {
            System.Diagnostics.Trace.WriteLine("Migrating related organisations start");

            using (var context = GetContext())
            {
                var relatedOrganisations = context.RelatedOrganisations.ToList();

                foreach (var relatedOrganisation in relatedOrganisations)
                {
                    context.RelatedOrganisations.Remove(relatedOrganisation);
                }

                context.SaveChangesWithoutAudit();

                foreach (var legacyRelatedOrganisation in legacyRelatedOrganisations)
                {
                    var relatedOrganisation = new EntityFramework.Organisations.Models.RelatedOrganisation
                    {
                        OrganisationId = legacyOrganisations.Single(x => x.OrganisationId == legacyRelatedOrganisation.OrganisationId).NewId,
                        RelatedOrganisationId = legacyOrganisations.Single(x => x.OrganisationId == legacyRelatedOrganisation.RelatedOrganisationId).NewId,
                    };

                    System.Diagnostics.Trace.WriteLine($"Adding related organisation {relatedOrganisation.Organisation} to {relatedOrganisation.RelatedOrganisationId}");

                    context.RelatedOrganisations.Add(relatedOrganisation);
                }

                context.SaveChangesWithoutAudit();
            }

            System.Diagnostics.Trace.WriteLine("Migrating related organisations end");
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
                        context.SaveChangesWithoutAudit();

                        legacyUser.NewId = newUser.Id;
                    }
                    else
                    {
                        var currentUser = currentUsers.Single(x => x.UserName.Equals(legacyUser.UserName, StringComparison.CurrentCultureIgnoreCase));

                        currentUser.UserName = legacyUser.UserName;
                        currentUser.NormalizedUserName = legacyUser.NormalizedUserName;
                        currentUser.Email = legacyUser.Email;
                        currentUser.NormalizedEmail = legacyUser.NormalizedEmail;
                        currentUser.EmailConfirmed = legacyUser.EmailConfirmed;
                        currentUser.PasswordHash = legacyUser.PasswordHash;
                        currentUser.SecurityStamp = legacyUser.SecurityStamp;
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

                        if (context.SaveChangesWithoutAudit() > 0)
                            System.Diagnostics.Trace.WriteLine($"Updated user with UserName {legacyUser.UserName}");

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

                foreach (var legacyOrder in validLegacyOrders)
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
                            OrderStatus = legacyOrder.OrderStatusId == 1 ? EntityFramework.Ordering.Models.OrderStatus.Complete : EntityFramework.Ordering.Models.OrderStatus.Incomplete,
                            IsDeleted = legacyOrder.IsDeleted,
                            LastUpdated = legacyOrder.LastUpdated,
                            LastUpdatedBy = legacyUsers.FirstOrDefault(x => x.Id.Equals(legacyOrder.LastUpdatedBy.ToString(), StringComparison.CurrentCultureIgnoreCase))?.NewId
                        };

                        context.Orders.Add(newOrder);
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ordering.Orders ON");
                            context.SaveChangesWithoutAudit();
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
                        var currentOrder = currentOrders.Single(x => x.Id == legacyOrder.Id);

                        currentOrder.Description = legacyOrder.Description;
                        currentOrder.OrderingPartyId = legacyOrganisations.Single(x => x.OrganisationId == legacyOrder.OrderingPartyId).NewId;
                        currentOrder.SupplierId = legacyOrder.SupplierId == null ? null : Convert.ToInt32(legacySuppliers.Single(x => x.Id.Equals(legacyOrder.SupplierId, StringComparison.CurrentCultureIgnoreCase)).Id);
                        currentOrder.CommencementDate = legacyOrder.CommencementDate;
                        currentOrder.FundingSourceOnlyGms = legacyOrder.FundingSourceOnlyGms;
                        currentOrder.Created = legacyOrder.Created;
                        currentOrder.OrderStatus = legacyOrder.OrderStatusId == 1 ? EntityFramework.Ordering.Models.OrderStatus.Complete : EntityFramework.Ordering.Models.OrderStatus.Incomplete;
                        currentOrder.IsDeleted = legacyOrder.IsDeleted;

                        if (currentOrder.OrderingPartyContact == null && legacyOrder.OrderingPartyContactId != null)
                        {
                            currentOrder.OrderingPartyContact = new EntityFramework.Ordering.Models.Contact
                            {
                                FirstName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).FirstName,
                                LastName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).LastName,
                                Email = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Email,
                                Phone = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Phone
                            };
                        }
                        else if (currentOrder.OrderingPartyContact != null && legacyOrder.OrderingPartyContactId == null)
                        {
                            currentOrder.OrderingPartyContact = null;
                        }
                        else if (currentOrder.OrderingPartyContact != null && legacyOrder.OrderingPartyContactId != null)
                        {
                            currentOrder.OrderingPartyContact.FirstName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).FirstName;
                            currentOrder.OrderingPartyContact.LastName = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).LastName;
                            currentOrder.OrderingPartyContact.Email = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Email;
                            currentOrder.OrderingPartyContact.Phone = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId).Phone;
                        }

                        if (currentOrder.SupplierContact == null && legacyOrder.SupplierContactId != null)
                        {
                            currentOrder.SupplierContact = new EntityFramework.Ordering.Models.Contact
                            {
                                FirstName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).FirstName,
                                LastName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).LastName,
                                Email = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Email,
                                Phone = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Phone
                            };
                        }
                        else if (currentOrder.SupplierContact != null && legacyOrder.SupplierContactId == null)
                        {
                            currentOrder.SupplierContact = null;
                        }
                        else if (currentOrder.SupplierContact != null && legacyOrder.SupplierContactId != null)
                        {
                            currentOrder.SupplierContact.FirstName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).FirstName;
                            currentOrder.SupplierContact.LastName = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).LastName;
                            currentOrder.SupplierContact.Email = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Email;
                            currentOrder.SupplierContact.Phone = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId).Phone;
                        }

                        currentOrder.LastUpdated = legacyOrder.LastUpdated;
                        currentOrder.LastUpdatedBy = legacyUsers.FirstOrDefault(x => x.Id.Equals(legacyOrder.LastUpdatedBy.ToString(), StringComparison.CurrentCultureIgnoreCase))?.NewId;

                        if (context.SaveChangesWithoutAudit() > 0)
                            System.Diagnostics.Trace.WriteLine($"Updated order with Description {legacyOrder.Description}");

                        legacyOrder.NewId = currentOrder.Id;

                        if (legacyOrder.Completed.HasValue && currentOrder.Completed != legacyOrder.Completed)
                            UpdateOrderCompletedDate(currentOrder.Id, legacyOrder.Completed.Value);
                    }
                }
            }

            if (validLegacyOrders.Any(x => x.NewId == 0))
                throw new InvalidOperationException("LegacyOrders contains at least one unmatched order");

            System.Diagnostics.Trace.WriteLine("Migrating orders end");
        }

        private void MigrateDefaultDeliveryDates()
        {
            System.Diagnostics.Trace.WriteLine("Migrating default delivery dates start");

            int matchedCount = 0;

            using (var context = GetContext())
            {
                var currentOrders = GetCurrentOrders(context);

                foreach (var currentOrder in currentOrders)
                {
                    var legacyDates = legacyDefaultDeliveryDates.Where(x => x.OrderId == currentOrder.Id).ToList();

                    if (currentOrder.DefaultDeliveryDates.Any())
                        currentOrder.DefaultDeliveryDates.Clear();

                    foreach (var legacyDate in legacyDates)
                    {
                        currentOrder.DefaultDeliveryDates.Add(new EntityFramework.Ordering.Models.DefaultDeliveryDate
                        {
                            CatalogueItemId = EntityFramework.Ordering.Models.CatalogueItemId.ParseExact(legacyDate.CatalogueItemId),
                            DeliveryDate = legacyDate.DeliveryDate
                        });
                        matchedCount++;
                    }

                    if (context.SaveChangesWithoutAudit() > 0)
                        System.Diagnostics.Trace.WriteLine($"Updated order with Description {currentOrder.Description} default delivery dates");
                }
            }

            if (matchedCount != legacyDefaultDeliveryDates.Count)
                throw new InvalidOperationException("Not all legacy default delivery dates have been matched");

            System.Diagnostics.Trace.WriteLine("Migrating default delivery dates end");
        }

        private void MigrateServiceRecipients()
        {
            System.Diagnostics.Trace.WriteLine("Migrating service recipients start");

            using (var context = GetContext())
            {
                var currentServiceRecipients = GetCurrentServiceRecipients(context);

                foreach (var legacyServiceRecipient in legacyServiceRecipients)
                {
                    if (!currentServiceRecipients.Any(x => x.OdsCode.Equals(legacyServiceRecipient.OdsCode, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        System.Diagnostics.Trace.WriteLine($"Adding missing service recipient with ODS code {legacyServiceRecipient.OdsCode}");

                        var newServiceRecipient = new EntityFramework.Ordering.Models.ServiceRecipient
                        {
                            OdsCode = legacyServiceRecipient.OdsCode,
                            Name = legacyServiceRecipient.Name
                        };

                        context.ServiceRecipients.Add(newServiceRecipient);
                        context.SaveChangesWithoutAudit();
                        legacyServiceRecipient.Processed = true;
                    }
                    else
                    {
                        var currentServiceRecipient = currentServiceRecipients.Single(x => x.OdsCode.Equals(legacyServiceRecipient.OdsCode, StringComparison.CurrentCultureIgnoreCase));

                        currentServiceRecipient.Name = legacyServiceRecipient.Name;

                        if (context.SaveChangesWithoutAudit() > 0)
                            System.Diagnostics.Trace.WriteLine($"Updated service recipient with ODS code {legacyServiceRecipient.OdsCode}");

                        legacyServiceRecipient.Processed = true;
                    }
                }

                if (legacyServiceRecipients.Any(x => !x.Processed))
                    throw new InvalidOperationException("LegacyServiceRecipients contains at least one unmatched service recipient");
            }

            System.Diagnostics.Trace.WriteLine("Migrating service recipients end");
        }

        private void MigrateOrderItemsAndRecipients()
        {
            System.Diagnostics.Trace.WriteLine("Migrating order items and recipients start");

            using var context = GetContext();

            var orderItemRecipients = context.OrderItemRecipients.ToList();

            foreach (var orderItemRecipient in orderItemRecipients)
                context.OrderItemRecipients.Remove(orderItemRecipient);

            context.SaveChangesWithoutAudit();

            var orderItems = context.OrderItems.ToList();

            foreach (var orderItem in orderItems)
                context.OrderItems.Remove(orderItem);

            context.SaveChangesWithoutAudit();

            var catalogueItems = context.CatalogueItems.ToList();
            var cataloguePrices = context.CataloguePrices.ToList();
            var pricingUnits = context.PricingUnits.ToList();
            var currentOrders = GetCurrentOrders(context);

            foreach (var legacyOrderItem in validLegacyOrderItems)
            {
                if (!currentOrders.Any(x => x.Id == legacyOrderItem.OrderId))
                {
                    if (testLegacyOrders.Any(x => x.Id == legacyOrderItem.OrderId))
                        System.Diagnostics.Trace.WriteLine($"Information!!!. Not migrating order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}. Order does not exist in current database. Original order is test data");
                    else
                        System.Diagnostics.Trace.WriteLine($"Warning!!!. Not migrating order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}. Order does not exist in current database");

                    continue;
                }

                if (!catalogueItems.Any(x => x.Id == EntityFramework.Ordering.Models.CatalogueItemId.ParseExact(legacyOrderItem.CatalogueItemId)))
                {
                    System.Diagnostics.Trace.WriteLine($"Warning!!!. Can't migrate order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}. CatalogueItem does not exist in current database.");
                    continue;
                }

                System.Diagnostics.Trace.WriteLine($"Migrating order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}");

                var orderItem = new EntityFramework.Ordering.Models.OrderItem
                {
                    OrderId = legacyOrderItem.OrderId,
                    CatalogueItemId = EntityFramework.Ordering.Models.CatalogueItemId.ParseExact(legacyOrderItem.CatalogueItemId),
                    Price = legacyOrderItem.Price,
                    EstimationPeriod = legacyOrderItem.EstimationPeriodId == null ? null : (legacyOrderItem.EstimationPeriodId == 1 ? EntityFramework.Catalogue.Models.TimeUnit.PerMonth : EntityFramework.Catalogue.Models.TimeUnit.PerYear),
                    DefaultDeliveryDate = legacyOrderItem.DefaultDeliveryDate,
                    Created = legacyOrderItem.Created,
                    LastUpdated = legacyOrderItem.LastUpdated,
                };

                if (legacyOrderItem.PriceId is not null)
                {
                    orderItem.PriceId = legacyOrderItem.PriceId.Value;

                    if (!cataloguePrices.Any(x => x.CataloguePriceId == orderItem.PriceId))
                    {
                        System.Diagnostics.Trace.WriteLine($"Warning!!!. Can't migrate order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}. CataloguePrice {orderItem.PriceId} does not exist in current database.");
                        continue;
                    }
                }
                else
                {
                    var pricingUnit = pricingUnits.SingleOrDefault(x => x.Name == legacyOrderItem.PricingUnitName);

                    if (pricingUnit is null)
                    {
                        System.Diagnostics.Trace.WriteLine($"Warning!!!. Can't migrate order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}. PricingUnitName {legacyOrderItem.PricingUnitName} does not exist in current database.");
                        continue;
                    }

                    var cataloguePrice = cataloguePrices.SingleOrDefault(x =>
                        x.CatalogueItemId == EntityFramework.Ordering.Models.CatalogueItemId.ParseExact(legacyOrderItem.CatalogueItemId)
                        && x.ProvisioningType == (EntityFramework.Catalogue.Models.ProvisioningType)legacyOrderItem.ProvisioningTypeId
                        && x.CataloguePriceType == (EntityFramework.Catalogue.Models.CataloguePriceType)legacyOrderItem.CataloguePriceTypeId
                        && x.PricingUnit == pricingUnit);

                    if (cataloguePrice is null)
                    {
                        System.Diagnostics.Trace.WriteLine($"Warning!!!. Can't migrate order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}. CataloguePrice does not exist in current database. ProvisioningType {legacyOrderItem.ProvisioningTypeId}, CatalogePriceTypeId = {legacyOrderItem.CataloguePriceTypeId}");
                        continue;
                    }

                    orderItem.PriceId = cataloguePrice.CataloguePriceId;
                }

                context.OrderItems.Add(orderItem);

                var newOrderItemRecipients = validLegacyOrderItemRecipients
                    .Where(x => x.OrderId == orderItem.OrderId && x.CatalogueItemId == legacyOrderItem.CatalogueItemId)
                    .Select(x => new EntityFramework.Ordering.Models.OrderItemRecipient
                    {
                        OrderId = orderItem.OrderId,
                        CatalogueItemId = EntityFramework.Ordering.Models.CatalogueItemId.ParseExact(x.CatalogueItemId),
                        OdsCode = x.OdsCode,
                        Quantity = x.Quantity,
                        DeliveryDate = x.DeliveryDate
                    });

                context.OrderItemRecipients.AddRange(newOrderItemRecipients);
            }

            context.SaveChangesWithoutAudit();

            System.Diagnostics.Trace.WriteLine("Migrating order items and recipients end");
        }

        private void UpdateOrderCompletedDate(int orderId, DateTime completedDate)
        {
            using var sqlConnection = new SqlConnection(GPITBuyingCatalogueConnectionString);
            sqlConnection.Open();
            sqlConnection.Execute("update ordering.Orders set Completed = @val where Id = @id", new { val = completedDate, id = orderId });
        }
    }
}
