using System;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order;

public class OrderTaskListTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_DescriptionOnly(
        string description)
    {
        var order = new EntityFramework.Ordering.Models.Order { Description = description };

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.CannotStart,
        };
        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_OrderingPartyContact(
        string description,
        Contact orderingPartyContact)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
        };

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.CannotStart,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_OrderingPartyNotCompleted(
        string description)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description, OrderingPartyContact = null,
        };

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed, OrderingPartyStatus = TaskProgress.NotStarted,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_Supplier_InProgress(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
        };

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.InProgress,
            AssociatedServiceBilling = TaskProgress.CannotStart,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_Supplier_Completed(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
        };

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.CannotStart,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_Timescales(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.CannotStart,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SolutionsOrService_InProgress(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        orderItem.OrderItemFunding = null;
        orderItem.OrderItemPrice = null;
        orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);

        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        orderItem.CatalogueItem = solution.CatalogueItem;
        order.OrderItems.Add(orderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.InProgress,
            AssociatedServiceBilling = TaskProgress.CannotStart,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SolutionsOrService_NoFunding(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        orderItem.OrderItemFunding = null;
        orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);

        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        orderItem.CatalogueItem = solution.CatalogueItem;
        order.OrderItems.Add(orderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SolutionsOrService_Funding(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        orderItem.CatalogueItem = solution.CatalogueItem;
        orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);

        order.OrderItems.Add(orderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_FundingSources_NotStarted(
    string description,
    Contact orderingPartyContact,
    EntityFramework.Catalogue.Models.Supplier supplier,
    Contact supplierContact,
    DateTime commencementDate,
    OrderItem solutionOrderItem,
    Solution solution,
    OrderItem additionalServiceOrderItem,
    AdditionalService additionalService)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        additionalServiceOrderItem.CatalogueItem = additionalService.CatalogueItem;

        additionalServiceOrderItem.OrderItemFunding = null;
        solutionOrderItem.OrderItemFunding = null;

        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(additionalServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            FundingSource = TaskProgress.NotStarted,
            ImplementationPlan = TaskProgress.CannotStart,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_FundingSources_InProgress(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem additionalServiceOrderItem,
        AdditionalService additionalService,
        EntityFramework.Catalogue.Models.Framework framework)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SelectedFramework = framework,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        solutionOrderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);

        additionalServiceOrderItem.OrderItemFunding = null;
        solutionOrderItem.OrderItemFunding = null;

        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(additionalServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.InProgress,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_FundingSourceInProgress_WithImplementationPlan(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem additionalServiceOrderItem,
        AdditionalService additionalService,
        EntityFramework.Catalogue.Models.Framework framework)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SelectedFramework = framework,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
            },
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        solutionOrderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);

        additionalServiceOrderItem.CatalogueItem = additionalService.CatalogueItem;
        additionalServiceOrderItem.OrderItemFunding = null;
        additionalServiceOrderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);

        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(additionalServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.NotStarted,
            ImplementationPlan = TaskProgress.InProgress,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_DeliveryDates_NotStarted(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        orderItem.CatalogueItem = solution.CatalogueItem;
        orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);

        order.OrderItems.Add(orderItem);

        var expected = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
        };

        var actual = new OrderTaskList(order);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_DeliveryDates_InProgress(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        orderItem.CatalogueItem = solution.CatalogueItem;
        orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate = null);
        orderItem.OrderItemRecipients.First().DeliveryDate = DateTime.UtcNow;

        order.OrderItems.Add(orderItem);

        var expected = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.InProgress,
            FundingSource = TaskProgress.InProgress,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
        };

        var actual = new OrderTaskList(order);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_ImplementationPlan_NoAssociatedServices(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
            },
        };

        orderItem.CatalogueItem = solution.CatalogueItem;
        order.OrderItems.Add(orderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.Completed,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
            DataProcessingInformation = TaskProgress.NotStarted,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_ImplementationPlan_AssociatedServices(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem associatedServiceOrderItem,
        AssociatedService associatedService)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
            },
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        associatedServiceOrderItem.CatalogueItem = associatedService.CatalogueItem;
        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(associatedServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.Completed,
            AssociatedServiceBilling = TaskProgress.NotStarted,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_AssociatedServiceBilling(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem associatedServiceOrderItem,
        AssociatedService associatedService)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
                UseDefaultBilling = true,
                HasSpecificRequirements = false,
            },
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        associatedServiceOrderItem.CatalogueItem = associatedService.CatalogueItem;
        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(associatedServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.Completed,
            AssociatedServiceBilling = TaskProgress.Completed,
            DataProcessingInformation = TaskProgress.NotStarted,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_OrderChanged_InProgressAssociatedServiceBilling(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem associatedServiceOrderItem,
        AssociatedService associatedService,
        EntityFramework.Catalogue.Models.Framework framework)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SelectedFramework = framework,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
                UseDefaultBilling = true,
                HasSpecificRequirements = false,
            },
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        associatedServiceOrderItem.CatalogueItem = associatedService.CatalogueItem;
        associatedServiceOrderItem.OrderItemPrice = null;
        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(associatedServiceOrderItem);

        associatedServiceOrderItem.OrderItemFunding = null;

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.InProgress,
            DeliveryDates = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.Completed,
            AssociatedServiceBilling = TaskProgress.InProgress,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_AssociatedServiceBilling_FundingSourceInProgress(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem associatedServiceOrderItem,
        AssociatedService associatedService,
        OrderItem additionalServiceOrderItem,
        AdditionalService additionalService,
        EntityFramework.Catalogue.Models.Framework framework)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            SelectedFramework = framework,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
                UseDefaultBilling = true,
                HasSpecificRequirements = false,
            },
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        associatedServiceOrderItem.CatalogueItem = associatedService.CatalogueItem;
        additionalServiceOrderItem.CatalogueItem = additionalService.CatalogueItem;
        additionalServiceOrderItem.OrderItemFunding = null;

        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(associatedServiceOrderItem);
        order.OrderItems.Add(additionalServiceOrderItem);

        order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate = commencementDate.AddDays(1)));

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            FundingSource = TaskProgress.InProgress,
            ImplementationPlan = TaskProgress.InProgress,
            AssociatedServiceBilling = TaskProgress.InProgress,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_AssociatedServiceBilling_ImplementationPlanNotCompleted(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem associatedServiceOrderItem,
        AssociatedService associatedService)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        associatedServiceOrderItem.CatalogueItem = associatedService.CatalogueItem;
        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(associatedServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.NotStarted,
            AssociatedServiceBilling = TaskProgress.CannotStart,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_AssociatedServiceBilling_InProgress(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem associatedServiceOrderItem,
        AssociatedService associatedService)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
                UseDefaultBilling = true,
            },
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        associatedServiceOrderItem.CatalogueItem = associatedService.CatalogueItem;
        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(associatedServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.Completed,
            AssociatedServiceBilling = TaskProgress.InProgress,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_DataProcessingPlan(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
                UseDefaultDataProcessing = true,
            },
        };

        orderItem.CatalogueItem = solution.CatalogueItem;
        order.OrderItems.Add(orderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.Completed,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
            DataProcessingInformation = TaskProgress.Completed,
            ReviewAndCompleteStatus = TaskProgress.NotStarted,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_DataProcessingPlan_InProgress(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem solutionOrderItem,
        Solution solution,
        OrderItem additionalServiceOrderItem,
        AdditionalService additionalService,
        EntityFramework.Catalogue.Models.Framework framework)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            SelectedFramework = framework,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
                UseDefaultDataProcessing = true,
            },
        };

        solutionOrderItem.CatalogueItem = solution.CatalogueItem;
        additionalServiceOrderItem.CatalogueItem = additionalService.CatalogueItem;
        additionalServiceOrderItem.OrderItemFunding = null;
        order.OrderItems.Add(solutionOrderItem);
        order.OrderItems.Add(additionalServiceOrderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            FundingSource = TaskProgress.InProgress,
            ImplementationPlan = TaskProgress.InProgress,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
            DataProcessingInformation = TaskProgress.InProgress,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_Completed(
        string description,
        Contact orderingPartyContact,
        EntityFramework.Catalogue.Models.Supplier supplier,
        Contact supplierContact,
        DateTime commencementDate,
        OrderItem orderItem,
        Solution solution)
    {
        var order = new EntityFramework.Ordering.Models.Order
        {
            Description = description,
            OrderingPartyContact = orderingPartyContact,
            Supplier = supplier,
            SupplierContact = supplierContact,
            CommencementDate = commencementDate,
            ContractFlags = new()
            {
                UseDefaultImplementationPlan = true,
                UseDefaultDataProcessing = true,
            },
        };

        order.Complete();

        orderItem.CatalogueItem = solution.CatalogueItem;
        order.OrderItems.Add(orderItem);

        var expectedModel = new OrderTaskList
        {
            DescriptionStatus = TaskProgress.Completed,
            OrderingPartyStatus = TaskProgress.Completed,
            SupplierStatus = TaskProgress.Completed,
            CommencementDateStatus = TaskProgress.Completed,
            SolutionOrService = TaskProgress.Completed,
            FundingSource = TaskProgress.Completed,
            DeliveryDates = TaskProgress.Completed,
            ImplementationPlan = TaskProgress.Completed,
            AssociatedServiceBilling = TaskProgress.NotApplicable,
            DataProcessingInformation = TaskProgress.Completed,
            ReviewAndCompleteStatus = TaskProgress.Completed,
        };

        var model = new OrderTaskList(order);

        model.Should().BeEquivalentTo(expectedModel);
    }
}
