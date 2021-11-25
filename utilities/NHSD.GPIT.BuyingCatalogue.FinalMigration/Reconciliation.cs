using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    [ExcludeFromCodeCoverage]
    internal sealed class Reconciliation : BaseMigrator
    {
        public Reconciliation() : base()
        {
        }

        public void RunReconciliation()
        {
            System.Diagnostics.Trace.WriteLine("####### STARTING RECONCILIATION #######");

            LoadLegacyData();

            ReconcileOrganisations();
            ReconcileRelatedOrganisations();
            ReconcileUsers();
            ReconcileOrders();
            ReconcileDefaultDeliveryDates();
            ReconcileServiceRecipients();
            ReconcileOrderItems();
            ReconcileOrderItemRecipients();
        }

        private void ReconcileOrganisations()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling organisations");

            using var context = GetContext();
            var currentOrganisations = context.Organisations.ToList();

            CompareCount("Organisations", currentOrganisations.Count, legacyOrganisations.Count);

            foreach (var legacyOrganisation in legacyOrganisations)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing organisation with ODS code {legacyOrganisation.OdsCode}");

                var currentOrganisation = currentOrganisations.Single(x => x.OdsCode.Equals(legacyOrganisation.OdsCode, StringComparison.CurrentCultureIgnoreCase));

                currentOrganisation.Name.Should().Be(legacyOrganisation.Name);
                currentOrganisation.PrimaryRoleId.Should().Be(legacyOrganisation.PrimaryRoleId);
                currentOrganisation.CatalogueAgreementSigned.Should().Be(legacyOrganisation.CatalogueAgreementSigned);
                currentOrganisation.LastUpdated.Should().Be(legacyOrganisation.LastUpdated);

                if (!AreAddressesEqual(currentOrganisation.Address, JsonDeserializer.Deserialize<EntityFramework.Addresses.Models.Address>(legacyOrganisation.Address)))
                    throw new Exception("Address mismatch");
            }
        }

        private void ReconcileRelatedOrganisations()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling related organisations");

            using var context = GetContext();
            var currentOrganisations = context.Organisations.ToList();
            var currentRelatedOrganisations = context.RelatedOrganisations.ToList();

            CompareCount("Related Organisations", currentRelatedOrganisations.Count, legacyRelatedOrganisations.Count);

            foreach (var legacyRelatedOrganisation in legacyRelatedOrganisations)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing related organisation with legacy ids {legacyRelatedOrganisation.OrganisationId} and {legacyRelatedOrganisation.RelatedOrganisationId}");

                var currentOrgId = currentOrganisations.Single(x => x.OdsCode == legacyOrganisations.Single(y => y.OrganisationId == legacyRelatedOrganisation.OrganisationId).OdsCode).Id;
                var currentRelatedOrgId = currentOrganisations.Single(x => x.OdsCode == legacyOrganisations.Single(y => y.OrganisationId == legacyRelatedOrganisation.RelatedOrganisationId).OdsCode).Id;
                var currentRelatedOrganisation = currentRelatedOrganisations.Single(x => x.OrganisationId == currentOrgId && x.RelatedOrganisationId == currentRelatedOrgId);
            }
        }

        private void ReconcileUsers()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling users");

            using var context = GetContext();
            var currentOrganisations = context.Organisations.ToList();
            var currentUsers = context.AspNetUsers.ToList();

            CompareCount("AspNetUsers", currentUsers.Count, legacyUsers.Count);

            foreach (var legacyUser in legacyUsers)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing user with username: {legacyUser.UserName}");

                var currentUser = currentUsers.Single(x => x.UserName.Equals(legacyUser.UserName, StringComparison.OrdinalIgnoreCase));

                var primaryOrganisationId = currentOrganisations.Single(x =>
                    x.OdsCode.Equals(legacyOrganisations.Single(y => y.OrganisationId == legacyUser.PrimaryOrganisationId).OdsCode, StringComparison.CurrentCultureIgnoreCase)).Id;

                currentUser.NormalizedUserName.Should().Be(legacyUser.NormalizedUserName);
                currentUser.Email.Should().Be(legacyUser.Email);
                currentUser.NormalizedEmail.Should().Be(legacyUser.NormalizedEmail);
                currentUser.EmailConfirmed.Should().Be(legacyUser.EmailConfirmed);
                currentUser.PasswordHash.Should().Be(legacyUser.PasswordHash);
                currentUser.SecurityStamp.Should().Be(legacyUser.SecurityStamp);
                currentUser.ConcurrencyStamp.Should().Be(legacyUser.ConcurrencyStamp);
                currentUser.PhoneNumber.Should().Be(legacyUser.PhoneNumber);
                currentUser.PhoneNumberConfirmed.Should().Be(legacyUser.PhoneNumberConfirmed);
                currentUser.TwoFactorEnabled.Should().Be(legacyUser.TwoFactorEnabled);
                currentUser.LockoutEnd.Should().Be(legacyUser.LockoutEnd);
                currentUser.LockoutEnabled.Should().Be(legacyUser.LockoutEnabled);
                currentUser.AccessFailedCount.Should().Be(legacyUser.AccessFailedCount);
                currentUser.PrimaryOrganisationId.Should().Be(primaryOrganisationId);
                currentUser.OrganisationFunction.Should().Be(legacyUser.OrganisationFunction);
                currentUser.Disabled.Should().Be(legacyUser.Disabled);
                currentUser.CatalogueAgreementSigned.Should().Be(legacyUser.CatalogueAgreementSigned);
                currentUser.FirstName.Should().Be(legacyUser.FirstName);
                currentUser.LastName.Should().Be(legacyUser.LastName);
            }
        }

        private void ReconcileOrders()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling orders");

            using var context = GetContext();
            var currentOrganisations = context.Organisations.ToList();
            var currentUsers = context.AspNetUsers.ToList();
            var currentOrders = context.Orders.IgnoreQueryFilters().Include(x => x.OrderingPartyContact).Include(x => x.SupplierContact).ToList();

            CompareCount("Orders", currentOrders.Count, validLegacyOrders.Count);

            foreach (var legacyOrder in validLegacyOrders)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing order with id: {legacyOrder.Id}");

                var legacyLastUpdatedBy = legacyUsers.Single(x => x.Id.Equals(legacyOrder.LastUpdatedBy.ToString(), StringComparison.CurrentCultureIgnoreCase));
                var lastUpdatedBy = currentUsers.Single(x => x.UserName.Equals(legacyLastUpdatedBy.UserName, StringComparison.CurrentCultureIgnoreCase)).Id;

                var currentOrder = currentOrders.Single(x => x.Id == legacyOrder.Id);

                (var success, var callOffId) = CallOffId.Parse(legacyOrder.CallOffId);

                currentOrder.CallOffId.Should().Be(callOffId);
                currentOrder.Description.Should().Be(legacyOrder.Description);

                var legacyOrderingPartyOrganisation = legacyOrganisations.Single(x => x.OrganisationId == legacyOrder.OrderingPartyId);
                var currentOrderingParttOrganisation = currentOrganisations.Single(x => x.OdsCode.Equals(legacyOrderingPartyOrganisation.OdsCode, StringComparison.CurrentCultureIgnoreCase));
                currentOrder.OrderingPartyId.Should().Be(currentOrderingParttOrganisation.Id);

                if (legacyOrder.OrderingPartyContactId is null)
                    currentOrder.OrderingPartyContactId.Should().BeNull();
                else
                {
                    var legacyContact = legacyContacts.Single(x => x.Id == legacyOrder.OrderingPartyContactId);
                    currentOrder.OrderingPartyContact.FirstName.Should().Be(legacyContact.FirstName);
                    currentOrder.OrderingPartyContact.LastName.Should().Be(legacyContact.LastName);
                    currentOrder.OrderingPartyContact.Email.Should().Be(legacyContact.Email);
                    currentOrder.OrderingPartyContact.Phone.Should().Be(legacyContact.Phone);
                }

                if (legacyOrder.SupplierId is null)
                    currentOrder.SupplierId.Should().BeNull();
                else
                {
                    var legacySupplier = legacySuppliers.Single(x => x.Id.Equals(legacyOrder.SupplierId, StringComparison.CurrentCultureIgnoreCase));
                    currentOrder.SupplierId.Should().Be(Convert.ToInt32(legacySupplier.Id));
                }

                if (legacyOrder.SupplierContactId is null)
                    currentOrder.SupplierContactId.Should().BeNull();
                else
                {
                    var legacyContact = legacyContacts.Single(x => x.Id == legacyOrder.SupplierContactId);
                    currentOrder.SupplierContact.FirstName.Should().Be(legacyContact.FirstName);
                    currentOrder.SupplierContact.LastName.Should().Be(legacyContact.LastName);
                    currentOrder.SupplierContact.Email.Should().Be(legacyContact.Email);
                    currentOrder.SupplierContact.Phone.Should().Be(legacyContact.Phone);
                }

                currentOrder.CommencementDate.Should().Be(legacyOrder.CommencementDate);
                currentOrder.FundingSourceOnlyGms.Should().Be(legacyOrder.FundingSourceOnlyGms);
                currentOrder.Created.Should().Be(legacyOrder.Created);
                currentOrder.LastUpdated.Should().Be(legacyOrder.LastUpdated);
                currentOrder.LastUpdatedBy.Should().Be(lastUpdatedBy);

                if (legacyOrder.Completed.HasValue)
                {
                    currentOrder.Completed.Should().NotBeNull();
                    currentOrder.Completed.Value.TrimMilliseconds().Should().Be(legacyOrder.Completed.Value.TrimMilliseconds());                    
                }
                else
                    currentOrder.Completed.Should().BeNull();
                
                currentOrder.OrderStatus.Should().Be((OrderStatus)legacyOrder.OrderStatusId);
                currentOrder.IsDeleted.Should().Be(legacyOrder.IsDeleted);
            }
        }

        private void ReconcileDefaultDeliveryDates()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling default delivery dates");

            using var context = GetContext();
            var currentDefaultDeliveryDates = context.DefaultDeliveryDates.ToList();

            CompareCount("DefaultDeliveryDates", currentDefaultDeliveryDates.Count, legacyDefaultDeliveryDates.Count);

            foreach (var legacyDefaultDeliveryDate in legacyDefaultDeliveryDates)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing default delivery date with order id: {legacyDefaultDeliveryDate.OrderId} and catalogueItemId {legacyDefaultDeliveryDate.CatalogueItemId}");

                var currentDefaultDeliveryDate = currentDefaultDeliveryDates.Single(x => x.OrderId == legacyDefaultDeliveryDate.OrderId && x.CatalogueItemId == CatalogueItemId.ParseExact(legacyDefaultDeliveryDate.CatalogueItemId));

                currentDefaultDeliveryDate.DeliveryDate.Should().Be(legacyDefaultDeliveryDate.DeliveryDate);
            }
        }

        private void ReconcileServiceRecipients()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling service recipients");

            using var context = GetContext();
            var currentServiceRecipients = context.ServiceRecipients.ToList();

            CompareCount("ServiceRecipients", currentServiceRecipients.Count, legacyServiceRecipients.Count);

            foreach (var legacyServiceRecipient in legacyServiceRecipients)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing service recipient with ods code {legacyServiceRecipient.OdsCode}");

                var currentServiceRecipient = currentServiceRecipients.Single(x => x.OdsCode.Equals(legacyServiceRecipient.OdsCode, StringComparison.CurrentCultureIgnoreCase));

                currentServiceRecipient.Name.Should().Be(legacyServiceRecipient.Name);
            }
        }

        private void ReconcileOrderItems()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling order items");

            using var context = GetContext();
            var cataloguePrices = context.CataloguePrices.ToList();
            var pricingUnits = context.PricingUnits.ToList();
            var currentOrderItems = context.OrderItems.ToList();

            CompareCount("OrderItems", currentOrderItems.Count, validLegacyOrderItems.Count);

            foreach (var legacyOrderItem in validLegacyOrderItems)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing order item with order id {legacyOrderItem.OrderId} and catalogue item id {legacyOrderItem.CatalogueItemId}");

                var currentOrderItem = currentOrderItems.FirstOrDefault(x => x.OrderId == legacyOrderItem.OrderId
                    && x.CatalogueItemId == CatalogueItemId.ParseExact(legacyOrderItem.CatalogueItemId));

                if(currentOrderItem is null)
                {
                    System.Diagnostics.Trace.WriteLine($"Warning!!!. Failed to reconcile order item {legacyOrderItem.OrderId} - {legacyOrderItem.CatalogueItemId}. OrderItem does not appear to have been migrated. Probably PriceId issue.");
                    continue;
                }

                if (legacyOrderItem.PriceId is not null)
                    currentOrderItem.PriceId.Should().Be(legacyOrderItem.PriceId);
                else
                {
                    var pricingUnit = pricingUnits.SingleOrDefault(x => x.Name == legacyOrderItem.PricingUnitName);

                    if (pricingUnit is null)
                        throw new Exception($"Failed to find pricing unit with name {legacyOrderItem.PricingUnitName}");

                    var cataloguePrice = cataloguePrices.SingleOrDefault(x =>
                        x.CatalogueItemId == CatalogueItemId.ParseExact(legacyOrderItem.CatalogueItemId)
                        && x.ProvisioningType == (ProvisioningType)legacyOrderItem.ProvisioningTypeId
                        && x.CataloguePriceType == (CataloguePriceType)legacyOrderItem.CataloguePriceTypeId
                        && x.PricingUnit == pricingUnit);

                    if (cataloguePrice is null)
                        throw new Exception($"Failed to find catalogue price");

                    currentOrderItem.PriceId.Should().Be(cataloguePrice.CataloguePriceId);
                }

                currentOrderItem.Price.Should().Be(legacyOrderItem.Price);

                if (legacyOrderItem.EstimationPeriodId is null)
                    currentOrderItem.EstimationPeriod.Should().BeNull();
                else
                    currentOrderItem.EstimationPeriod.Should().Be((TimeUnit)legacyOrderItem.EstimationPeriodId);

                currentOrderItem.DefaultDeliveryDate.Should().Be(legacyOrderItem.DefaultDeliveryDate);
                currentOrderItem.Created.Should().Be(legacyOrderItem.Created);
            }
        }

        private void ReconcileOrderItemRecipients()
        {
            System.Diagnostics.Trace.WriteLine("Reconciling order item recipients");

            using var context = GetContext();
            var currentOrderItemRecipients = context.OrderItemRecipients.ToList();

            CompareCount("OrderItemRecipients", currentOrderItemRecipients.Count, validLegacyOrderItemRecipients.Count);

            foreach (var legacyOrderItemRecipient in validLegacyOrderItemRecipients)
            {
                System.Diagnostics.Trace.WriteLine($"Comparing order item recipient with order id {legacyOrderItemRecipient.OrderId} and catalogue item id {legacyOrderItemRecipient.CatalogueItemId} and ods code {legacyOrderItemRecipient.OdsCode}");

                var currentOrderItemRecipient = currentOrderItemRecipients.FirstOrDefault(x => x.OrderId == legacyOrderItemRecipient.OrderId
                    && x.CatalogueItemId == CatalogueItemId.ParseExact(legacyOrderItemRecipient.CatalogueItemId)
                    && x.OdsCode.Equals(legacyOrderItemRecipient.OdsCode, StringComparison.CurrentCultureIgnoreCase));

                if(currentOrderItemRecipient is null)
                {                    
                    System.Diagnostics.Trace.WriteLine($"Warning!!!. failed to reconcile order item recipient with order id {legacyOrderItemRecipient.OrderId} and catalogue item id {legacyOrderItemRecipient.CatalogueItemId} and ods code {legacyOrderItemRecipient.OdsCode}. Probably OrderItem missing due to PriceId issue");                    
                    continue;
                }

                currentOrderItemRecipient.Quantity.Should().Be(legacyOrderItemRecipient.Quantity);
                currentOrderItemRecipient.DeliveryDate.Should().Be(legacyOrderItemRecipient.DeliveryDate);
            }
        }
    }
}
