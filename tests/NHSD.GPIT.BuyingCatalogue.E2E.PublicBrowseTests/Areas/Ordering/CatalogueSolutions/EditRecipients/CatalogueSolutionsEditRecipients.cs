using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsEditRecipients
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private const string CatalogueItemName = "E2E With Contact Multiple Prices";
        private static readonly CallOffId CallOffId = new(90006, 01);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters =
        new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public CatalogueSolutionsEditRecipients(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsEditRecipients_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsTable)
                .Should()
                .BeTrue();

            var serviceRecipients = MemoryCache.GetServiceRecipients();

            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(serviceRecipients.Count());

            var model = Session.GetOrderStateFromSession(CallOffId.ToString());

            var selectedRecipientName = model.ServiceRecipients.FirstOrDefault(sr => sr.Selected).Name;

            CommonActions.CheckBoxSelectedByLabelText(selectedRecipientName).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditRecipients_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(CatalogueSolutionsController),
            nameof(CatalogueSolutionsController.EditSolution)).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditRecipients_DeselectAll_ThrowsError()
        {
            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients)).Should().BeTrue();

            CommonActions.AllCheckBoxesSelected().Should().BeFalse();

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsErrorMessage,
                "Error: Select a service recipient").Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditRecipients_DeselectFirst_SelectAnother_ExpectedResult()
        {
            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickCheckboxByLabel("Test Service Recipient Two");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution)).Should().BeTrue();

            var numberOfRecipients =
                Session
                .GetOrderStateFromSession(CallOffId.ToString())
                .ServiceRecipients
                .Where(sr => sr.Selected)
                .Count();

            Driver
            .FindElements(By.ClassName("nhsuk-input"))
            .Count(input => input.GetAttribute("id").EndsWith("Quantity"))
            .Should()
            .Be(numberOfRecipients);

            Driver
            .FindElements(By.ClassName("nhsuk-input"))
            .Where(input => input.GetAttribute("id").EndsWith("Quantity"))
            .All(input => !string.IsNullOrWhiteSpace(input.GetAttribute("value")))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditRecipients_NoChange_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution)).Should().BeTrue();

            var numberOfRecipients =
                Session
                .GetOrderStateFromSession(CallOffId.ToString())
                .ServiceRecipients
                .Where(sr => sr.Selected)
                .Count();

            Driver
            .FindElements(By.ClassName("nhsuk-input"))
            .Count(input => input.GetAttribute("id").EndsWith("Quantity"))
            .Should()
            .Be(numberOfRecipients);
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeServiceRecipientMemoryCacheHandler(InternalOrgId);

            using var context = GetEndToEndDbContext();
            var price = context
                .CataloguePrices
                .Include(cp => cp.PricingUnit)
                .SingleOrDefault(cp => cp.CataloguePriceId == 3);

            var serviceRecipients = MemoryCache.GetServiceRecipients();

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = DateTime.UtcNow.AddDays(1),
                CatalogueItemName = CatalogueItemName,
                CataloguePrice = price,
                ServiceRecipients = new List<OrderItemRecipientModel>(),
                Quantity = 123,
                EstimationPeriod = EntityFramework.Catalogue.Models.TimeUnit.PerMonth,
                IsNewSolution = false,
                HasHitEditSolution = true,
                CurrencySymbol = "£",
                AgreedPrice = 100,
            };

            foreach (var recipient in serviceRecipients)
            {
                model.ServiceRecipients
                    .Add(new OrderItemRecipientModel
                    {
                        Name = recipient.Name,
                        OdsCode = recipient.OrgId,
                    });
            }

            model.ServiceRecipients[0].Selected = true;
            model.ServiceRecipients[0].Quantity = 123;
            model.ServiceRecipients[0].DeliveryDate = DateTime.UtcNow.AddDays(2);

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
