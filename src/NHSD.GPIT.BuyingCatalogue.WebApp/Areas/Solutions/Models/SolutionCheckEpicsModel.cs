namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionCheckEpicsModel
    {
        public string SolutionId { get; set; }

        public string Name { get; set; }

        public string SolutionName { get; set; }

        public string LastReviewed { get; set; }

        public string[] NhsDefined { get; set; }

        public string[] SupplierDefined { get; set; }

        public bool HasNhsDefined()
        {
            return NhsDefined != null && NhsDefined.Length > 0;
        }

        public bool HasSupplierDefined()
        {
            return SupplierDefined != null && SupplierDefined.Length > 0;
        }
    }
}
