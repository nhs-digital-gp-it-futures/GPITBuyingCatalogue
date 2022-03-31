namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public struct SelectableRadioOption<TValue>
    {
        public SelectableRadioOption(
            string name,
            TValue value)
        {
            Name = name;
            Value = value;

            Advice = null;
        }

        public SelectableRadioOption(
            string name,
            string advice,
            TValue value)
        {
            Name = name;
            Advice = advice;
            Value = value;
        }

        public string Name { get; set; }

        public string Advice { get; set; }

        public TValue Value { get; set; }
    }
}
