using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrganisationImporter.Models;
using OrganisationImporter.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
          .WriteTo.Console()
          .CreateLogger();

var services = new ServiceCollection()
    .AddSingleton<OrganisationImportService>()
    .AddLogging(builder => builder.AddSerilog());

var serviceProvider = services.BuildServiceProvider();

Parser
    .Default
    .ParseArguments<CommandLineArgs>(args)
    .WithNotParsed(errors =>
    {
        var logger = serviceProvider.GetService<ILogger<Program>>();

        logger.LogError("Failed to parse command line args.");
    })
    .WithParsedAsync(parsedArgs =>
    {
        var organisationImportService = serviceProvider
            .GetService<OrganisationImportService>();

        return organisationImportService!.ImportFromUrl(parsedArgs.Url);
    });