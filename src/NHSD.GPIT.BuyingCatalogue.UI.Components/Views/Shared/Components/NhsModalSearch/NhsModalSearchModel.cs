namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsModalSearch
{
    public sealed class NhsModalSearchModel
    {
        public string Id { get; set; }

        public string ShowDialogButton { get; set; }

        public string Title { get; set; }

        public string Advice { get; set; }

        public string NotFoundText { get; set; }

        public string Placeholder { get; set; }

        public string ApplyButtonText { get; set; }

        public string TablePartialView { get; set; }

        public object TableData { get; set; }

        public string CallbackFunction { get; set; }

        public string TableContentFunction { get; set; }

        public bool ClearSearch { get; set; }

        public bool ClearSelection { get; set; }
    }
}
