using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home
{
    /// <summary>
    /// Steps for the GPIT Buying Catalogue home page for the GP Option. Ticket 24867.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class StepModel
    {
        public StepModel(string title, string helpText, int number)
        {
            Title = title;
            HelpText = helpText;
            Number = number;
        }

        public int Number { get; set; }

        public string Title { get; set; }

        public string HelpText { get; set; }
    }
}
