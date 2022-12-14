﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database;

internal static class DbContextExtensions
{
    internal static void InsertRangeWithIdentity<T>(this BuyingCatalogueDbContext context, IEnumerable<T> items)
        where T : class
    {
        context.Set<T>().AddRange(items);
        context.SaveChanges();
    }
}
