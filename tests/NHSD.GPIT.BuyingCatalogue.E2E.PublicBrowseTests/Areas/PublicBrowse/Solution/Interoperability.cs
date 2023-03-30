using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class Interoperability : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public Interoperability(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.Interoperability),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public void Interoperability_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions
                    .ElementIsDisplayed(CommonSelectors.Header2)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.IMIntegrationsHeading)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.GpConnectHeading)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.TableAppointmentBooking)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.TableBulk)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.TableHtmlView)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.TablePatientFacing)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.TableStructuredRecord)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementIsDisplayed(InteroperabilityObjects.TableTransactional)
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void Interoperability_NoIntegrations_NoSectionsDisplayed()
        {
            RunTest(() =>
            {
                using var context = GetEndToEndDbContext();
                var solution = context.Solutions.First(s => s.CatalogueItemId == SolutionId);
                var originalIntegrations = solution.Integrations;
                solution.Integrations = null;
                context.SaveChanges();

                Driver.Navigate().Refresh();

                CommonActions
                    .ElementExists(InteroperabilityObjects.IMIntegrationsHeading)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(InteroperabilityObjects.GpConnectHeading)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(InteroperabilityObjects.TableAppointmentBooking)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(InteroperabilityObjects.TableBulk)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(InteroperabilityObjects.TableHtmlView)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(InteroperabilityObjects.TablePatientFacing)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(InteroperabilityObjects.TableStructuredRecord)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(InteroperabilityObjects.TableTransactional)
                    .Should()
                    .BeFalse();

                solution.Integrations = originalIntegrations;
                context.SaveChanges();
            });
        }

        [Fact]
        public async Task Interoperability_SolutionIsSuspended_Redirect()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);
                solution.PublishedStatus = PublicationStatus.Suspended;
                await context.SaveChangesAsync();

                Driver.Navigate().Refresh();

                CommonActions
                    .PageLoadedCorrectGetIndex(
                        typeof(SolutionsController),
                        nameof(SolutionsController.Description))
                    .Should()
                    .BeTrue();
            });
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.First(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
