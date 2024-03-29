﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database;

internal static class DbContextExtensions
{
    internal static void InsertRangeWithIdentity<T>(this BuyingCatalogueDbContext context, IEnumerable<T> items)
        where T : class
        => context.InsertRangeWithIdentityAsync(items).GetAwaiter().GetResult();

    internal static async Task InsertRangeWithIdentityAsync<T>(this BuyingCatalogueDbContext context, IEnumerable<T> items)
        where T : class
    {
        var tableName = context.Set<T>().EntityType.GetSchemaQualifiedTableName();

        using var transaction = context.Database.BeginTransaction();

        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON");

        context.Set<T>().AddRange(items);
        await context.SaveChangesAsync();

        context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} OFF");

        await transaction.CommitAsync();
    }
}
