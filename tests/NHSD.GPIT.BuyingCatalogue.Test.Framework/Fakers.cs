using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using Bogus.Extensions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework
{
    public static class Fakers
    {
        private static readonly Fixture Fixture = new();
        private static readonly Random Random = new();

        static Fakers()
        {
            Fixture.Customize(
                new CompositeCustomization(
                    new AutoMoqCustomization(),
                    new IgnoreCircularReferenceCustomisation(),
                    new CallOffIdCustomization(),
                    new CatalogueItemIdCustomization()
                ));
        }

        public static readonly Faker<AdditionalService> AdditionalService = new Faker<AdditionalService>()
                .RuleFor(add => add.CatalogueItem, _ => new CatalogueItem())
                .RuleFor(add => add.FullDescription, f => f.Lorem.Paragraphs())
                .RuleFor(add => add.LastUpdated, f => f.Date.Recent())
                .RuleFor(add => add.LastUpdatedBy, _ => Guid.NewGuid())
                .RuleFor(add => add.Solution, f => Solution.Generate())
                .RuleFor(add => add.Summary, f => f.Lorem.Paragraph());
        
        public static readonly Faker<AssociatedService> AssociatedService = new Faker<AssociatedService>()
            .RuleFor(add => add.AssociatedServiceNavigation, _ => new CatalogueItem())
            .RuleFor(add => add.Description, f => f.Lorem.Paragraph())
            .RuleFor(add => add.OrderGuidance, f => f.Lorem.Paragraphs())
            .RuleFor(add => add.LastUpdated, f => f.Date.Recent())
            .RuleFor(add => add.LastUpdatedBy, _ => Guid.NewGuid());
        
        public static readonly Faker<CatalogueItem> CatalogueItem = new Faker<CatalogueItem>()
            .RuleFor(ci => ci.AdditionalService, _ => AdditionalService.Generate())
            .RuleFor(ci => ci.AssociatedService, _ => AssociatedService.Generate())
            .RuleFor(ci => ci.CataloguePrices, _ => CataloguePrice.GenerateBetween(4, 9))
            .RuleFor(ci => ci.Created, f => f.Date.Recent())
            .RuleFor(ci => ci.Name, f => f.Commerce.ProductName())
            .RuleFor(ci => ci.PublishedStatus, PublicationStatus.Published)
            .RuleFor(ci => ci.Solution, _ => Solution.Generate())
            .RuleFor(ci => ci.Supplier, _ => Fixture.Build<Supplier>()
                .With(s => s.CatalogueItems,
                    Enumerable.Range(1, Random.Next(2, 6))
                        .ToList()
                        .Select(
                            i => new CatalogueItem
                            {
                                AssociatedService = AssociatedService.Generate(),
                                CataloguePrices = new List<CataloguePrice>(),
                            })
                        .ToList)
                .Without(s => s.SupplierContacts)
                .Create());

        public static readonly Faker<CataloguePrice> CataloguePrice = new Faker<CataloguePrice>()
            .RuleFor(cp => cp.CataloguePriceType, CataloguePriceType.Flat)
            .RuleFor(cp => cp.CurrencyCode, f => new[] { "GBP", "USD", "EUR", }[Random.Next(0, 3)])
            .RuleFor(cp => cp.Price, f => f.Random.Decimal())
            .RuleFor(cp => cp.PricingUnit, f => Fixture.Build<PricingUnit>().Without(pu => pu.CataloguePrices).Create())
            .RuleFor(cp => cp.TimeUnit, f => Fixture.Create<TimeUnit>());
        
        public static readonly Faker<Integration> Integration = new Faker<Integration>()
            .RuleFor(i => i.Link, f => f.Internet.Url())
            .RuleFor(i => i.Name, f => f.Commerce.Product())
            .RuleFor(i => i.SubTypes, _ => Fixture.CreateMany<IntegrationSubType>().ToArray());

        public static readonly Faker<Solution> Solution = new Faker<Solution>()
                .RuleFor(s => s.ClientApplication, f => JsonConvert.SerializeObject(
                    Fixture
                        .Build<ClientApplication>()
                        .With(
                            ca => ca.BrowsersSupported,
                            new HashSet<string>
                            {
                                "Internet Explorer 11",
                                "Google Chrome",
                                "OPERA",
                                "safari",
                                "mozilla firefox"
                            })
                        .With(ca => ca.ClientApplicationTypes, GetClientApplicationTypes())
                        .With(
                            ca => ca.MobileConnectionDetails,
                            Fixture.Build<MobileConnectionDetails>()
                                .With(
                                    m => m.ConnectionType,
                                    new HashSet<string> { "5g", "lte", "GpRS", "wifi" })
                                .Create())
                        .With(
                            ca => ca.MobileOperatingSystems,
                            Fixture.Build<MobileOperatingSystems>()
                                .With(m => m.OperatingSystems, new HashSet<string> { "andrOID", "Apple ios", })
                                .Create())
                        .Create()
                ))
            .RuleFor(s => s.Features, _ => JsonConvert.SerializeObject(Fixture.Create<string[]>()))
            .RuleFor(s => s.Hosting, _ => JsonConvert.SerializeObject(Fixture.Create<Hosting>()))
            .RuleFor(s => s.ImplementationDetail, f => f.Lorem.Paragraphs())
            .RuleFor(s => s.Integrations, _ => JsonConvert.SerializeObject(Integration.GenerateBetween(2, 5)))
            .RuleFor(s => s.ServiceLevelAgreement, f => f.Lorem.Paragraph())
            .RuleFor(s => s.SolutionCapabilities, f => Fixture.Build<SolutionCapability>()
                    .Without(sc => sc.Capability)
                    .Without(sc => sc.Solution)
                    .Without(sc => sc.Status)
                    .CreateMany().ToList())
            ;
        
        private static HashSet<string> GetClientApplicationTypes()
        {
            var result = new HashSet<string>();

            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("browser-BASED");
            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("NATive-mobile");
            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("native-DESKtop");

            return result;
        }
        
        private static List<Integration> GetIntegrations(IFixture fixture)
        {
            var result = fixture.Build<Integration>()
                .With(i => i.SubTypes, fixture.CreateMany<IntegrationSubType>().ToArray)
                .CreateMany()
                .ToList();

            foreach (var integration in result)
            {
                for (int i = 0; i < integration.SubTypes.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        integration.SubTypes[i].DetailsDictionary.Clear();
                    }
                    else
                    {
                        integration.SubTypes[i].DetailsSystemDictionary.Clear();
                    }
                }
            }
            
            return result;
        }
    }
}
