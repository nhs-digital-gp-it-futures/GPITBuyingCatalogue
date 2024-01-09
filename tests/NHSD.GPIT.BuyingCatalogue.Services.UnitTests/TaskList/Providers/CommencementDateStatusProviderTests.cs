using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class CommencementDateStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            CommencementDateStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            CommencementDateStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            CommencementDateStatusProvider service)
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
        public static void Get_SupplierStatusNotComplete_ReturnsCannotStart(
            TaskProgress supplierStatus,
            Order order,
            CommencementDateStatusProvider service)
        {
            var state = new OrderProgress
            {
                SupplierStatus = supplierStatus,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_TimescalesNotStarted_ReturnsNotStarted(
            TaskProgress supplierStatus,
            Order order,
            CommencementDateStatusProvider service)
        {
            var state = new OrderProgress
            {
                SupplierStatus = supplierStatus,
            };

            order.CommencementDate = null;
            order.MaximumTerm = null;
            order.InitialPeriod = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_OnlyCommencementDateSpecified_ReturnsInProgress(
            TaskProgress supplierStatus,
            Order order,
            CommencementDateStatusProvider service)
        {
            var state = new OrderProgress
            {
                SupplierStatus = supplierStatus,
            };

            order.CommencementDate = DateTime.UtcNow;
            order.MaximumTerm = null;
            order.InitialPeriod = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed, 10, null)]
        [CommonInlineAutoData(TaskProgress.Amended, 10, null)]
        [CommonInlineAutoData(TaskProgress.Completed, null, 6)]
        [CommonInlineAutoData(TaskProgress.Amended, null, 6)]
        public static void Get_MaximumTermAndInitialPeriod_ReturnsInProgress(
            TaskProgress supplierStatus,
            int? maximumTerm,
            int? initialPeriod,
            Order order,
            CommencementDateStatusProvider service)
        {
            var state = new OrderProgress
            {
                SupplierStatus = supplierStatus,
            };

            order.CommencementDate = null;
            order.MaximumTerm = maximumTerm;
            order.InitialPeriod = initialPeriod;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_ReturnsCompleted(
            TaskProgress supplierStatus,
            Order order,
            CommencementDateStatusProvider service)
        {
            var state = new OrderProgress
            {
                SupplierStatus = supplierStatus,
            };

            order.CommencementDate = DateTime.UtcNow;
            order.MaximumTerm = 5;
            order.InitialPeriod = 6;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}
