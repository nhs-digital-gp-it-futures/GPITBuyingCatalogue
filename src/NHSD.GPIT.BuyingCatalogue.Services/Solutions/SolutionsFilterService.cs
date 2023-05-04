﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class SolutionsFilterService : ISolutionsFilterService
    {
        private const string ColumnName = "Id";
        private const string CapabilityParamName = "@CapabilityIds";
        private const string CapabilityParamType = "catalogue.CapabilityIds";
        private const string EpicParamName = "@EpicIds";
        private const string EpicParamType = "catalogue.EpicIds";
        private const string FilterProcName = "catalogue.FilterCatalogueItems";

        private readonly ICapabilitiesService capabilityService;

        private readonly BuyingCatalogueDbContext dbContext;

        public SolutionsFilterService(BuyingCatalogueDbContext dbContext, ICapabilitiesService capabilityService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.capabilityService = capabilityService ?? throw new ArgumentNullException(nameof(capabilityService));
        }
            

        public async Task<(IList<CatalogueItem> CatalogueItems, PageOptions Options, List<CapabilitiesAndCountModel> CapabilitiesAndCount)> GetAllSolutionsFiltered(
            PageOptions options,
            string selectedCapabilityIds = null,
            string selectedEpicIds = null,
            string search = null)
        {
            options ??= new PageOptions();

            var (query, count) = string.IsNullOrWhiteSpace(selectedCapabilityIds)
                ? NonFilteredQuery(dbContext)
                : await FilteredQuery(dbContext, selectedCapabilityIds, selectedEpicIds);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(ci => ci.Supplier.Name.Contains(search) || ci.Name.Contains(search));

            options.TotalNumberOfItems = await query.CountAsync();

            query = options.Sort switch
            {
                PageOptions.SortOptions.LastPublished => query.OrderByDescending(ci => ci.LastPublished)
                                                                .ThenBy(ci => ci.Name),
                PageOptions.SortOptions.ZToA => query.OrderByDescending(ci => ci.Name),
                PageOptions.SortOptions.AtoZ or _ => query.OrderBy(ci => ci.Name),
            };

            if (options.PageNumber != 0)
                query = query.Skip((options.PageNumber - 1) * options.PageSize);

            query = query.Take(options.PageSize);

            var results = await query.ToListAsync();

            return (results, options, count);
        }

        public async Task<List<SearchFilterModel>> GetSolutionsBySearchTerm(string searchTerm, int maxToBringBack = 15)
        {
            var searchBySolutionNameQuery = dbContext.CatalogueItems.AsNoTracking()
                .Where(ci =>
                    ci.Name.Contains(searchTerm)
                    && ci.CatalogueItemType == CatalogueItemType.Solution
                    && (ci.PublishedStatus == PublicationStatus.Published || ci.PublishedStatus == PublicationStatus.InRemediation)
                    && ci.Supplier.IsActive)
                .Select(ci => new SearchFilterModel
                {
                    Title = ci.Name,
                    Category = "Solution",
                });

            var searchBySupplierNameQuery = dbContext.Suppliers.AsNoTracking()
                .Where(s => s.Name.Contains(searchTerm) && s.IsActive)
                .Select(s => new SearchFilterModel
                {
                    Title = s.Name,
                    Category = "Supplier",
                });

            return await searchBySolutionNameQuery
                .Union(searchBySupplierNameQuery)
                .OrderBy(ssfm => ssfm.Title)
                .Take(maxToBringBack)
                .ToListAsync();
        }

        public async Task<string> SaveFilter(
            string name,
            string description,
            string organisationId,
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes)
        {
            var organisation = await dbContext.Organisations.FirstAsync(o => o.InternalIdentifier == organisationId);
            var framework = await dbContext.Frameworks.FirstAsync(o => o.Id == frameworkId);

            var filter = 
                new Filter() 
                {
                    Name = name,
                    Description = description,
                    Organisation = organisation,
                    Framework = framework,
                };

            dbContext.Filters.Add(filter);

            await dbContext.SaveChangesAsync();

            await AddFilterCapabilities(filter.Id, capabilityIds);
            await AddFilterEpics(filter.Id, epicIds);
            await AddFilterClientApplicationTypes(filter.Id, clientApplicationTypes);
            await AddFilterHostingTypes(filter.Id, hostingTypes);

            return filter.Id;
        }

        private async Task AddFilterCapabilities(string filterId, List<int> capabilityIds)
        {
            foreach (var id in capabilityIds)
            {
                var capability = dbContext.Capabilities.First(x => x.Id == id);

                dbContext.FilterCapabilities.Add(new FilterCapability()
                {
                    FilterId = filterId,
                    CapabilityId = id,
                    Capability = capability,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task AddFilterEpics(string filterId, List<string> epicIds)
        {
            foreach (var id in epicIds)
            {
                var epic = dbContext.Epics.First(x => x.Id == id);

                dbContext.FilterEpics.Add(new FilterEpic()
                {
                    FilterId = filterId,
                    EpicId = id,
                    Epic = epic,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task AddFilterClientApplicationTypes(string filterId, List<ClientApplicationType> clientApplicationTypes)
        {
            foreach (var type in clientApplicationTypes)
            {
                dbContext.FilterClientApplicationTypes.Add(new FilterClientApplicationType()
                {
                    FilterId = filterId,
                    ClientApplicationType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task AddFilterHostingTypes(string filterId, List<HostingType> hostingTypes)
        {
            foreach (var type in hostingTypes)
            {
                dbContext.FilterHostingTypes.Add(new FilterHostingType()
                {
                    FilterId = filterId,
                    HostingType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private static (IQueryable<CatalogueItem> Query, List<CapabilitiesAndCountModel> Count) NonFilteredQuery(BuyingCatalogueDbContext dbContext) =>
            (dbContext.CatalogueItems
                .AsNoTracking()
                .AsSplitQuery()
                .Include(i => i.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(s => s.Framework)
                .Include(i => i.Supplier)
                .Include(i => i.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                .Where(i =>
                i.CatalogueItemType == CatalogueItemType.Solution
                && (i.PublishedStatus == PublicationStatus.Published || i.PublishedStatus == PublicationStatus.InRemediation)
                && i.Supplier.IsActive), new List<CapabilitiesAndCountModel>());

        private static async Task<(IQueryable<CatalogueItem> Query, List<CapabilitiesAndCountModel> Count)> FilteredQuery(
            BuyingCatalogueDbContext dbContext,
            string selectedCapabilityIds,
            string selectedEpicIds)
        {
            var capabilityIds = SolutionsFilterHelper.ParseCapabilityIds(selectedCapabilityIds);

            var epicIds = SolutionsFilterHelper.ParseEpicIds(selectedEpicIds);

            var capabilityParam = CreateIdListParameter(capabilityIds, CapabilityParamName, CapabilityParamType);
            var epicParam = CreateIdListParameter(epicIds, EpicParamName, EpicParamType);

            // old school ADO.NET baby
            using var cmd = dbContext.Database.GetDbConnection().CreateCommand();

            cmd.CommandText = FilterProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(capabilityParam);
            cmd.Parameters.Add(epicParam);

            var wasOpen = cmd.Connection.State == ConnectionState.Open;

            var selectedSolutions = new List<CatalogueItemId>();

            var capabilitiesAndCount = new List<CapabilitiesAndCountModel>();

            try
            {
                if (!wasOpen)
                    await cmd.Connection.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                // gets the list of CatalogueItemIds that meet the filter criteria
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                        selectedSolutions.Add(CatalogueItemId.ParseExact(reader.GetString(0)));
                }

                // gets a list of the selected capabilities and the number of epics under them
                if (await reader.NextResultAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                            capabilitiesAndCount.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
                    }
                }
            }
            finally
            {
                if (!wasOpen)
                    await cmd.Connection.CloseAsync();
            }

            return (dbContext.CatalogueItems
                .AsNoTracking()
                .AsSplitQuery()
                .Include(i => i.Supplier)
                .Include(i => i.Solution)
                    .ThenInclude(s => s.AdditionalServices
                        .Where(adit => selectedSolutions.Contains(adit.CatalogueItemId)))
                    .ThenInclude(adit => adit.CatalogueItem)
                    .ThenInclude(ci => ci.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                .Include(i => i.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(fs => fs.Framework)
                .Include(i => i.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                .Where(i =>
                    i.CatalogueItemType == CatalogueItemType.Solution
                    && selectedSolutions.Contains(i.Id)),
                capabilitiesAndCount);
        }

        private static SqlParameter CreateIdListParameter<T>(ICollection<T> ids, string paramName, string type)
        {
            var table = new DataTable();

            table.Columns.Add(ColumnName);

            foreach (var id in ids)
            {
                var row = table.NewRow();

                row[ColumnName] = id;

                table.Rows.Add(row);
            }

            return new SqlParameter(paramName, SqlDbType.Structured)
            {
                Value = table,
                TypeName = type,
            };
        }
    }
}
