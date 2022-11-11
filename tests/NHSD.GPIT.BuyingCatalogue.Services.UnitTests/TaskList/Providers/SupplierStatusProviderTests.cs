﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class SupplierStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            SupplierStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            SupplierStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_OrderingPartyStatusNotComplete_ReturnsCannotStart(
            TaskProgress orderingPartyStatus,
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = orderingPartyStatus,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderHasNoSupplier_ReturnsNotStarted(
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = TaskProgress.Completed,
            };

            order.Supplier = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderHasNoSupplierContact_ReturnsInProgress(
            Supplier supplier,
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = TaskProgress.Completed,
            };

            order.Supplier = supplier;
            order.SupplierContact = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderHasSupplierAndSupplierContact_ReturnsCompleted(
            Supplier supplier,
            Contact contact,
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = TaskProgress.Completed,
            };

            order.Supplier = supplier;
            order.SupplierContact = contact;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}