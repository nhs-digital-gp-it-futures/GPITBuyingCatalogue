namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class FrameworkFilter
    {
        public string FrameworkId { get; set; }

        public int Count { get; set; }

        public string DisplayText => $"{FrameworkFullName} ({Count})";

        public string FrameworkFullName { get; init; }
    }
}
