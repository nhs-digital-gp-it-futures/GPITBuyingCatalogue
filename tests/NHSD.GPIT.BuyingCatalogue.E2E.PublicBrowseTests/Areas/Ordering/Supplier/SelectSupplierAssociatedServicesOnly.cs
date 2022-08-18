﻿using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Supplier
{
    public class SelectSupplierAssociatedServicesOnly : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private const string SearchTerm = "E2E Test Supplier";
        private const string ExpectedResult = "E2E Test Supplier With Contact";
        private static readonly CallOffId CallOffId = new(90019, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public SelectSupplierAssociatedServicesOnly(LocalWebApplicationFactory factory)
            : base(factory, typeof(SupplierController), nameof(SupplierController.SelectSupplier), Parameters)
        {
        }

        [Fact]
        public void SelectSupplierAssociatedServicesOnly_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Select supplier - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierObjects.AssociatedServicesInset).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierObjects.SupplierAutoComplete).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectSupplierAssociatedServicesOnly_FilterSuppliers_WithMatches_ExpectedResult()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SearchTerm);
            CommonActions.WaitUntilElementIsDisplayed(SupplierObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(SupplierObjects.SearchResult(0)).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierObjects.SearchResult(1)).Should().BeFalse();
            CommonActions.ElementTextEqualTo(SupplierObjects.SearchResult(0), ExpectedResult).Should().BeTrue();
        }
    }
}
