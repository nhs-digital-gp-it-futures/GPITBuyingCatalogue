using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.ProxyBuyer
{
    [Collection(nameof(OrderingCollection))]
    public class OrderSelectOrganisation
        : BuyerTestBase
    {
        private const string InternalOrgId = "CG-15F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderSelectOrganisation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderTriageController),
                  nameof(OrderTriageController.SelectOrganisation),
                  Parameters,
                  UserSeedData.AliceEmail)
        {
        }

        [Fact]
        public void SelectOrganisation_AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.Include(o => o.RelatedOrganisationOrganisations).First(o => o.InternalIdentifier == InternalOrgId);
            var expectedNumberOfRadioOptions = organisation.RelatedOrganisationOrganisations.Count + 1;

            CommonActions.PageTitle()
                .Should()
                .Be("Which organisation are you ordering for?".FormatForComparison());

            CommonActions.GetNumberOfRadioButtonsDisplayed()
                .Should()
                .Be(expectedNumberOfRadioOptions);

            CommonActions.SaveButtonDisplayed();
        }

        [Fact]
        public void SelectOrganisation_Save_RedirectsToOrderTasklist()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart))
                .Should().BeTrue();
        }
    }
}
