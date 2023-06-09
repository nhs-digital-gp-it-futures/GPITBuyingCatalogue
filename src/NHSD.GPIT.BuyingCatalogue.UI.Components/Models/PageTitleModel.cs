namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Models
{
    public record PageTitleModel : IPageTitleModel
    {
        public string Title { get; init; }

        public string Caption { get; init; }

        public string Advice { get; init; }

        public string AdditionalAdvice { get; init; }
    }
}
