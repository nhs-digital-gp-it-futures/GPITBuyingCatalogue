using System.Linq;
using System.Text;
using Deque.AxeCore.Commons;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;

public static class AccessibilityReporter
{
    public static void Report(AxeResult result, ITestOutputHelper output, string pageName)
    {
        output.WriteLine($"Accessibility scan: {pageName}");
        output.WriteLine(new string('=', 60));
        output.WriteLine("");

        output.WriteLine($"Violations: {result.Violations.Length}");
        output.WriteLine($"Passed:     {result.Passes.Length}");
        output.WriteLine($"Incomplete: {result.Incomplete.Length}   (needs human review)");
        output.WriteLine("");

        if (result.Violations.Any())
        {
            output.WriteLine("VIOLATIONS");
            output.WriteLine(new string('-', 60));
            PrintItems(result.Violations, output);
        }

        if (result.Incomplete.Any())
        {
            output.WriteLine("INCOMPLETE — axe could not decide, review manually");
            output.WriteLine(new string('-', 60));
            PrintItems(result.Incomplete, output);
        }

        if (!result.Violations.Any() && !result.Incomplete.Any())
            output.WriteLine("No violations or incomplete items detected by axe-core.");
    }

    private static void PrintItems(AxeResultItem[] items, ITestOutputHelper output)
    {
        var ordered = items.OrderBy(v => SeverityRank(v.Impact)).ToList();

        foreach (var item in ordered)
        {
            output.WriteLine($"[{item.Impact?.ToUpper() ?? "UNKNOWN"}] {item.Id}");
            output.WriteLine($"  Rule:      {item.Help}");
            output.WriteLine($"  WCAG tags: {string.Join(", ", item.Tags.Where(t => t.StartsWith("wcag")))}");
            output.WriteLine($"  Elements:  {item.Nodes.Length}");
            output.WriteLine($"  Guidance:  {item.HelpUrl}");

            foreach (var node in item.Nodes.Take(3))
            {
                var target = node.Target?.ToString() ?? "unknown";
                output.WriteLine($"    - {target}");
            }

            if (item.Nodes.Length > 3)
                output.WriteLine($"    ...and {item.Nodes.Length - 3} more");

            output.WriteLine("");
        }
    }

    private static int SeverityRank(string? impact) => impact switch
    {
        "critical" => 0,
        "serious" => 1,
        "moderate" => 2,
        "minor" => 3,
        _ => 4
    };
}
