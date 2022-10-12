using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv;

public abstract class CsvServiceBase
{
    protected static async Task WriteRecordsAsync<TEntity, TClassMap>(MemoryStream stream, IEnumerable<TEntity> items)
        where TClassMap : ClassMap<TEntity>
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        await using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
        await using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy" } });

        csvWriter.Context.RegisterClassMap<TClassMap>();

        await csvWriter.WriteRecordsAsync(items);
    }
}
