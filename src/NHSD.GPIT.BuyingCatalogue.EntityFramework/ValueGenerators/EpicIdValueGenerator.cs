using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.ValueGenerators
{
    public sealed class EpicIdValueGenerator : ValueGenerator<string>
    {
        public override bool GeneratesTemporaryValues => false;

        public override string Next(EntityEntry entry)
            => NextAsync(entry).AsTask().GetAwaiter().GetResult();

        public override async ValueTask<string> NextAsync(EntityEntry entry, CancellationToken cancellationToken = default)
        {
            if (entry.Entity is not Epic epic)
                throw new ArgumentException($"Entity must be of type {typeof(Epic).Name}", nameof(entry));

            if (!epic.SupplierDefined)
                return epic.Id;

            if (!string.IsNullOrWhiteSpace(epic.Id))
                return epic.Id;

            var latestSupplierDefinedEpic = await entry.Context.Set<Epic>()
                .AsNoTracking()
                .Where(e => e.SupplierDefined && EF.Functions.Like(e.Id, "S%") && e.Id.Length == 6)
                .OrderByDescending(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var id = 0;
            if (latestSupplierDefinedEpic is not null)
            {
                id = int.Parse(latestSupplierDefinedEpic.Id[1..]);
            }

            return $"S{(id + 1).ToString().PadLeft(5, '0')}";
        }
    }
}
