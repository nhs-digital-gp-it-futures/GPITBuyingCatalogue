using System;
using System.Linq;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class SolutionCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Solution> composer) => composer
                .FromFactory(new SolutionSpecimenBuilder())
                .Without(s => s.AdditionalServices)
                .Without(s => s.CatalogueItem)
                .Without(s => s.CatalogueItemId)
                .Without(s => s.ClientApplication)
                .Without(s => s.Features)
                .Without(s => s.FrameworkSolutions)
                .Without(s => s.Integrations)
                .Without(s => s.LastUpdatedByUser)
                .Without(s => s.MarketingContacts)
                .Without(s => s.ServiceLevelAgreement);

            fixture.Customize<Solution>(ComposerTransformation);
        }

        private sealed class SolutionSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(Solution)))
                    return new NoSpecimen();

                var catalogueItem = context.Create<CatalogueItem>();
                var solution = new Solution();

                catalogueItem.CatalogueItemType = CatalogueItemType.Solution;
                catalogueItem.Solution = solution;

                solution.CatalogueItem = catalogueItem;
                solution.CatalogueItemId = catalogueItem.Id;
                solution.ClientApplication = JsonSerializer.Serialize(context.Create<ClientApplication>());
                solution.Features = JsonSerializer.Serialize(context.Create<string[]>());
                solution.Integrations = JsonSerializer.Serialize(context.CreateMany<Integration>());

                AddAdditionalServices(solution, context);
                AddFrameworkSolutions(solution, context);
                AddMarketingContacts(solution, context);
                InitializeSupplier(solution);

                return solution;
            }

            private static void AddAdditionalServices(Solution solution, ISpecimenContext context)
            {
                var additionalServices = context.CreateMany<AdditionalService>().ToList();
                additionalServices.ForEach(a =>
                {
                    solution.AdditionalServices.Add(a);

                    a.CatalogueItem.Supplier = solution.CatalogueItem.Supplier;
                    a.CatalogueItem.SupplierId = solution.CatalogueItem.SupplierId;
                    a.Solution = solution;
                    a.SolutionId = solution.CatalogueItemId;
                });
            }

            private static void AddFrameworkSolutions(Solution solution, ISpecimenContext context)
            {
                var frameworkSolutions = context.CreateMany<FrameworkSolution>().ToList();
                frameworkSolutions.ForEach(f =>
                {
                    solution.FrameworkSolutions.Add(f);
                    f.Solution = solution;
                    f.SolutionId = solution.CatalogueItemId;
                });
            }

            private static void AddMarketingContacts(Solution solution, ISpecimenContext context)
            {
                var marketingContacts = context.CreateMany<MarketingContact>().ToList();
                marketingContacts.ForEach(mc =>
                {
                    solution.MarketingContacts.Add(mc);
                    mc.SolutionId = solution.CatalogueItemId;
                });
            }

            private static void InitializeSupplier(Solution solution)
            {
                foreach (var additionalService in solution.AdditionalServices)
                {
                    var catalogueItem = additionalService.CatalogueItem;
                    var supplier = solution.CatalogueItem.Supplier;

                    catalogueItem.Supplier = supplier;
                    catalogueItem.SupplierId = supplier.Id;
                    supplier.CatalogueItems.Add(catalogueItem);
                }
            }
        }
    }
}
