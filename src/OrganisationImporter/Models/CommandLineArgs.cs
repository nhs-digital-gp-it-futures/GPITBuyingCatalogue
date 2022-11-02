using CommandLine;

namespace OrganisationImporter.Models;

public class CommandLineArgs
{
    [Option('u', "url", Required = true, HelpText = "The URL of the file to import from TRUD")]
    public Uri Url { get; set; }
}